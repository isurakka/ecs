using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    interface IEntityUtility
    {
        void AddComponent<T>(Entity entity, T component) where T : IComponent;
        void RemoveComponent<T>(Entity entity, T component) where T : IComponent;
        void RemoveComponent<T>(Entity entity) where T : IComponent;
        bool HasComponent<T>(Entity entity) where T : IComponent;
        T GetComponent<T>(Entity entity) where T: IComponent;
        void RemoveEntity(Entity entity);
    }
}
