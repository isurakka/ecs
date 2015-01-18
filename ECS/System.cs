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

        internal IEnumerable<Entity> lastInterested;

        private EntityComponentSystem context;
        protected internal EntityComponentSystem Context
        {
            get
            {
                return context;
            }
            internal set
            {
                context = value;
            }
        }

        internal System(Aspect aspect)
        {
            this.Aspect = aspect;
        }

        protected virtual void Begin() {  }

        protected virtual void End() {  }

        protected internal virtual void OnAdded(Entity entity) {  }

        protected internal virtual void OnRemoved(Entity entity) { }

        internal void Update(IEnumerable<Entity> entities, float deltaTime)
        {
            Begin();

            var preprocessed = PreprocessEntities(entities);
            processAll(preprocessed, deltaTime);

            End();
        }

        protected virtual IEnumerable<Entity> PreprocessEntities(IEnumerable<Entity> entities)
        {
            return entities;
        }

        internal abstract void processAll(IEnumerable<Entity> entities, float deltaTime);

        protected abstract void Process(Entity entity, float deltaTime);
    }
}
