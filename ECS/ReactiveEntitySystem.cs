using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class ReactiveEntitySystem : EntitySystem
    {
        protected ReactiveEntitySystem(Aspect aspect)
            : base(aspect)
        {
            
        }

        protected ReactiveEntitySystem(params Type[] types)
            : base(Aspect.All(types))
        {

        }

        internal override void SystemRemovedInternal()
        {
            foreach (var entity in interestedEntities)
            {
                OnRemoved(entity);
            }
            interestedEntities.Clear();

            base.SystemRemovedInternal();
        }

        protected virtual void OnAdded(Entity entity) { }

        protected virtual void OnRemoved(Entity entity) { }

        internal sealed override void Update(float deltaTime)
        {
            if (MissedUpdates)
            {
                Debug.Assert(interestedEntities.Count == 0);
                interestedEntities.Clear();
                foreach (var entity in Context.FindEntities(Aspect))
                {
                    interestedEntities.Add(entity);
                    OnAdded(entity);
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
                                var entityComponentBits = EntityComponentSystem.Instance.entityManager.entityComponentBits[entityChange.Entity.Id].Value;
                                if (Aspect.Cache.Interested(entityComponentBits))
                                {
                                    if (interestedEntities.Add(entityChange.Entity))
                                    {
                                        OnAdded(entityChange.Entity);
                                    }
                                }
                            }
                            break;
                        case EntityChange.ChangeType.EntityRemoved:
                            if (interestedEntities.Remove(entityChange.Entity))
                            {
                                OnRemoved(entityChange.Entity);
                            }
                            break;
                        default:
                            {
                                var entityComponentBits = EntityComponentSystem.Instance.entityManager.entityComponentBits[entityChange.Entity.Id].Value;
                                if (Aspect.Cache.Interested(entityComponentBits))
                                {
                                    if (interestedEntities.Remove(entityChange.Entity))
                                    {
                                        OnRemoved(entityChange.Entity);
                                    }
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
    }
}
