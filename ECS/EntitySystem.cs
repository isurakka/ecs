using System;
using System.Collections.Generic;
using System.Linq;

namespace ECS
{
    public abstract class EntitySystem : System
    {
        protected readonly Aspect Aspect;

        protected HashSet<Entity> InterestedEntities;

        protected EntitySystem(Aspect aspect)
        {
            Aspect = aspect;
            InterestedEntities = new HashSet<Entity>();
        }

        /// <summary>
        /// Called before each update, before preprocessing. 
        /// Even if no entities get processed
        /// </summary>
        protected virtual void Begin() { }

        /// <summary>
        /// Called after each update
        /// </summary>
        protected virtual void End() { }

        internal void EntityChanged(EntityChange entityChange)
        {
            switch (entityChange.TypeOfChange)
            {
                case EntityChange.ChangeType.EntityAdded:
                    if (Aspect.Cache.Interested(Context.EntityComponentBits[entityChange.Entity.Id]))
                    {
                        InterestedEntities.Add(entityChange.Entity);
                        OnAdded(entityChange.Entity);
                    }
                    break;
                case EntityChange.ChangeType.EntityRemoved:
                    var removed = InterestedEntities.Remove(entityChange.Entity);
                    if (removed)
                    {
                        OnRemoved(entityChange.Entity);
                    }
                    break;
                case EntityChange.ChangeType.ComponentAdded:
                    if (!InterestedEntities.Contains(entityChange.Entity) && Aspect.Cache.Interested(Context.EntityComponentBits[entityChange.Entity.Id]))
                    {
                        InterestedEntities.Add(entityChange.Entity);
                        OnAdded(entityChange.Entity);
                    }
                    break;
                case EntityChange.ChangeType.ComponentRemoved:
                    if (InterestedEntities.Contains(entityChange.Entity) && !Aspect.Cache.Interested(Context.EntityComponentBits[entityChange.Entity.Id]))
                    {
                        InterestedEntities.Remove(entityChange.Entity);
                        OnRemoved(entityChange.Entity);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void OnAdded(Entity entity) { }

        protected virtual void OnRemoved(Entity entity) { }

        internal override void SystemAddedInternal()
        {
            InterestedEntities = new HashSet<Entity>(Context.FindEntities(Aspect));
            base.SystemAddedInternal();
        }

        internal override void SystemRemovedInternal()
        {
            foreach (var entity in InterestedEntities.ToList())
            {
                EntityChanged(EntityChange.CreateEntityRemoved(entity));
            }
            InterestedEntities = null;

            base.SystemRemovedInternal();
        }

        internal override void Update(float deltaTime)
        {
            // Start of actual processing
            // TODO: Should begin, end and processing happen if there are no entities to process? (probably not)
            Begin();

            ProcessEntities(InterestedEntities, deltaTime);

            End();
        }

        protected sealed override void Process(float deltaTime)
        {
        }

        protected abstract void ProcessEntities(IEnumerable<Entity> entities, float deltaTime);
    }
}
