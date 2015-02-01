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

        protected sealed override void ProcessEntities(IEnumerable<Entity> entities, float deltaTime)
        {
            Parallel.ForEach(entities, ent => Process(ent, deltaTime));
        }

        protected abstract void Process(Entity entity, float deltaTime);
    }
}
