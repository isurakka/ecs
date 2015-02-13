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

        //internal HashSet<Entity> entities = new HashSet<Entity>();
        internal Dictionary<long, Entity> entities = new Dictionary<long, Entity>();

        private ConcurrentBag<Entity> toAddEntity = new ConcurrentBag<Entity>();
        private ConcurrentBag<Entity> toRemoveEntity = new ConcurrentBag<Entity>();

        internal EntityManager()
        {
            
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(this);
            entity.Id = Interlocked.Increment(ref currentEntityId);
            toAddEntity.Add(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            toRemoveEntity.Add(entity);
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
