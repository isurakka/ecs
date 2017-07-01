using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        // first index = entity, second index = component
        private object[][] components = new[] { new object[1] };
        private readonly ConcurrentQueue<(ChangeType changeType, int entityId, object component)> changes = new ConcurrentQueue<(ChangeType changeType, int entityId, object component)>();

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

        private class EntityComponentAccessor : IDictionary<Type, object>
        {

        }

        public Task<ComponentAccess> GetEntities(Type[] types, ComponentAccessMode[] accessModes)
        {
            
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
                        }
                        // notify subscribers
                        break;
                    case ChangeType.EntityRemoved:
                        // notify subscribers
                        break;
                    case ChangeType.ComponentAdded:
                        {
                            var componentIndex = componentMapper.GetIndexForType(change.component.GetType());
                            var componentArray = components[change.entityId];
                            if (componentIndex >= componentArray.Length)
                            {
                                Array.Resize(ref componentArray, componentArray.Length * 2);
                            }
                            Debug.Assert(componentArray[componentIndex] == null);
                            componentArray[componentIndex] = change.component;
                            // notify subscribers
                        }
                        break;
                    case ChangeType.ComponentRemoved:
                        {
                            // notify subscribers
                            var componentIndex = componentMapper.GetIndexForType(change.component.GetType());
                            var componentArray = components[change.entityId];
                            Debug.Assert(componentArray[componentIndex] != null);
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
