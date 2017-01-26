using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class EntityProcessingSystem : EntitySystem
    {
        public EntityExecution Execution { get; }

        protected EntityProcessingSystem(Aspect aspect, EntityExecution execution = EntityExecution.Synchronous)
            : base(aspect)
        {
            Execution = execution;
        }

        protected sealed override void ProcessEntities(IEnumerable<Entity> entities, float deltaTime)
        {
            if (Execution == EntityExecution.Synchronous)
            {
                foreach (var ent in entities)
                {
                    Process(ent, deltaTime);
                }
            }
            else
            {
                Parallel.ForEach(entities, ent =>
                {
                    Process(ent, deltaTime);
                });
            }
        }

        protected abstract void Process(Entity entity, float deltaTime);
    }
}
