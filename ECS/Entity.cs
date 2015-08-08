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
        public int Id { get; internal set; }

        internal IEntityUtility EntityUtility;

        public IEnumerable<IComponent> Components
        {
            get
            {
                return EntityUtility.GetComponents(this);
            }
        }

        internal Entity(int id, IEntityUtility entityUtility)
        {
            this.EntityUtility = entityUtility;
        }

        public void Remove()
        {
            EntityUtility.RemoveEntity(this);
        }

        public void AddComponent<T>(T component) where T : IComponent
        {
            EntityUtility.AddComponent(this, component);
        }

        public void RemoveComponent<T>(T component) where T: IComponent
        {
            EntityUtility.RemoveComponent(this, component);
        }

        public void RemoveComponent<T>() where T: IComponent
        {
            EntityUtility.RemoveComponent<T>(this);
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
            return Id.GetHashCode();
        }  
    }
}