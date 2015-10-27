﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECS
{
    internal class EntityManager : IEntityUtility
    {
        private int nextEntityId;
        private int componentArraySize;

        // key = component type, value = array of components where key is entity id
        private readonly Dictionary<Type, IComponent[]> components;

        // array of component bitsets where key is entity id
        // null means that entity doesn't exist
        private BigInteger?[] entityComponentBits;
        private Entity[] entityCache;
        private Dictionary<int, bool>[] entityInterestedCache; 

        // key = entity id, value 1 = component type, value 2 = component instance
        private readonly Queue<Tuple<int, Type, IComponent>> pendingComponentAdds;

        // key = entity id, value = component type
        private readonly Queue<Tuple<int, Type>> pendingComponentRemovals;

        private readonly Queue<Entity> pendingEntityAdds;

        // key = entity id
        private readonly Queue<int> pendingEntityRemovals;

        private readonly Queue<EntityChange> pendingChangeOrder;  

        private enum EntityChange
        {
            AddEntity,
            RemoveEntity,
            AddComponent,
            RemoveComponent,
        }

        internal EntityManager()
        {
            components = new Dictionary<Type, IComponent[]>();
            entityComponentBits = new BigInteger?[0];
            entityCache = new Entity[0];
            entityInterestedCache = new Dictionary<int, bool>[0];

            pendingComponentAdds = new Queue<Tuple<int, Type, IComponent>>();
            pendingComponentRemovals = new Queue<Tuple<int, Type>>();
            pendingEntityAdds = new Queue<Entity>();
            pendingEntityRemovals = new Queue<int>();
            pendingChangeOrder = new Queue<EntityChange>();
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(nextEntityId++, this);
            pendingEntityAdds.Enqueue(entity);
            pendingChangeOrder.Enqueue(EntityChange.AddEntity);
            FlushPending();
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
                Array.Resize(ref entityInterestedCache, componentArraySize);
            }

            var entity = pendingEntityAdds.Dequeue();
            entityComponentBits[entity.Id] = BigInteger.Zero;
            entityCache[entity.Id] = new Entity(entity.Id, this);

            if (entityInterestedCache[entity.Id] == null)
            {
                entityInterestedCache[entity.Id] = new Dictionary<int, bool>();
            }
        }

        public void RemoveEntity(Entity entity)
        {
            foreach (var component in GetComponents(entity))
            {
                RemoveComponent(entity, component);
            }

            pendingEntityRemovals.Enqueue(entity.Id);
            pendingChangeOrder.Enqueue(EntityChange.RemoveEntity);
        } 

        internal void FlushEntityRemovalOnce()
        {
            var id = pendingEntityRemovals.Dequeue();
            foreach (var pair in components)
            {
                pair.Value[id] = null;
            }
            entityComponentBits[id] = null;
            entityCache[id] = null;
            entityInterestedCache[id].Clear();
        }

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            pendingComponentAdds.Enqueue(Tuple.Create(entity.Id, typeof(T), (IComponent)component));
            pendingChangeOrder.Enqueue(EntityChange.AddComponent);
            FlushPending();
        }

        internal void FlushComponentAddOnce()
        {
            var tuple = pendingComponentAdds.Dequeue();
            var id = tuple.Item1;
            var type = tuple.Item2;
            var component = tuple.Item3;

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
            entityInterestedCache[id].Clear();
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            pendingComponentRemovals.Enqueue(Tuple.Create(entity.Id, typeof(T)));
            pendingChangeOrder.Enqueue(EntityChange.RemoveComponent);
        }

        public void RemoveComponent(Entity entity, IComponent component)
        {
            pendingComponentRemovals.Enqueue(Tuple.Create(entity.Id, component.GetType()));
            pendingChangeOrder.Enqueue(EntityChange.RemoveComponent);
        }

        internal void FlushComponentRemovalOnce()
        {
            var tuple = pendingComponentRemovals.Dequeue();
            var id = tuple.Item1;
            var type = tuple.Item2;
            components[type][id] = null;
            entityComponentBits[id] &= ~AspectMapper.TypesToBigInteger(type);
            entityInterestedCache[id].Clear();
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
            var ret = new List<Entity>();
            for (int i = 0; i < componentArraySize; i++)
            {
                if (entityComponentBits[i] == null) continue;

                var aspectHash = aspect.GetHashCode();
                bool interested;
                if (!entityInterestedCache[i].TryGetValue(aspectHash, out interested))
                {
                    interested = aspect.Interested(entityComponentBits[i].Value);
                    entityInterestedCache[i][aspectHash] = interested;
                }

                if (interested)
                {
                    ret.Add(entityCache[i]);
                }
            }
            return ret;
        }

        internal void FlushPending()
        {
            while (pendingChangeOrder.Any())
            {
                var change = pendingChangeOrder.Dequeue();
                switch (change)
                {
                    case EntityChange.AddEntity:
                        FlushEntityAddOnce();
                        break;
                    case EntityChange.RemoveEntity:
                        FlushEntityRemovalOnce();
                        break;
                    case EntityChange.AddComponent:
                        FlushComponentAddOnce();
                        break;
                    case EntityChange.RemoveComponent:
                        FlushComponentRemovalOnce();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
