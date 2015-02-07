using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class EntityProcessingSystem : EntitySystem
    {
        public EntityProcessingSystem(Aspect aspect)
            : base(aspect)
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
