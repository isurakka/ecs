using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class IntervalEntitySystem : System
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

        public IntervalEntitySystem(Aspect aspect, float interval)
            : base(aspect)
        {
            this.interval = interval;
        }

        internal sealed override void processAll(IEnumerable<Entity> entities, float deltaTime)
        {
            accumulator += deltaTime;

            while (accumulator >= interval)
            {
                accumulator -= interval;

                foreach (var item in entities)
                {
                    process(item, interval);
                }
            }
        }
    }
}
