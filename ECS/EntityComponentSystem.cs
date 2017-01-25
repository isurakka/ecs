using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECS
{
    public class EntityComponentSystem
    {
        private int nextEntityId;
        private int componentArraySize;

        // key = component type, value = array of components where key is entity id
        private readonly Dictionary<Type, IComponent[]> components;

        // entity related
        // array of component bitsets where key is entity id
        // null means that entity doesn't exist
        internal BigInteger?[] EntityComponentBits;
        private Entity[] entityCache;
        private Dictionary<int, bool>[] entityInterestedCache;
        private readonly Queue<EntityChange> pendingEntityChanges;

        // system related
        private readonly Dictionary<int, Queue<SystemChange>> pendingSystemChanges;
        private readonly SortedDictionary<int, List<System>> systems;

        private static EntityComponentSystem instance;
        /// <summary>
        /// Get the singleton instance. This will probably not be singleton in the future.
        /// </summary>
        public static EntityComponentSystem Instance 
            => instance ?? (instance = new EntityComponentSystem());

        private EntityComponentSystem()
        {
            
            components = new Dictionary<Type, IComponent[]>();
            EntityComponentBits = new BigInteger?[0];
            entityCache = new Entity[0];
            entityInterestedCache = new Dictionary<int, bool>[0];
            pendingEntityChanges = new Queue<EntityChange>();

            pendingSystemChanges = new Dictionary<int, Queue<SystemChange>>();
            systems = new SortedDictionary<int, List<System>>();
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(nextEntityId++, this);
            pendingEntityChanges.Enqueue(EntityChange.CreateEntityAdded(entity));
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

        public void RemoveEntity(Entity entity)
        {
            foreach (var component in GetComponents(entity))
            {
                RemoveComponent(entity, component);
            }

            pendingEntityChanges.Enqueue(EntityChange.CreateEntityRemoved(entity));
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

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            pendingEntityChanges.Enqueue(EntityChange.CreateComponentAdded(entity, component, typeof(T)));
        }

        public T AddComponent<T>(Entity entity) where T : IComponent, new()
        {
            var component = new T();
            pendingEntityChanges.Enqueue(EntityChange.CreateComponentAdded(entity, component, typeof(T)));
            return component;
        }

        private void FlushComponentAdd(EntityChange entityChange)
        {
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
            EntityComponentBits[id] |= AspectMapper.TypesToBigInteger(type);
            entityInterestedCache[id].Clear();
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            pendingEntityChanges.Enqueue(EntityChange.CreateComponentRemoved(entity, null, typeof(T)));
        }

        public void RemoveComponent(Entity entity, IComponent component)
        {
            pendingEntityChanges.Enqueue(EntityChange.CreateComponentRemoved(entity, component, null));
        }

        private void FlushComponentRemoval(EntityChange entityChange)
        {
            var id = entityChange.Entity.Id;
            var type = entityChange.ComponentType;
            components[type][id] = null;
            EntityComponentBits[id] &= ~AspectMapper.TypesToBigInteger(type);
            entityInterestedCache[id].Clear();
        }

        public IEnumerable<IComponent> GetComponents(Entity entity)
        {
            return components.Values
                .Select(arr => arr[entity.Id])
                .Where(c => c != null);
        }

        public T GetComponent<T>(Entity entity) where T : IComponent
        {
            if (!components.ContainsKey(typeof(T))) return default(T);

            return (T)components[typeof(T)][entity.Id];
        }

        public IComponent GetComponent(Entity entity, Type type)
        {
            if (!components.ContainsKey(type)) return null;

            return components[type][entity.Id];
        }

        public bool HasComponent<T>(Entity entity) where T : IComponent
        {
            if (!components.ContainsKey(typeof(T)) || entity.Id >= componentArraySize) return false;

            return components[typeof(T)][entity.Id] != null;
        }

        public bool HasComponent(Entity entity, Type type)
        {
            if (!components.ContainsKey(type) || entity.Id >= componentArraySize) return false;

            return components[type][entity.Id] != null;
        }

        public bool HasComponent(Entity entity, IComponent component)
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

            while (pendingEntityChanges.Any())
            {
                var entityChange = pendingEntityChanges.Dequeue();
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

            return entityChanges;
        }

        // TODO: Is this needed and is this right place for this method?
        public IEnumerable<Entity> FindEntities(Aspect aspect) => GetEntitiesForAspect(aspect);

        public Entity GetEntity(int id)
        {
            return entityCache[id];
        }

        public void Update(float deltaTime)
        {
            FlushChanges();

            foreach (var layerOrderSystems in GetSystemsInLayerOrder())
            {
                foreach (var system in layerOrderSystems)
                {
                    system.Update(deltaTime);
                    if (system.MissedUpdates)
                    {
                        system.MissedUpdates = false;
                    }
                }

                FlushChanges();
            }
        }

        public void UpdateSpecific(float deltaTime, int layer)
        {
            FlushChanges();

            foreach (var system in GetSystems(layer))
            {
                system.Update(deltaTime);
            }

            FlushChanges();
        }

        public void FlushChanges()
        {
            var entityChanges = FlushPending();
            FlushPendingChanges();

            foreach (var entityChange in entityChanges)
            {
                foreach (var layer in systems)
                {
                    foreach (var system in layer.Value)
                    {
                        // TODO: This looping and casting may be too inefficient
                        var entitySystem = system as EntitySystem;

                        entitySystem?.EntityChanged(entityChange);
                    }
                }
            }
        }

        public void AddSystem(System system, int layer = 0) =>
            pendingSystemChanges.GetOrAddNew(layer).Enqueue(SystemChange.CreateSystemAdded(system));

        public System AddSystem<T>(int layer = 0) where T : System, new()
        {
            var system = new T();
            pendingSystemChanges.GetOrAddNew(layer).Enqueue(SystemChange.CreateSystemAdded(system));
            return system;
        }

        public void RemoveSystem(System system, int layer) =>
            pendingSystemChanges.GetOrAddNew(layer).Enqueue(SystemChange.CreateSystemRemoved(system));

        private void FlushPendingChanges()
        {
            foreach (var pair in pendingSystemChanges)
            {
                var layer = pair.Key;
                var value = pair.Value;

                foreach (var change in value)
                {
                    var system = change.System;
                    if (change.TypeOfChange == SystemChange.ChangeType.Added)
                    {
                        system.Context = this;
                        systems.GetOrAddNew(layer).Add(system);
                        system.MissedUpdates = true;
                        system.SystemAddedInternal();
                    }
                    else
                    {
                        system.Context = null;
                        var systemsInLayer = systems[layer];
                        systemsInLayer.Remove(system);
                        system.SystemRemovedInternal();

                        if (systemsInLayer.Count <= 0)
                        {
                            systems.Remove(layer);
                        }
                    }
                }
            }

            pendingSystemChanges.Clear();
        }

        private IEnumerable<System> GetSystems(int layer)
        {
            if (!systems.TryGetValue(layer, out var ret))
            {
                return Enumerable.Empty<System>();
            }

            return ret;
        }

        private KeyValuePair<int, List<System>>? NextSystemsInclusive(int layer)
        {
            var first = systems.FirstOrDefault(pair => pair.Key >= layer);
            return first.Value == null ? null : new KeyValuePair<int, List<System>>?(first);
        }

        private IEnumerable<List<System>> GetSystemsInLayerOrder()
        {
            KeyValuePair<int, List<System>>? nextSystems;
            var nextKey = int.MinValue;
            do
            {
                nextSystems = NextSystemsInclusive(nextKey);
                if (nextSystems == null) continue;

                yield return nextSystems.Value.Value;

                if (nextKey == int.MaxValue)
                {
                    yield break;
                }
                nextKey = nextSystems.Value.Key + 1;
            } while (nextSystems != null);
        }
    }
}