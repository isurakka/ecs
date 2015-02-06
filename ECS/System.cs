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

        internal IEnumerable<Entity> lastInterested = new List<Entity>();

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

        protected virtual void OnAdded(Entity entity) {  }

        protected virtual void OnRemoved(Entity entity) { }

        internal void Update(IEnumerable<Entity> entities, float deltaTime)
        {
            // Check for added and removed entities and call the respective methods for them
            // TODO: There is probably more efficient way to check for added and removed entities
            var added = entities.Except(lastInterested);
            var removed = lastInterested.Except(entities);

            foreach (var removedEntity in removed)
            {
                OnRemoved(removedEntity);
            }

            foreach (var addedEntity in added)
            {
                OnAdded(addedEntity);
            }

            lastInterested = entities.ToList();

            // Start of actual processing
            Begin();

            var preprocessed = PreprocessEntities(entities);
            ProcessEntities(preprocessed, deltaTime);

            End();
        }

        protected virtual IEnumerable<Entity> PreprocessEntities(IEnumerable<Entity> entities)
        {
            return entities;
        }

        protected abstract void ProcessEntities(IEnumerable<Entity> entities, float deltaTime);
    }
}
