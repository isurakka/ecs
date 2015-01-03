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
            systemManager.context = this;
        }

        public Entity CreateEntity()
        {
            return entityManager.CreateEntity();
        }

        public void AddSystem(System system, int priority = 0)
        {
            systemManager.SetSystem(system, priority);
        }

        // TODO: Is this needed and is this right place for this method?
        public IEnumerable<Entity> QueryActiveEntities(Aspect aspect)
        {
            return entityManager.GetEntitiesForAspect(aspect);
        }

        public void Update(float deltaTime)
        {
            foreach (var systems in systemManager.SystemsByPriority())
            {
                entityManager.ProcessQueues();

                foreach (var system in systems)
                {
                    system.processAll(entityManager.GetEntitiesForAspect(system.Aspect), deltaTime);
                }
            }

            systemManager.ProcessQueues();
        }
    }
}
