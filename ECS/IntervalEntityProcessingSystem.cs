using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class IntervalEntityProcessingSystem : EntitySystem
    {
        private float accumulator;

        private float interval;
        public float Interval
        {
            get
            {
                return interval;
            }
            private set
            {
                interval = value;
            }
        }

        public IntervalEntityProcessingSystem(Aspect aspect, float interval)
            : base(aspect)
        {
            this.interval = interval;
        }

        protected sealed override void ProcessEntities(IEnumerable<Entity> entities, float deltaTime)
        {
            accumulator += deltaTime;

            while (accumulator >= interval)
            {
                accumulator -= interval;

                foreach (var item in entities)
                {
                    Process(item, interval);
                }
            }
        }

        protected abstract void Process(Entity entity, float deltaTime);
    }
}
