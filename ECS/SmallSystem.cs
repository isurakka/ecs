using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class SmallEntitySystem : EntitySystem
    {
        Action<Entity, float> processor;

        public SmallEntitySystem(Aspect aspect, Action<Entity, float> processor)
            : base(aspect)
        {
            this.processor = processor;
        }

        protected override void Process(Entity entity, float deltaTime)
        {
            if (processor == null)
            {
                return;
            }

            processor(entity, deltaTime);
        }
    }
}
