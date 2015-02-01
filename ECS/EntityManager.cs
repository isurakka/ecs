using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECS
{
    internal class EntityManager : IEntityUtility
    {
        private long currentEntityId = long.MinValue;

        internal HashSet<Entity> entities = new HashSet<Entity>();

        private ConcurrentBag<Entity> toAddEntity = new ConcurrentBag<Entity>();
        private ConcurrentBag<Entity> toRemoveEntity = new ConcurrentBag<Entity>();
        private ConcurrentBag<Tuple<Entity, IComponent>> toAddComponent = new ConcurrentBag<Tuple<Entity, IComponent>>();
        private ConcurrentBag<Tuple<Entity, IComponent>> toRemoveComponent = new ConcurrentBag<Tuple<Entity, IComponent>>();

        internal EntityManager()
        {
            
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(this);
            toAddEntity.Add(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            toRemoveEntity.Add(entity);
        }

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            toAddComponent.Add(Tuple.Create<Entity, IComponent>(entity, (IComponent)component));
        }

        public void RemoveComponent<T>(Entity entity, T component) where T : IComponent
        {
            toRemoveComponent.Add(new Tuple<Entity, IComponent>(entity, component));
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            toRemoveComponent.Add(new Tuple<Entity, IComponent>(entity, entity.Components.First(c => c.GetType() == typeof(T))));
        }

        public bool HasComponent<T>(Entity entity) where T : IComponent
        {
            return entity.Components.Any(c => c.GetType() == typeof(T));
        }

        public T GetComponent<T>(Entity entity) where T: IComponent
        {
            var component = entity.Components.FirstOrDefault(c => c.GetType() == typeof(T));
            
            if (component == null)
            {
                return default(T);
            }

            return (T)component;
        }

        internal IEnumerable<Entity> GetEntitiesForAspect(Aspect aspect)
        {
            foreach (var entity in entities)
            {
                if (aspect.Interested(entity.Components.Select(c => c.GetType())))
                {
                    yield return entity;
                }
            }
        }

        internal void ProcessQueues()
        {
            Entity entity;

            while (toRemoveEntity.TryTake(out entity))
            {
                if (!entities.Remove(entity))
                {
                    throw new InvalidOperationException("No such is added so it can't be removed.");
                }
            }

            while (toAddEntity.TryTake(out entity))
            {
                entity.Id = Interlocked.Increment(ref currentEntityId);
                if (!entities.Add(entity))
                {
                    throw new InvalidOperationException("Could not add the specified entity because it is already added.");
                }
            }

            Tuple<Entity, IComponent> tuple;

            while (toRemoveComponent.TryTake(out tuple))
            {
                entity = tuple.Item1;
                var component = tuple.Item2;
                if (!entity.Components.Remove(component))
                {
                    throw new InvalidOperationException("Could not remove the specified component " + component +
                        " because the entity doesn't have it.");
                }
            }

            while (toAddComponent.TryTake(out tuple))
            {
                entity = tuple.Item1;
                var component = tuple.Item2;
                var type = component.GetType();

                if (entity.Components.Any(c => c.GetType() == type))
                {
                    throw new InvalidOperationException("Entity already contains a component of the specified type");
                }

                entity.Components.Add(component);
            }
        }
    }
}
