using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    internal class EntityManager
    {
        private uint nextId = 1;

        internal Dictionary<Entity, EntityData> entities = new Dictionary<Entity, EntityData>();

        private ConcurrentQueue<Entity> toAddEntity = new ConcurrentQueue<Entity>();
        private ConcurrentQueue<Entity> toRemoveEntity = new ConcurrentQueue<Entity>();
        private ConcurrentQueue<Tuple<Entity, IComponent>> toAddComponent = new ConcurrentQueue<Tuple<Entity, IComponent>>();
        private ConcurrentQueue<Tuple<Entity, IComponent>> toRemoveComponent = new ConcurrentQueue<Tuple<Entity, IComponent>>();

        internal EntityManager()
        {

        }

        internal Entity CreateEntity()
        {
            var entity = new Entity(this);
            toAddEntity.Enqueue(entity);
            return entity;
        }

        internal void RemoveEntity(Entity entity)
        {
            toRemoveEntity.Enqueue(entity);
        }

        internal void AddComponent(Entity entity, IComponent component)
        {
            toAddComponent.Enqueue(new Tuple<Entity, IComponent>(entity, component));
        }

        internal void RemoveComponent(Entity entity, IComponent component)
        {
            toRemoveComponent.Enqueue(new Tuple<Entity, IComponent>(entity, component));
        }

        internal void ProcessQueues()
        {
            while (toRemoveEntity.Count > 0)
            {
                Entity entity;
                toRemoveEntity.TryDequeue(out entity);

                entities.Remove(entity);
            }

            while (toAddEntity.Count > 0)
            {
                Entity entity;
                toAddEntity.TryDequeue(out entity);
                entity.id = nextId++;

                entities.Add(entity, new EntityData());
            }

            while (toRemoveComponent.Count > 0)
            {
                Tuple<Entity, IComponent> tuple;
                toRemoveComponent.TryDequeue(out tuple);
                var entity = tuple.Item1;
                var component = tuple.Item2;
                var type = component.GetType();

                var data = entities[entity];

                if (!data.Types.Contains(type))
                {
                    if (toAddComponent.Any(t => type == t.Item2.GetType()))
                    {
                        throw new ArgumentException("Can't add and remove component of same type in the same update");
                    }

                    throw new ArgumentException("Entity doesn't contain a component of specified type");
                }

                data.Components.Remove(component);
                data.Types.Remove(type);
            }

            while (toAddComponent.Count > 0)
            {
                Tuple<Entity, IComponent> tuple;
                toAddComponent.TryDequeue(out tuple);
                var entity = tuple.Item1;
                var component = tuple.Item2;
                var type = component.GetType();

                var data = entities[entity];

                if (data.Types.Contains(type))
                {
                    throw new ArgumentException("Entity already contains a component of specified type");
                }

                data.Components.Add(component);
                data.Types.Add(type);
            }
        }
    }
}
