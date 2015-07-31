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
        IEnumerable<IComponent> GetComponents(Entity entity);
        void RemoveEntity(Entity entity);
    }
}
