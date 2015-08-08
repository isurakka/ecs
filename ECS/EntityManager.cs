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
    internal class EntityManager : IEntityUtility
    {
        private int nextEntityId = 0;
        private int componentArraySize = 0;

        private ComponentTypesToBigIntegerMapper mapper;

        private Dictionary<Type, IComponent[]> components;
        private BigInteger[] entityComponentBits;

        private HashSet<int> entitiesToBeRemoved;

        internal EntityManager()
        {
            mapper = new ComponentTypesToBigIntegerMapper();
            components = new Dictionary<Type, IComponent[]>();
            entityComponentBits = new BigInteger[0];
            entitiesToBeRemoved = new HashSet<int>();
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(nextEntityId++, this);

            if (componentArraySize < nextEntityId)
            {
                componentArraySize = componentArraySize > 0 ? componentArraySize * 2 : 1;
                foreach (var key in components.Keys)
                {
                    var value = components[key];
                    Array.Resize(ref value, componentArraySize);
                }

                Array.Resize(ref entityComponentBits, componentArraySize);
            }

            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            entitiesToBeRemoved.Add(entity.Id);
        }

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            if (!components.ContainsKey(typeof(T)))
            {
                components.Add(typeof(T), new IComponent[componentArraySize]);
            }
            // only check this if this wasn't first component of this type to be added
            else if (components[typeof(T)][entity.Id] != null)
            {
                throw new InvalidOperationException(
                    $@"Entity of type {typeof(T).Name} is already 
added to entity with {nameof(entity.Id)} {entity.Id}");
            }

            components[typeof(T)][entity.Id] = component;
            entityComponentBits[entity.Id] &= mapper.TypesToBigInteger(typeof(T));
        }


        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            components[typeof(T)][entity.Id] = null;
            entityComponentBits[entity.Id] &= ~mapper.TypesToBigInteger(typeof(T));
        }

        public void RemoveComponent(Entity entity, IComponent component)
        {
            components[component.GetType()][entity.Id] = null;
            entityComponentBits[entity.Id] &= ~mapper.TypesToBigInteger(component.GetType());
        }

        public IEnumerable<IComponent> GetComponents(Entity entity)
        {
            return components.Values
                .Select(arr => arr[entity.Id])
                .Where(c => c != null);
        }

        internal IEnumerable<Entity> GetEntitiesForAspect(Aspect aspect)
        {
            for (int i = 0; i < componentArraySize; i++)
            {
                if (entityComponentBits[i] != BigInteger.Zero && 
                    aspect.Interested(entityComponentBits[i]))
                {
                    yield return new Entity(i, this);
                }
            }
        }

        internal void FlushPending()
        {
            throw new NotImplementedException();
        }
    }
}
