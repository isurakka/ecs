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
        protected readonly HashSet<Entity> interestedEntities;

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

        internal override void Update(float deltaTime)
        {
            if (MissedUpdates)
            {
                interestedEntities.Clear();
                foreach (var entity in Context.FindEntities(Aspect))
                {
                    interestedEntities.Add(entity);
                }
            }
            else
            {
                foreach (var entityChange in EntityComponentSystem.Instance.entityManager.changesInLastFlush)
                {
                    switch (entityChange.TypeOfChange)
                    {
                        case EntityChange.ChangeType.EntityAdded:
                        case EntityChange.ChangeType.ComponentAdded:
                        {
                            var entityComponentBits = EntityComponentSystem.Instance.entityManager.entityComponentBits[entityChange.Entity.Id];
                            if (entityComponentBits.HasValue && Aspect.Cache.Interested(entityComponentBits.Value))
                            {
                                interestedEntities.Add(entityChange.Entity);
                            }
                        }
                            break;
                        case EntityChange.ChangeType.EntityRemoved:
                            interestedEntities.Remove(entityChange.Entity);
                            break;
                        default:
                        {
                            var entityComponentBits = EntityComponentSystem.Instance.entityManager.entityComponentBits[entityChange.Entity.Id];
                            if (!entityComponentBits.HasValue || Aspect.Cache.Interested(entityComponentBits.Value))
                            {
                                interestedEntities.Remove(entityChange.Entity);
                            }
                        }
                            break;
                    }
                }
            }

            // Start of actual processing
            // TODO: Should begin, end and processing happen if there are no entities to process? (probably not)
            Begin();

            var preprocessed = PreprocessEntities(interestedEntities);
            ProcessEntities(preprocessed, deltaTime);

            End();
        }

        protected sealed override void Process(float deltaTime)
        {
        }

        protected abstract void ProcessEntities(IEnumerable<Entity> entities, float deltaTime);

        protected virtual IEnumerable<Entity> PreprocessEntities(IEnumerable<Entity> entities)
        {
            return entities;
        }
    }
}
