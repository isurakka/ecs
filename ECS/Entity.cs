using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Entity : IEquatable<Entity>
    {
        internal long Id = long.MinValue;

        internal HashSet<IComponent> ComponentSet = new HashSet<IComponent>(ComponentEqualityComparer.Instance);
        //internal HashSet<Type> Types = new HashSet<Type>();
        internal IEntityUtility EntityUtility;

        public IEnumerable<IComponent> Components
        {
            get
            {
                return ComponentSet.AsEnumerable();
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
            return EntityUtility.HasComponent<T>(this);
        }

        public T GetComponent<T>()
            where T: IComponent
        {
            return EntityUtility.GetComponent<T>(this);
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