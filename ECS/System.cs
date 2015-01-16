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

        protected virtual void begin() {  }

        protected virtual void end() {  }

        internal void Update(IEnumerable<Entity> entities, float deltaTime)
        {
            begin();

            var preprocessed = preprocessEntities(entities);
            processAll(preprocessed, deltaTime);

            end();
        }

        protected virtual IEnumerable<Entity> preprocessEntities(IEnumerable<Entity> entities)
        {
            return entities;
        }

        private abstract void processAll(IEnumerable<Entity> entities, float deltaTime);

        protected abstract void process(Entity entity, float deltaTime);
    }
}
