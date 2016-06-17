using System.Collections.Generic;

namespace ECS
{
    interface IEntityUtility
    {
        void AddComponent<T>(Entity entity, T component) where T : IComponent;
        T AddComponent<T>(Entity entity) where T : IComponent, new();
        void RemoveComponent<T>(Entity entity) where T : IComponent;
        void RemoveComponent(Entity entity, IComponent component);
        IEnumerable<IComponent> GetComponents(Entity entity);
        T GetComponent<T>(Entity entity) where T : IComponent;
        bool HasComponent<T>(Entity entity) where T : IComponent;
        bool HasComponent(Entity entity, IComponent component);
        void RemoveEntity(Entity entity);
    }
}
