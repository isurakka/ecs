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
        private int nextEntityId = 0;
        private int nextResizeId = 0;

        private Dictionary<Type, IComponent[]> components;

        internal EntityManager()
        {
            components = new Dictionary<Type, IComponent[]>();
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
            throw new NotImplementedException();
        }

        public void RemoveComponent<T>(Entity entity, T component) where T : IComponent
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IComponent> GetComponents(Entity entity)
        {
            return components.Values
                .Select(arr => arr[entity.Id])
                .Where(c => c != null);
        }

        internal IEnumerable<Entity> GetEntitiesForAspect(Aspect aspect)
        {
            foreach (var pair in entities)
            {
                var entity = pair.Value;
                if (aspect.Interested(entity.ComponentSet.Keys))
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
                if (!entities.Remove(entity.Id))
                {
                    throw new InvalidOperationException("No such is added so it can't be removed.");
                }
            }

            while (toAddEntity.TryTake(out entity))
            {
                entities.Add(entity.Id, entity);
                //throw new InvalidOperationException("Could not add the specified entity because it is already added.");
            }
        }
    }
}
