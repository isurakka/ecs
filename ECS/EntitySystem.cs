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

        protected sealed override void processAll(IEnumerable<Entity> entities)
        {
            foreach (var item in entities)
            {
                process(item);
            }
        }

        protected abstract override void process(Entity entity);
    }
}
