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
        private int currentEntityId = int.MinValue;

        internal HashSet<Entity> entities = new HashSet<Entity>();

        private ConcurrentQueue<Entity> toAddEntity = new ConcurrentQueue<Entity>();
        private ConcurrentQueue<Entity> toRemoveEntity = new ConcurrentQueue<Entity>();
        private ConcurrentQueue<Tuple<Entity, IComponent>> toAddComponent = new ConcurrentQueue<Tuple<Entity, IComponent>>();
        private ConcurrentQueue<Tuple<Entity, IComponent>> toRemoveComponent = new ConcurrentQueue<Tuple<Entity, IComponent>>();

        internal EntityManager()
        {

        }

        public Entity CreateEntity()
        {
            var entity = new Entity(this);
            toAddEntity.Enqueue(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            toRemoveEntity.Enqueue(entity);
        }

        public void AddComponent(Entity entity, IComponent component)
        {
            toAddComponent.Enqueue(new Tuple<Entity, IComponent>(entity, component));
        }

        public void RemoveComponent(Entity entity, IComponent component)
        {
            toRemoveComponent.Enqueue(new Tuple<Entity, IComponent>(entity, component));
        }

        public T GetComponent<T>(Entity entity) where T: IComponent
        {
            throw new NotImplementedException();
        }

        internal void ProcessQueues()
        {
            Entity entity;
            bool success;

            while (toRemoveEntity.Count > 0)
            {
                success = toRemoveEntity.TryDequeue(out entity);
                if (!success)
                {
                    throw new InvalidOperationException("Could not remove an entity");
                }

                entities.Remove(entity);
            }

            while (toAddEntity.Count > 0)
            {
                success = toAddEntity.TryDequeue(out entity);
                if (!success)
                {
                    throw new InvalidOperationException("Could not add an entity");
                }
                entity.Id = Interlocked.Increment(ref currentEntityId);

                entities.Add(entity);
            }

            Tuple<Entity, IComponent> tuple;

            while (toRemoveComponent.Count > 0)
            {
                success = toRemoveComponent.TryDequeue(out tuple);
                if (!success)
                {
                    throw new InvalidOperationException("Could not remove a component");
                }

                entity = tuple.Item1;
                var component = tuple.Item2;
                var type = component.GetType();

                entity.Components.Remove(component);
            }

            while (toAddComponent.Count > 0)
            {
                success = toAddComponent.TryDequeue(out tuple);
                if (!success)
                {
                    throw new InvalidOperationException("Could not add a component");
                }

                entity = tuple.Item1;
                var component = tuple.Item2;
                var type = component.GetType();

                if (entity.Components.Any(c => c.GetType() == type))
                {
                    throw new ArgumentException("Entity already contains a component of the specified type");
                }

                entity.Components.Add(component);
            }
        }
    }
}
