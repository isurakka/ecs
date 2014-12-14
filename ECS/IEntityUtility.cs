using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    interface IEntityUtility
    {
        void AddComponent(Entity entity, IComponent component);
        void RemoveComponent(Entity entity, IComponent component);
        T GetComponent<T>(Entity entity) where T: IComponent;
    }
}
