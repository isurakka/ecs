using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ECS
{
    public partial class EntityComponentSystem
    {
        private int nextEntityId;
        private int componentArraySize;

        // component related
        // key = component type, value = array of components where key is entity id
        private readonly Dictionary<Type, object[]> components = new Dictionary<Type, object[]>();
        private readonly Dictionary<Type, AsyncReaderWriterLock> componentLocks = new Dictionary<Type, AsyncReaderWriterLock>();
        private readonly SemaphoreSlim componentLockAcquirer = new SemaphoreSlim(0, 1);
        private readonly AsyncManualResetEvent componentLocksFreed = new AsyncManualResetEvent(true); 
        
        // entity related
        // array of component bitsets where key is entity id
        // null means that entity doesn't exist
        internal BigInteger?[] EntityComponentBits = new BigInteger?[0];
        private Entity[] entityCache = new Entity[0];
        private Dictionary<int, bool>[] entityInterestedCache = new Dictionary<int, bool>[0];
        private readonly ThreadLocal<Queue<EntityChange>> pendingEntityChanges = new ThreadLocal<Queue<EntityChange>>(() => new Queue<EntityChange>(), true);
        
        private static EntityComponentSystem instance;
        /// <summary>
        /// Get the singleton instance. This will probably not be singleton in the future.
        /// </summary>
        public static EntityComponentSystem Instance 
            => instance ?? (instance = new EntityComponentSystem());

        private EntityComponentSystem() {  }

        /// <summary>
        /// Thread safe
        /// </summary>
        public Entity CreateEntity()
        {
            var entity = new Entity(Interlocked.Increment(ref nextEntityId), this);
            pendingEntityChanges.Value.Enqueue(EntityChange.CreateEntityAdded(entity));
            return entity;
        }

        private void FlushEntityAdd(EntityChange entityChange)
        {
            if (componentArraySize < nextEntityId)
            {
                componentArraySize = componentArraySize > 0 ? componentArraySize * 2 : 1;
                foreach (var key in components.Keys.ToArray())
                {
                    var value = components[key];
                    Array.Resize(ref value, componentArraySize);
                    components[key] = value;
                }

                Array.Resize(ref EntityComponentBits, componentArraySize);
                Array.Resize(ref entityCache, componentArraySize);
                Array.Resize(ref entityInterestedCache, componentArraySize);
            }

            EntityComponentBits[entityChange.Entity.Id] = BigInteger.Zero;
            entityCache[entityChange.Entity.Id] = entityChange.Entity;

            if (entityInterestedCache[entityChange.Entity.Id] == null)
            {
                entityInterestedCache[entityChange.Entity.Id] = new Dictionary<int, bool>();
            }
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public void RemoveEntity(Entity entity)
        {
            foreach (var component in GetComponents(entity))
            {
                RemoveComponent(entity, component);
            }

            pendingEntityChanges.Value.Enqueue(EntityChange.CreateEntityRemoved(entity));
        }

        private void FlushEntityRemoval(EntityChange entityChange)
        {
            var id = entityChange.Entity.Id;
            foreach (var pair in components)
            {
                pair.Value[id] = null;
            }
            EntityComponentBits[id] = null;
            entityCache[id] = null;
            entityInterestedCache[id].Clear();
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public void AddComponent<T>(Entity entity, T component)
        {
            pendingEntityChanges.Value.Enqueue(EntityChange.CreateComponentAdded(entity, component, typeof(T)));
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public T AddComponent<T>(Entity entity) where T : new()
        {
            var component = new T();
            pendingEntityChanges.Value.Enqueue(EntityChange.CreateComponentAdded(entity, component, typeof(T)));
            return component;
        }

        private void FlushComponentAdd(EntityChange entityChange)
        {
            var id = entityChange.Entity.Id;
            var type = entityChange.RelevantComponent.GetType();
            var component = entityChange.RelevantComponent;

            if (!components.ContainsKey(type))
            {
                components.Add(type, new object[componentArraySize]);
                componentLocks.Add(type, new AsyncReaderWriterLock());
            }
            // only check this if this wasn't first component of this type to be added
            else if (components[type][id] != null)
            {
                throw new InvalidOperationException(
                    $@"Entity of type {type.Name} is already " +
                    $"added to entity with {nameof(id)} {id}");
            }

            components[type][id] = component;
            EntityComponentBits[id] |= AspectMapper.TypesToBigInteger(type);
            entityInterestedCache[id].Clear();
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public void RemoveComponent<T>(Entity entity)
        {
            pendingEntityChanges.Value.Enqueue(EntityChange.CreateComponentRemoved(entity, null, typeof(T)));
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public void RemoveComponent(Entity entity, object component)
        {
            pendingEntityChanges.Value.Enqueue(EntityChange.CreateComponentRemoved(entity, component, null));
        }

        private void FlushComponentRemoval(EntityChange entityChange)
        {
            var id = entityChange.Entity.Id;
            var type = entityChange.ComponentType;
            components[type][id] = null;
            EntityComponentBits[id] &= ~AspectMapper.TypesToBigInteger(type);
            entityInterestedCache[id].Clear();
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public IEnumerable<object> GetComponents(Entity entity)
        {
            return components.Values
                .Select(arr => arr[entity.Id])
                .Where(c => c != null);
        }

        public async Task<ComponentAccess<IEnumerable<TRead0>>> GetComponents<TRead, TRead0>()
            where TRead: Read
        {
            start:
            await componentLocksFreed.WaitAsync().ConfigureAwait(false);
            await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

            var cts = new CancellationTokenSource();

            var read0LockTask = componentLocks[typeof(TRead0)].ReaderLockAsync(cts.Token).ConfigureAwait(false);

            if (!read0LockTask.GetAwaiter().IsCompleted)
            {
                cts.Cancel();
                componentLockAcquirer.Release();
                goto start;
            }

            componentLockAcquirer.Release();

            return new ComponentAccess<IEnumerable<TRead0>>
            {
                Acquires = new List<IDisposable> { await read0LockTask },
                Components = components[typeof(TRead0)].Cast<TRead0>(),
            };
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public T GetComponent<T>(Entity entity)
        {
            if (!components.ContainsKey(typeof(T))) return default(T);

            return (T)components[typeof(T)][entity.Id];
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public object GetComponent(Entity entity, Type type)
        {
            if (!components.ContainsKey(type)) return null;

            return components[type][entity.Id];
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public bool HasComponent<T>(Entity entity)
        {
            if (!components.ContainsKey(typeof(T)) || entity.Id >= componentArraySize) return false;

            return components[typeof(T)][entity.Id] != null;
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public bool HasComponent(Entity entity, Type type)
        {
            if (!components.ContainsKey(type) || entity.Id >= componentArraySize) return false;

            return components[type][entity.Id] != null;
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public bool HasComponent(Entity entity, object component)
        {
            if (!components.ContainsKey(component.GetType())) return false;

            return components[component.GetType()][entity.Id] != null;
        }

        private IEnumerable<Entity> GetEntitiesForAspect(Aspect aspect)
        {
            var ret = new List<Entity>();
            for (int i = 0; i < componentArraySize; i++)
            {
                if (EntityComponentBits[i] == null) continue;

                var aspectHash = aspect.GetHashCode();
                if (!entityInterestedCache[i].TryGetValue(aspectHash, out bool interested))
                {
                    // TODO: Don't cache aspect here
                    interested = aspect.Cache.Interested(EntityComponentBits[i].Value);
                    entityInterestedCache[i][aspectHash] = interested;
                }

                if (interested)
                {
                    ret.Add(entityCache[i]);
                }
            }
            return ret;
        }

        private IEnumerable<EntityChange> FlushPending()
        {
            var entityChanges = new List<EntityChange>();

            foreach (var changeQueue in pendingEntityChanges.Values)
            {
                while (changeQueue.Any())
                {
                    var entityChange = changeQueue.Dequeue();
                    switch (entityChange.TypeOfChange)
                    {
                        case EntityChange.ChangeType.EntityAdded:
                            FlushEntityAdd(entityChange);
                            break;
                        case EntityChange.ChangeType.EntityRemoved:
                            FlushEntityRemoval(entityChange);
                            break;
                        case EntityChange.ChangeType.ComponentAdded:
                            FlushComponentAdd(entityChange);
                            break;
                        case EntityChange.ChangeType.ComponentRemoved:
                            FlushComponentRemoval(entityChange);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    entityChanges.Add(entityChange);
                }
            }

            return entityChanges;
        }

        // TODO: Is this needed and is this right place for this method?
        public IEnumerable<Entity> FindEntities(Aspect aspect) => GetEntitiesForAspect(aspect);

        /// <summary>
        /// Thread safe
        /// </summary>
        public Entity GetEntity(int id)
        {
            return entityCache[id];
        }

        public void FlushChanges()
        {
            var entityChanges = FlushPending();
        }
    }
}