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

        internal sealed override void processAll(IEnumerable<Entity> entities, float deltaTime)
        {
            Parallel.ForEach(entities, ent => Process(ent, deltaTime));
        }
    }
}
