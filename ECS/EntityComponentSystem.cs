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

        public void AddSystem(System system, int layer = 0)
        {
            systemManager.AddSystem(system, layer);
        }

        public void RemoveSystem(System system, int layer)
        {
            systemManager.RemoveSystem(system, layer);
        }

        // TODO: Is this needed and is this right place for this method?
        public IEnumerable<Entity> FindEntities(Aspect aspect)
        {
            return entityManager.GetEntitiesForAspect(aspect);
        }

        public void Update(float deltaTime)
        {
            systemManager.FlushPendingChanges();

            bool anySystems = false;

            foreach (var systems in systemManager.GetSystemsInLayerOrder())
            {
                anySystems |= true;

                // Add and remove entities before each system layer
                entityManager.FlushPending();

                foreach (var system in systems)
                {
                    system.Update(deltaTime);
                }

                systemManager.FlushPendingChanges();
            }

            // If there are no systems, still add and remove entities
            if (!anySystems)
            {
                entityManager.FlushPending();
            }
        }
    }
}
