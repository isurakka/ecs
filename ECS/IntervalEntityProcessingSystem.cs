using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class IntervalEntityProcessingSystem : EntitySystem
    {
        private float accumulator;

        public float Interval { get; protected set; }
        public EntityExecution Execution { get; }

        protected IntervalEntityProcessingSystem(Aspect aspect, float interval, EntityExecution execution = EntityExecution.Synchronous)
            : base(aspect)
        {
            this.Interval = interval;
            Execution = execution;
        }

        protected sealed override void ProcessEntities(IEnumerable<Entity> entities, float deltaTime)
        {
            accumulator += deltaTime;

            while (accumulator >= Interval)
            {
                accumulator -= Interval;

                if (Execution == EntityExecution.Synchronous)
                {
                    foreach (var ent in entities)
                    {
                        Process(ent, Interval);
                    }
                }
                else
                {
                    Parallel.ForEach(entities, ent =>
                    {
                        Process(ent, Interval);
                    });
                }
            }
        }

        protected abstract void Process(Entity entity, float deltaTime);
    }
}
