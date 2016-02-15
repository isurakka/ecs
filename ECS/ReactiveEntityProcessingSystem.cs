using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class ReactiveEntityProcessingSystem : ReactiveEntitySystem
    {
        protected ReactiveEntityProcessingSystem(Aspect aspect)
            : base(aspect)
        {

        }

        protected ReactiveEntityProcessingSystem(params Type[] types)
            : this(Aspect.All(types))
        {

        }

        protected sealed override void ProcessEntities(IEnumerable<Entity> entities, float deltaTime)
        {
            foreach (var item in entities)
            {
                Process(item, deltaTime);
            }
        }

        protected abstract void Process(Entity entity, float deltaTime);
    }
}
