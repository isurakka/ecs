using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class EntitySystem : System
    {
        internal readonly Aspect Aspect;

        protected HashSet<Entity> interestedEntities;

        protected EntitySystem(Aspect aspect)
        {
            Aspect = aspect;
            interestedEntities = new HashSet<Entity>();
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
                        interestedEntities.Add(entityChange.Entity);
                        OnAdded(entityChange.Entity);
                    }
                    break;
                case EntityChange.ChangeType.EntityRemoved:
                    var removed = interestedEntities.Remove(entityChange.Entity);
                    if (removed)
                    {
                        OnRemoved(entityChange.Entity);
                    }
                    break;
                case EntityChange.ChangeType.ComponentAdded:
                    if (!interestedEntities.Contains(entityChange.Entity) && Aspect.Cache.Interested(Context.EntityComponentBits[entityChange.Entity.Id]))
                    {
                        interestedEntities.Add(entityChange.Entity);
                        OnAdded(entityChange.Entity);
                    }
                    break;
                case EntityChange.ChangeType.ComponentRemoved:
                    if (interestedEntities.Contains(entityChange.Entity) && !Aspect.Cache.Interested(Context.EntityComponentBits[entityChange.Entity.Id]))
                    {
                        interestedEntities.Remove(entityChange.Entity);
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
            interestedEntities = new HashSet<Entity>(Context.FindEntities(Aspect));
            base.SystemAddedInternal();
        }

        internal override void SystemRemovedInternal()
        {
            foreach (var entity in interestedEntities.ToList())
            {
                EntityChanged(EntityChange.CreateEntityRemoved(entity));
            }
            interestedEntities = null;

            base.SystemRemovedInternal();
        }

        internal override void Update(float deltaTime)
        {
            // Start of actual processing
            // TODO: Should begin, end and processing happen if there are no entities to process? (probably not)
            Begin();

            ProcessEntities(interestedEntities, deltaTime);

            End();
        }

        protected sealed override void Process(float deltaTime)
        {
        }

        protected abstract void ProcessEntities(IEnumerable<Entity> entities, float deltaTime);
    }
}
