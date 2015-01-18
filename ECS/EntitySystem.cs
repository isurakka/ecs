using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class EntitySystem : System
    {
        public EntitySystem(Aspect aspect)
            : base(aspect)
        {

        }

        internal sealed override void processAll(IEnumerable<Entity> entities, float deltaTime)
        {
            foreach (var item in entities)
            {
                Process(item, deltaTime);
            }
        }
    }
}
