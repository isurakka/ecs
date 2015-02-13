using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Entity : IEquatable<Entity>
    {
        //internal HashSet<IComponent> ComponentSet = new HashSet<IComponent>(ComponentEqualityComparer.Instance);
        internal ConcurrentDictionary<Type, IComponent> ComponentSet = new ConcurrentDictionary<Type, IComponent>();
        //internal HashSet<Type> Types = new HashSet<Type>();
        internal IEntityUtility EntityUtility;

        private long id = long.MinValue;
        public long Id
        {
            internal set
            {
                id = value;
            }
            get
            {
                return id;
            }
        }

        public IEnumerable<IComponent> Components
        {
            get
            {
                return ComponentSet.Values.AsEnumerable();
            }
        }

        internal Entity(IEntityUtility entityUtility)
        {
            this.EntityUtility = entityUtility;
        }

        public void Remove()
        {
            EntityUtility.RemoveEntity(this);
        }

        public void AddComponent<T>(T component) where T : IComponent
        {
            var success = ComponentSet.TryAdd(component.GetType(), component);
            if (!success)
            {
                throw new InvalidOperationException("Entity already contains a component of the specified type");
            }
        }

        public void RemoveComponent<T>(T component) where T: IComponent
        {
            IComponent removed;
            var success = ComponentSet.TryRemove(component.GetType(), out removed);
            if (!success)
            {
                throw new InvalidOperationException("Could not remove the specified component " + component +
                        " because the entity doesn't have it.");
            }
        }

        public void RemoveComponent<T>() where T: IComponent
        {
            IComponent removed;
            var success = ComponentSet.TryRemove(typeof(T), out removed);
            if (!success)
            {
                throw new InvalidOperationException("Could not remove the component of specified type " + typeof(T) +
                        " because the entity doesn't have it.");
            }
        }

        public bool HasComponent<T>() where T: IComponent
        {
            return ComponentSet.ContainsKey(typeof(T));
        }

        public T GetComponent<T>() where T: IComponent
        {
            if (ComponentSet.ContainsKey(typeof(T)))
            {
                return (T)ComponentSet[typeof(T)];
            }

            return default(T);
        }

        public bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other) || !ReferenceEquals(this.EntityUtility, other.EntityUtility))
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Id.GetHashCode();
            }
        }  
    }
}