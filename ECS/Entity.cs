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
        public int Id { get; }

        internal IEntityUtility EntityUtility;

        public IEnumerable<IComponent> Components => EntityUtility.GetComponents(this);

        internal Entity(int id, IEntityUtility entityUtility)
        {
            Id = id;
            EntityUtility = entityUtility;
        }

        public void Remove() => EntityUtility.RemoveEntity(this);

        public void AddComponent<T>(T component) where T : IComponent
            => EntityUtility.AddComponent(this, component);

        public void RemoveComponent<T>(T component) where T: IComponent
            => EntityUtility.RemoveComponent(this, component);

        public void RemoveComponent<T>() where T: IComponent
            => EntityUtility.RemoveComponent<T>(this);

        public bool HasComponent<T>() where T: IComponent
            => EntityUtility.HasComponent<T>(this);

        public bool HasComponent(IComponent component)
            => EntityUtility.HasComponent(this, component);

        public T GetComponent<T>() where T : IComponent 
            => EntityUtility.GetComponent<T>(this);

        public bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj) => Equals(obj as Entity);

        public static bool operator ==(Entity e1, Entity e2)
        {
            if (ReferenceEquals(e1, e2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)e1 == null) || ((object)e2 == null))
            {
                return false;
            }

            return e1.Equals(e2);
        }

        public static bool operator !=(Entity e1, Entity e2)
        {
            return !(e1 == e2);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}