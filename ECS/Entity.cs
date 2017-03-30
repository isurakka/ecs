using System;
using System.Collections.Generic;

namespace ECS
{
    public class Entity : IEquatable<Entity>
    {
        public int Id { get; }

        private readonly EntityComponentSystem ecs;

        internal Entity(int id, EntityComponentSystem ecs)
        {
            Id = id;
            this.ecs = ecs;
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        public void Remove() => ecs.RemoveEntity(this);

        /// <summary>
        /// Thread safe
        /// </summary>
        public void AddComponent<T>(T component)
            => ecs.AddComponent(this, component);

        /// <summary>
        /// Thread safe
        /// </summary>
        public T AddComponent<T>() where T : new() 
            => ecs.AddComponent<T>(this);

        /// <summary>
        /// Thread safe
        /// </summary>
        public void RemoveComponent<T>(T component)
            => ecs.RemoveComponent(this, component);

        /// <summary>
        /// Thread safe
        /// </summary>
        public void RemoveComponent<T>()
            => ecs.RemoveComponent<T>(this);

        /// <summary>
        /// Thread safe
        /// </summary>
        public bool HasComponent<T>()
            => ecs.HasComponent<T>(this);

        /// <summary>
        /// Thread safe
        /// </summary>
        public bool HasComponent(Type type)
            => ecs.HasComponent(this, type);

        /// <summary>
        /// Thread safe
        /// </summary>
        public bool HasComponent(object component)
            => ecs.HasComponent(this, component);

        /// <summary>
        /// Thread safe
        /// </summary>
        public T GetComponent<T>()
            => ecs.GetComponent<T>(this);

        /// <summary>
        /// Thread safe
        /// </summary>
        public object GetComponent(Type type)
            => ecs.GetComponent(this, type);

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

    public enum EntityExecution
    {
        Synchronous,
        Asynchronous
    }
}