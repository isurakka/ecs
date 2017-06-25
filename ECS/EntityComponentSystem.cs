using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                        if (change.entityId > components.GetLength(0))
                        {
                            Array.Resize(ref components, components.GetLength(0) * 2);
                        }
                        break;
                    case ChangeType.EntityRemoved:
                        break;
                    case ChangeType.ComponentAdded:
                        break;
                    case ChangeType.ComponentRemoved:
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
