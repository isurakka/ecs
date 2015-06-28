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
    public interface IEntityManager
    {
        Entity CreateEntity();
        void RemoveEntity(Entity entity);
        IEnumerable<Entity> GetEntities(Aspect aspect);
    }

    internal class EntityManager : IEntityManager, IEntityUtility
    {
        internal int nextEntityId = 0;
        internal BigInteger[] entityComponentCache;
        internal Dictionary<Type, IComponent[]> components;

        internal EntityManager()
        {
            
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(this);
            entity.Id = nextEntityId++;
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            //toRemoveEntity.Add(entity);
        }

        private void growBuffer<T>(ref T[] array)
        {
            Array.Resize(ref array, array.Length * 2);
        }

        internal IEnumerable<Entity> GetEntities(Aspect aspect)
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
