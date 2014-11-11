using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class EntityManager
    {
        private uint nextId = 1;

        private Dictionary<Entity, EntityData> entities = new Dictionary<Entity, EntityData>();

        private Queue<Entity> toAddEntity = new Queue<Entity>();
        private Queue<Tuple<Entity, IComponent>> toAddComponent = new Queue<Tuple<Entity, IComponent>>();
        private Queue<Tuple<Entity, IComponent>> toRemoveComponent = new Queue<Tuple<Entity, IComponent>>();

        internal EntityManager()
        {

        }

        public Entity CreateEntity()
        {
            var entity = new Entity();
            toAddEntity.Enqueue(entity);
            return entity;
        }

        public void AddComponent(Entity entity, IComponent component)
        {
            toAddComponent.Enqueue(new Tuple<Entity, IComponent>(entity, component));
        }

        public void RemoveComponent(Entity entity, IComponent component)
        {
            toRemoveComponent.Enqueue(new Tuple<Entity, IComponent>(entity, component));
        }

        private void ProcessQueues()
        {
            while (toAddEntity.Count > 0)
            {
                var entity = toAddEntity.Dequeue();
                entity.id = nextId++;

                entities.Add(entity, new EntityData());
            }

            while (toRemoveComponent.Count > 0)
            {
                var tuple = toRemoveComponent.Dequeue();
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
                var tuple = toAddComponent.Dequeue();
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
