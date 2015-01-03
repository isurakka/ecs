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

        internal List<IComponent> Components = new List<IComponent>();
        //internal HashSet<Type> Types = new HashSet<Type>();
        internal IEntityUtility EntityUtility;

        internal Entity(IEntityUtility entityUtility)
        {
            this.EntityUtility = entityUtility;
        }

        public void AddComponent<T>(T component) where T : IComponent
        {
            EntityUtility.AddComponent(this, component);
        }

        public void RemoveComponent<T>(T component) where T: IComponent
        {
            EntityUtility.RemoveComponent(this, component);
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