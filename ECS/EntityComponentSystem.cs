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
        private SystemManager systemManager;

        public EntityComponentSystem()
        {
            entityManager = new EntityManager();
            systemManager = new SystemManager();
        }

        public Entity CreateEntity()
        {
            return entityManager.CreateEntity();
        }

        public void AddComponent(Entity entity, IComponent component)
        {
            entityManager.AddComponent(entity, component);
        }

        public void AddSystem(System system, int priority = 0)
        {
            systemManager.AddSystem(system, priority);
        }

        public void Update(float deltaTime)
        {
            foreach (var systems in systemManager.SystemsByPriority())
            {
                foreach (var system in systems)
                {
                    system.processAll(entityManager.GetEntitiesForAspect(system.Aspect), deltaTime);
                }

                entityManager.ProcessQueues();
            }

            systemManager.ProcessQueues();
        }
    }
}
