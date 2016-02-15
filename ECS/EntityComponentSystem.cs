﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class EntityComponentSystem
    {
        internal readonly EntityManager entityManager;
        private readonly SystemManager systemManager;

        private static readonly bool running64Bit;

        private static EntityComponentSystem instance;
        /// <summary>
        /// Get the singleton instance. This will probably not be singleton in the future.
        /// </summary>
        public static EntityComponentSystem Instance 
            => instance ?? (instance = new EntityComponentSystem());

        static EntityComponentSystem()
        {
            running64Bit = Environment.Is64BitProcess;
        }

        private EntityComponentSystem()
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
            return entityManager.entityCache[id];
        }

        public void Update(float deltaTime)
        {
            entityManager.Updating = true;
            FlushChanges();

            // 256 MB
            var noGc = GC.TryStartNoGCRegion(running64Bit ? 256000000L : 16000000L);

            foreach (var systems in systemManager.GetSystemsInLayerOrder())
            {
                foreach (var system in systems)
                {
                    system.Update(deltaTime);
                    if (system.MissedUpdates)
                    {
                        system.MissedUpdates = false;
                    }
                }

                FlushChanges();
            }

            if (noGc) GC.EndNoGCRegion();
            entityManager.Updating = false;
        }

        public void UpdateSpecific(float deltaTime, int layer)
        {
            entityManager.Updating = true;
            FlushChanges();

            foreach (var system in systemManager.GetSystems(layer))
            {
                system.Update(deltaTime);
            }

            FlushChanges();
            entityManager.Updating = false;
        }

        public void FlushChanges()
        {
            entityManager.FlushPending();
            systemManager.FlushPendingChanges();
        }
    }
}
