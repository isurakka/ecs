using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class System
    {
        internal readonly Aspect Aspect;

        internal EntityComponentSystem context;
        protected EntityComponentSystem Context
        {
            get
            {
                return context;
            }
        }

        internal System(Aspect aspect)
        {
            this.Aspect = aspect;
        }

        internal abstract void processAll(IEnumerable<Entity> entities, float deltaTime);

        protected abstract void process(Entity entity, float deltaTime);
    }
}
