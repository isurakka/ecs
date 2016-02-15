using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECS
{
    public class EntityChange
    {
        public enum ChangeType
        {
            EntityAdded,
            EntityRemoved,
            ComponentAdded,
            ComponentRemoved,
        }

        public Entity Entity { get; }
        public IComponent RelevantComponent { get; }

        private Type componentType;
        public Type ComponentType => componentType ?? (componentType = RelevantComponent?.GetType());

        public ChangeType TypeOfChange { get; set; }

        protected EntityChange(Entity entity, IComponent relevantComponent, Type componentType, ChangeType typeOfChange)
        {
            Entity = entity;
            RelevantComponent = relevantComponent;
            this.componentType = componentType;
            TypeOfChange = typeOfChange;
        }

        public static EntityChange CreateEntityAdded(Entity entity)
            => new EntityChange(entity, null, null, ChangeType.EntityAdded);

        public static EntityChange CreateEntityRemoved(Entity entity)
            => new EntityChange(entity, null, null, ChangeType.EntityRemoved);

        public static EntityChange CreateComponentAdded(Entity entity, IComponent component, Type componentType)
            => new EntityChange(entity, component, componentType, ChangeType.ComponentAdded);

        public static EntityChange CreateComponentRemoved(Entity entity, IComponent component, Type componentType)
            => new EntityChange(entity, component, componentType, ChangeType.ComponentRemoved);
    }

    internal class EntityManager : IEntityUtility
    {
        private int nextEntityId;
        private int componentArraySize;

        // key = component type, value = array of components where key is entity id
        private readonly Dictionary<Type, IComponent[]> components;

        // array of component bitsets where key is entity id
        // null means that entity doesn't exist
        internal BigInteger?[] entityComponentBits;
        internal Entity[] entityCache;
        //private Dictionary<int, bool>[] entityInterestedCache;
        private readonly Queue<EntityChange> pendingChanges;
        internal readonly List<EntityChange> changesInLastFlush;

        //public event EventHandler<Entity> EntityChanged;

        internal bool Updating { get; set; }

        internal EntityManager()
        {
            components = new Dictionary<Type, IComponent[]>();
            entityComponentBits = new BigInteger?[0];
            entityCache = new Entity[0];
            //entityInterestedCache = new Dictionary<int, bool>[0];

            pendingChanges = new Queue<EntityChange>();
            changesInLastFlush = new List<EntityChange>();
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(nextEntityId++, this);
            var entityChange = EntityChange.CreateEntityAdded(entity);
            pendingChanges.Enqueue(entityChange);

            if (Updating) return entity;

            FlushEntityAddOnce();
            changesInLastFlush.Add(entityChange);
            return entity;
        }

        internal void FlushEntityAddOnce()
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

                Array.Resize(ref entityComponentBits, componentArraySize);
                Array.Resize(ref entityCache, componentArraySize);
                //Array.Resize(ref entityInterestedCache, componentArraySize);
            }

            var entityChange = pendingChanges.Dequeue();
            entityComponentBits[entityChange.Entity.Id] = BigInteger.Zero;
            entityCache[entityChange.Entity.Id] = entityChange.Entity;

            //if (entityInterestedCache[entityChange.Entity.Id] == null)
            //{
            //    entityInterestedCache[entityChange.Entity.Id] = new Dictionary<int, bool>();
            //}
        }

        public void RemoveEntity(Entity entity)
        {
            foreach (var component in GetComponents(entity))
            {
                RemoveComponent(entity, component);
            }

            var entityChange = EntityChange.CreateEntityRemoved(entity);
            pendingChanges.Enqueue(entityChange);

            if (Updating) return;

            FlushEntityRemovalOnce();
            changesInLastFlush.Add(entityChange);
        } 

        internal void FlushEntityRemovalOnce()
        {
            var id = pendingChanges.Dequeue().Entity.Id;
            foreach (var pair in components)
            {
                pair.Value[id] = null;
            }
            entityComponentBits[id] = null;
            entityCache[id] = null;
            //entityInterestedCache[id].Clear();
        }

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            var entityChange = EntityChange.CreateComponentAdded(entity, component, typeof (T));
            pendingChanges.Enqueue(entityChange);

            if (Updating) return;

            FlushComponentAddOnce();
            changesInLastFlush.Add(entityChange);
        }

        internal void FlushComponentAddOnce()
        {
            var entityChange = pendingChanges.Dequeue();
            var id = entityChange.Entity.Id;
            var type = entityChange.RelevantComponent.GetType();
            var component = entityChange.RelevantComponent;

            if (!components.ContainsKey(type))
            {
                components.Add(type, new IComponent[componentArraySize]);
            }
            // only check this if this wasn't first component of this type to be added
            else if (components[type][id] != null)
            {
                throw new InvalidOperationException(
                    $@"Entity of type {type.Name} is already " +
                    $"added to entity with {nameof(id)} {id}");
            }

            components[type][id] = component;
            entityComponentBits[id] |= AspectMapper.TypesToBigInteger(type);
            //entityInterestedCache[id].Clear();
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            var entityChange = EntityChange.CreateComponentRemoved(entity, null, typeof (T));
            pendingChanges.Enqueue(entityChange);

            if (Updating) return;

            FlushComponentRemovalOnce();
            changesInLastFlush.Add(entityChange);
        }

        public void RemoveComponent(Entity entity, IComponent component)
        {
            var entityChange = EntityChange.CreateComponentRemoved(entity, component, null);
            pendingChanges.Enqueue(entityChange);

            if (Updating) return;

            FlushComponentRemovalOnce();
            changesInLastFlush.Add(entityChange);
        }

        internal void FlushComponentRemovalOnce()
        {
            var entityChange = pendingChanges.Dequeue();
            var id = entityChange.Entity.Id;
            var type = entityChange.ComponentType;
            components[type][id] = null;
            entityComponentBits[id] &= ~AspectMapper.TypesToBigInteger(type);
            //entityInterestedCache[id].Clear();
        }

        public IEnumerable<IComponent> GetComponents(Entity entity)
        {
            return components.Values
                .Select(arr => arr[entity.Id])
                .Where(c => c != null);
        }

        public T GetComponent<T>(Entity entity) where T: IComponent
        {
            if (!components.ContainsKey(typeof(T))) return default(T);

            return (T)components[typeof(T)][entity.Id];
        }

        public bool HasComponent<T>(Entity entity) where T : IComponent
        {
            if (!components.ContainsKey(typeof(T)) || entity.Id >= componentArraySize) return false;

            return components[typeof(T)][entity.Id] != null;
        }

        public bool HasComponent(Entity entity, IComponent component)
        {
            if (!components.ContainsKey(component.GetType())) return false;

            return components[component.GetType()][entity.Id] != null;
        }

        internal IEnumerable<Entity> GetEntitiesForAspect(Aspect aspect)
        {
            /*
            var ret = new List<Entity>();
            for (int i = 0; i < componentArraySize; i++)
            {
                if (entityComponentBits[i] == null) continue;

                var aspectHash = aspect.GetHashCode();
                bool interested;
                if (!entityInterestedCache[i].TryGetValue(aspectHash, out interested))
                {
                    // TODO: Don't cache aspect here
                    interested = aspect.Cache.Interested(entityComponentBits[i].Value);
                    entityInterestedCache[i][aspectHash] = interested;
                }

                if (interested)
                {
                    ret.Add(entityCache[i]);
                }
            }
            return ret;
            */

            for (int i = 0; i < componentArraySize; i++)
            {
                if (entityComponentBits[i] == null) continue;

                if (aspect.Cache.Interested(entityComponentBits[i].Value)) yield return entityCache[i];
            }
        }

        internal void FlushPending()
        {
            changesInLastFlush.Clear();
            while (pendingChanges.Any())
            {
                var entityChange = pendingChanges.Peek();
                switch (entityChange.TypeOfChange)
                {
                    case EntityChange.ChangeType.EntityAdded:
                        FlushEntityAddOnce();
                        break;
                    case EntityChange.ChangeType.EntityRemoved:
                        FlushEntityRemovalOnce();
                        break;
                    case EntityChange.ChangeType.ComponentAdded:
                        FlushComponentAddOnce();
                        break;
                    case EntityChange.ChangeType.ComponentRemoved:
                        FlushComponentRemovalOnce();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                changesInLastFlush.Add(entityChange);
            }
        }
    }
}
