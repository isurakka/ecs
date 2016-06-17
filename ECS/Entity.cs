using System;
using System.Collections.Generic;

namespace ECS
{
    public class Entity : IEquatable<Entity>
    {
        public int Id { get; }

        private readonly IEntityUtility entityUtility;

        public IEnumerable<IComponent> Components => entityUtility.GetComponents(this);

        internal Entity(int id, IEntityUtility entityUtility)
        {
            Id = id;
            this.entityUtility = entityUtility;
        }

        public void Remove() => entityUtility.RemoveEntity(this);

        public void AddComponent<T>(T component) where T : IComponent
            => entityUtility.AddComponent(this, component);

        public T AddComponent<T>() where T : IComponent, new() 
            => entityUtility.AddComponent<T>(this);

        public void RemoveComponent<T>(T component) where T: IComponent
            => entityUtility.RemoveComponent(this, component);

        public void RemoveComponent<T>() where T: IComponent
            => entityUtility.RemoveComponent<T>(this);

        public bool HasComponent<T>() where T: IComponent
            => entityUtility.HasComponent<T>(this);

        public bool HasComponent(IComponent component)
            => entityUtility.HasComponent(this, component);

        public T GetComponent<T>() where T : IComponent 
            => entityUtility.GetComponent<T>(this);

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