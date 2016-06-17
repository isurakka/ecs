using System.Collections.Generic;

namespace ECS
{
    public abstract class IntervalEntityProcessingSystem : EntitySystem
    {
        private float accumulator;

        public float Interval { get; protected set; }

        protected IntervalEntityProcessingSystem(Aspect aspect, float interval)
            : base(aspect)
        {
            this.Interval = interval;
        }

        protected sealed override void ProcessEntities(IEnumerable<Entity> entities, float deltaTime)
        {
            accumulator += deltaTime;

            while (accumulator >= Interval)
            {
                accumulator -= Interval;

                foreach (var item in entities)
                {
                    Process(item, Interval);
                }
            }
        }

        protected abstract void Process(Entity entity, float deltaTime);
    }
}
