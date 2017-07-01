using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECS
{
    internal enum ChangeType
    {
        EntityAdded,
        EntityRemoved,
        ComponentAdded,
        ComponentRemoved,
    }

    public class EntityComponentSystem
    {
        private int nextEntityId;

        // first index = entity, second index = component index
        private object[][] components = new[] { new object[1] };
        // key = entity index
        private BigInteger[] entityComponentCache = new[] { new BigInteger() };
        private readonly ConcurrentQueue<(ChangeType changeType, int entityId, object component)> changes = new ConcurrentQueue<(ChangeType changeType, int entityId, object component)>();

        // key = component index
        private AsyncReaderWriterLock[] componentLocks = new[] { new AsyncReaderWriterLock() };
        private SemaphoreSlim componentLocksAcquirer = new SemaphoreSlim(0, 1);
        private readonly Subject<BigInteger> typesFreed = new Subject<BigInteger>();

        private readonly ComponentMapper componentMapper = new ComponentMapper();

        /// <summary>
        /// Thread safe. The entity starts existing after a call to Flush.
        /// </summary>
        /// <returns>Entity Id</returns>
        public int CreateEntity()
        {
            var entityId = Interlocked.Increment(ref nextEntityId);
            changes.Enqueue((ChangeType.EntityAdded, entityId, null));
            return entityId;
        }

        public void RemoveEntity(int entityId)
        {
            changes.Enqueue((ChangeType.EntityRemoved, entityId, null));
        }

        /// <summary>
        /// Thread safe. The component starts existing after a call to Flush. Component modifications are allowed until a call to Flush.
        /// </summary>
        /// <returns>Entity Id</returns>
        public void AddComponent<T>(int entityId, T component)
        {
            changes.Enqueue((ChangeType.ComponentAdded, entityId, component));
        }

        /// <summary>
        /// Thread safe. The component starts existing after a call to Flush. Component modifications are allowed until a call to Flush.
        /// </summary>
        /// <returns>Entity Id</returns>
        public void AddComponent(int entityId, object component)
        {
            changes.Enqueue((ChangeType.ComponentAdded, entityId, component));
        }

        /// <summary>
        /// Thread safe. The component is removed after a call to Flush.
        /// </summary>
        public void RemoveComponent<T>(int entityId, T component)
        {
            changes.Enqueue((ChangeType.ComponentRemoved, entityId, component));
        }

        public Task WhenCanTryAcquireComponents(BigInteger componentTypes)
        {
            var semaphore = new SemaphoreSlim(1, 1);
            var disposable = typesFreed.Subscribe(types =>
            {
                if (componentMapper.Intersects(componentTypes, types))
                {
                    semaphore.Release();
                }
            });
            return semaphore
                .WaitAsync()
                .ContinueWith(t => disposable.Dispose());
        }

        public async Task WhenCanTryAcquireAllComponents()
        {
            await typesFreed.FirstAsync();
        }

        public async Task<ComponentAccess> GetEntities(Type[] componentTypes, ComponentAccessMode[] accessModes)
        {
            var typesHash = componentMapper.TypesToBigInteger(componentTypes);

            var first = true;

            start:
            if (first)
            {
                first = false;
            }
            else
            {
                await WhenCanTryAcquireComponents(typesHash).ConfigureAwait(false);
            }
            await componentLocksAcquirer.WaitAsync().ConfigureAwait(false);

            var gotAllLocks = true;
            var acquiredLocks = new IDisposable[componentTypes.Length];
            for (int i = 0; i < componentTypes.Length; i++)
            {
                var accessMode = accessModes[i];
                var componentType = componentTypes[i];
                var componentIndex = componentMapper.GetIndexForType(componentType);

                IDisposable acquiredLock;
                if (accessModes[i] == ComponentAccessMode.Write)
                {
                    acquiredLock = componentLocks[componentIndex].TryWriterLock();
                }
                else
                {
                    acquiredLock = componentLocks[componentIndex].TryReaderLock();
                }

                if (acquiredLock == null)
                {
                    gotAllLocks = false;
                    for (int j = 0; j < i; j++)
                    {
                        acquiredLocks[j].Dispose();
                    }
                    break;
                }

                acquiredLocks[i] = acquiredLock;
            }

            componentLocksAcquirer.Release();

            if (!gotAllLocks) goto start;

            // At this point we got all locks and prepare for return to caller

            var componentIndices = new int[componentTypes.Length];
            for (int i = 0; i < componentTypes.Length; i++)
            {
                componentIndices[i] = componentMapper.GetIndexForType(componentTypes[i]);
            }

            var entitiesWithComponents = ImmutableDictionary.CreateBuilder<int, ImmutableDictionary<Type, object>>();
            for (int entityIndex = 0; entityIndex < components.Length; entityIndex++)
            {
                var interested = componentMapper.Interested(entityComponentCache[entityIndex], typesHash);
                if (!interested) continue;

                var entityComponents = components[entityIndex];

                var builder = ImmutableDictionary.CreateBuilder<Type, object>();

                for (int componentIndexRet = 0; componentIndexRet < componentIndices.Length; componentIndexRet++)
                {
                    var entityComponent = entityComponents[componentIndices[componentIndexRet]];
                    var entityComponentType = componentTypes[componentIndexRet];
                    builder.Add(entityComponentType, entityComponent);
                }

                entitiesWithComponents.Add(entityIndex, builder.ToImmutable());
            }

            return new ComponentAccess(() =>
            {
                foreach (var acquiredLock in acquiredLocks)
                {
                    acquiredLock.Dispose();
                }

                typesFreed.OnNext(typesHash);
            }, entitiesWithComponents.ToImmutable());
        }

        public async Task<ComponentAccess> GetAllEntities(ComponentAccessMode accessMode)
        {
            var first = true;

            start:
            if (first)
            {
                first = false;
            }
            else
            {
                await WhenCanTryAcquireAllComponents().ConfigureAwait(false);
            }
            await componentLocksAcquirer.WaitAsync().ConfigureAwait(false);

            var componentTypes = componentMapper.AllComponentTypes;
            var typesHash = componentMapper.TypesToBigInteger(componentTypes);

            var gotAllLocks = true;
            var acquiredLocks = new IDisposable[componentTypes.Count];
            for (int i = 0; i < componentTypes.Count; i++)
            {
                var componentIndex = componentMapper.GetIndexForType(componentTypes[i]);

                IDisposable acquiredLock;
                if (accessMode == ComponentAccessMode.Write)
                {
                    acquiredLock = componentLocks[componentIndex].TryWriterLock();
                }
                else
                {
                    acquiredLock = componentLocks[componentIndex].TryReaderLock();
                }

                if (acquiredLock == null)
                {
                    gotAllLocks = false;
                    for (int j = 0; j < i; j++)
                    {
                        acquiredLocks[j].Dispose();
                    }
                    break;
                }

                acquiredLocks[i] = acquiredLock;
            }

            componentLocksAcquirer.Release();

            if (!gotAllLocks) goto start;

            // At this point we got all locks and prepare for return to caller

            var entitiesWithComponents = ImmutableDictionary.CreateBuilder<int, ImmutableDictionary<Type, object>>();
            for (int entityIndex = 0; entityIndex < components.Length; entityIndex++)
            {
                var entityComponents = components[entityIndex];

                var builder = ImmutableDictionary.CreateBuilder<Type, object>();

                for (int componentIndexRet = 0; componentIndexRet < componentTypes.Count; componentIndexRet++)
                {
                    var componentType = componentTypes[componentIndexRet];
                    var componentIndex = componentMapper.GetIndexForType(componentType);

                    if (componentIndex >= entityComponents.Length) break;

                    var entityComponent = entityComponents[componentIndex];
                    
                    builder.Add(componentType, entityComponent);
                }

                entitiesWithComponents.Add(entityIndex, builder.ToImmutable());
            }

            return new ComponentAccess(() =>
            {
                foreach (var acquiredLock in acquiredLocks)
                {
                    acquiredLock.Dispose();
                }

                typesFreed.OnNext(typesHash);
            }, entitiesWithComponents.ToImmutable());
        }

        /// <summary>
        /// NOT thread safe. Other operations are not allowed if this is being called. Makes all pending changes concrete.
        /// </summary>
        /// <returns></returns>
        public void Flush()
        {
            // process pending changes and make them concrete
            // resize components array as needed
            while (changes.TryDequeue(out var change))
            {
                switch (change.changeType)
                {
                    case ChangeType.EntityAdded:
                        if (change.entityId >= components.Length)
                        {
                            Array.Resize(ref components, components.Length * 2);
                            Array.Resize(ref entityComponentCache, entityComponentCache.Length * 2);
                        }
                        Debug.Assert(entityComponentCache[change.entityId] == BigInteger.Zero);
                        // notify subscribers
                        break;
                    case ChangeType.EntityRemoved:
                        Debug.Assert(entityComponentCache[change.entityId] == BigInteger.Zero);
                        // notify subscribers
                        break;
                    case ChangeType.ComponentAdded:
                        {
                            var componentType = change.component.GetType();
                            var componentIndex = componentMapper.GetIndexForType(componentType);
                            var componentArray = components[change.entityId];
                            if (componentIndex >= componentArray.Length)
                            {
                                Array.Resize(ref componentArray, componentArray.Length * 2);
                                Array.Resize(ref componentLocks, componentLocks.Length * 2);
                            }
                            Debug.Assert(componentArray[componentIndex] == null);
                            componentMapper.AddTypeToBigInteger(ref entityComponentCache[change.entityId], componentType);
                            componentArray[componentIndex] = change.component;
                            // notify subscribers
                        }
                        break;
                    case ChangeType.ComponentRemoved:
                        {
                            // notify subscribers
                            var componentType = change.component.GetType();
                            var componentIndex = componentMapper.GetIndexForType(componentType);
                            var componentArray = components[change.entityId];
                            Debug.Assert(componentArray[componentIndex] != null);
                            componentMapper.RemoveTypeFromBigInteger(ref entityComponentCache[change.entityId], componentType);
                            componentArray[componentIndex] = null;
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
