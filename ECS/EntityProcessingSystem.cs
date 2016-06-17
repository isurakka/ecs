using System.Collections.Generic;

namespace ECS
{
    public abstract class EntityProcessingSystem : EntitySystem
    {
        protected EntityProcessingSystem(Aspect aspect)
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
