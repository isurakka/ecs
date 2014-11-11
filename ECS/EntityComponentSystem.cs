using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class EntityComponentSystem
    {
        private EntityManager entityManager;

        public EntityComponentSystem()
        {
            entityManager = new EntityManager();
        }

        public Entity CreateEntity()
        {
            return entityManager.CreateEntity();
        }

        public void AddComponent(Entity entity, IComponent component)
        {

        }
    }
}
