using System;

namespace ECS
{
    public class ClosureEntityProcessingSystem : EntityProcessingSystem
    {
        public Action<Entity, float> ProcessorAction;
        public Action BeginAction;
        public Action EndAction;
        public Action<Entity> OnAddedAction;
        public Action<Entity> OnRemovedAction;

        public ClosureEntityProcessingSystem(Aspect aspect, Action<Entity, float> processor)
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

        protected override void Process(Entity entity, float deltaTime)
        {
            ProcessorAction?.Invoke(entity, deltaTime);
        }
    }
}
