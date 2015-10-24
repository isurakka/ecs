using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class EntityComponentSystem
    {
        private readonly EntityManager entityManager;
        private readonly SystemManager systemManager;

        private static readonly bool running64Bit;

        static EntityComponentSystem()
        {
            running64Bit = Environment.Is64BitProcess;
        }

        public EntityComponentSystem()
        {
            entityManager = new EntityManager();
            systemManager = new SystemManager { context = this };
        }

        public Entity CreateEntity() => entityManager.CreateEntity();

        public void AddSystem(System system, int layer = 0) => 
            systemManager.AddSystem(system, layer);

        public void RemoveSystem(System system, int layer) => 
            systemManager.RemoveSystem(system, layer);

        // TODO: Is this needed and is this right place for this method?
        public IEnumerable<Entity> FindEntities(Aspect aspect) => 
            entityManager.GetEntitiesForAspect(aspect);

        public Entity GetEntity(int id)
        {
            return new Entity(id, entityManager);
        }

        public void Update(float deltaTime)
        {
            FlushChanges();

            // 256 MB
            var noGc = GC.TryStartNoGCRegion(running64Bit ? 256000000L : 16000000L);

            foreach (var systems in systemManager.GetSystemsInLayerOrder())
            {
                foreach (var system in systems)
                {
                    system.Update(deltaTime);
                }

                FlushChanges();
            }

            if (noGc) GC.EndNoGCRegion();
        }

        public void UpdateSpecific(float deltaTime, int layer)
        {
            FlushChanges();

            foreach (var system in systemManager.GetSystems(layer))
            {
                system.Update(deltaTime);
            }

            FlushChanges();
        }

        public void FlushChanges()
        {
            entityManager.FlushPending();
            systemManager.FlushPendingChanges();
        }
    }
}
