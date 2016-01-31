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
        public Func<IEnumerable<Entity>, IEnumerable<Entity>> PreprocessEntitiesAction;

        public ClosureEntityProcessingSystem(Aspect aspect, Action<Entity, float> processor)
            : base(aspect)
        {
            this.ProcessorAction = processor;
        }

        protected override void Begin()
        {
            BeginAction?.Invoke();
        }

        protected override void End()
        {
            EndAction?.Invoke();
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
            ProcessorAction?.Invoke(entity, deltaTime);
        }
    }
}
