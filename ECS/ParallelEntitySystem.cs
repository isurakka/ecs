using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class ParallelEntitySystem : System
    {
        public ParallelEntitySystem(Aspect aspect)
            : base(aspect)
        {

        }

        protected sealed override void processAll(IEnumerable<Entity> entities)
        {
            Parallel.ForEach(entities, process);
        }

        protected abstract override void process(Entity entity);
    }
}
