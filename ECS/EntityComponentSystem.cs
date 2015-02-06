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

        public void SetSystem(System system, int priority = 0)
        {
            systemManager.SetSystem(system, priority);
        }

        // TODO: Is this needed and is this right place for this method?
        public IEnumerable<Entity> FindEntities(Aspect aspect)
        {
            return entityManager.GetEntitiesForAspect(aspect);
        }

        public void Update(float deltaTime)
        {
            // Add and remove systems before each update
            systemManager.ProcessQueues();

            bool anySystems = false;

            foreach (var systems in systemManager.SystemsByPriority())
            {
                anySystems |= true;

                // Add and remove entities before each system priority
                entityManager.ProcessQueues();

                foreach (var system in systems)
                {
                    var entities = entityManager.GetEntitiesForAspect(system.Aspect);
                    system.Update(entities, deltaTime);
                }
            }

            // If there are no systems, still add and remove them
            if (!anySystems)
            {
                entityManager.ProcessQueues();
            }
        }
    }
}
