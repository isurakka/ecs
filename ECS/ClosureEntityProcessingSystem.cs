using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class ClosureEntityProcessingSystem : EntityProcessingSystem
    {
        public Action<Entity, float> ProcessorAction;
        public Action BeginAction;
        public Action EndAction;
        public Action<Entity> OnAddedAction;
        public Action<Entity> OnRemovedAction;
        public Func<IEnumerable<Entity>, IEnumerable<Entity>> PreprocessEntitiesAction;

        public ClosureEntityProcessingSystem(Aspect aspect, Action<Entity, float> processor)
            : base(aspect)
        {
            this.ProcessorAction = processor;
        }

        protected override void Begin()
        {
            if (BeginAction == null)
            {
                return;
            }

            BeginAction();
        }

        protected override void End()
        {
            if (EndAction == null)
            {
                return;
            }

            EndAction();
        }

        protected override void OnAdded(Entity entity)
        {
            if (OnAddedAction == null)
            {
                return;
            }

            OnAddedAction(entity);
        }

        protected override void OnRemoved(Entity entity)
        {
            if (OnRemovedAction == null)
            {
                return;
            }

            OnRemovedAction(entity);
        }

        protected override IEnumerable<Entity> PreprocessEntities(IEnumerable<Entity> entities)
        {
            if (PreprocessEntitiesAction == null)
            {
                return entities;
            }

            return PreprocessEntitiesAction(entities);
        }

        protected override void Process(Entity entity, float deltaTime)
        {
            if (ProcessorAction == null)
            {
                return;
            }

            ProcessorAction(entity, deltaTime);
        }
    }
}
