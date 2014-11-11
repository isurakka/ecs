using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class System
    {
        internal Aspect Aspect;

        internal System(Aspect aspect)
        {
            this.Aspect = aspect;
        }

        protected abstract void processAll(IEnumerable<Entity> entities);

        protected abstract void process(Entity entity);
    }
}
