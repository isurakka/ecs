using System;
using System.Collections.Generic;

namespace ECS
{
    public class ClosureEntitySystem : EntitySystem
    {
        public Action<IEnumerable<Entity>, float> ProcessorAction;
        public Action BeginAction;
        public Action EndAction;
        public Action<Entity> OnAddedAction;
        public Action<Entity> OnRemovedAction;

        public ClosureEntitySystem(Aspect aspect, Action<IEnumerable<Entity>, float> processor = null)
            : base(aspect)
        {
            ProcessorAction = processor;
        }

        protected override void Begin()
        {
            BeginAction?.Invoke();
        }

        protected override void End()
        {
            EndAction?.Invoke();
        }

        protected override void OnAdded(Entity entity)
        {
            OnAddedAction?.Invoke(entity);
        }

        protected override void OnRemoved(Entity entity)
        {
            OnRemovedAction?.Invoke(entity);
        }

        protected override void ProcessEntities(IEnumerable<Entity> entities, float deltaTime)
        {
            ProcessorAction?.Invoke(entities, deltaTime);
        }
    }
}
