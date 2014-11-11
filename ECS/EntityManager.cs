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

        private Queue<Tuple<Entity, IComponent>> toAdd = new Queue<Tuple<Entity, IComponent>>();
        private Queue<Tuple<Entity, IComponent>> toRemove = new Queue<Tuple<Entity, IComponent>>();

        public Entity CreateEntity()
        {
            var entity = new Entity(nextId++);
            entities.Add(entity, new EntityData());
            return entity;
        }

        public void AddComponent(Entity entity, IComponent component)
        {
            toAdd.Enqueue(new Tuple<Entity, IComponent>(entity, component));
        }

        public void RemoveComponent(Entity entity, IComponent component)
        {
            toRemove.Enqueue(new Tuple<Entity, IComponent>(entity, component));
        }

        private void ProcessQueues()
        {
            while (toRemove.Count > 0)
            {
                var tuple = toRemove.Dequeue();
                var entity = tuple.Item1;
                var component = tuple.Item2;
                var type = component.GetType();

                var data = entities[entity];

                if (!data.Types.Contains(type))
                {
                    throw new ArgumentException("Entity doesn't contain a component of specified type");
                }

                data.Components.Remove(component);
                data.Types.Remove(type);
            }

            while (toAdd.Count > 0)
            {
                var tuple = toAdd.Dequeue();
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
