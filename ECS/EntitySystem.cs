using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class EntitySystem : System
    {
        internal IEnumerable<Entity> LastInterested = new List<Entity>();
        internal readonly Aspect Aspect;
        internal int AspectHash { get; }

        protected EntitySystem(Aspect aspect)
        {
            Aspect = aspect;
            AspectHash = aspect.GetHashCode();
        }

        internal override void SystemRemovedInternal()
        {
            foreach (var item in LastInterested)
            {
                OnRemoved(item);
            }

            LastInterested = new List<Entity>();

            base.SystemRemovedInternal();
        }

        /// <summary>
        /// Called before each update, before preprocessing. 
        /// Even if no entities get processed
        /// </summary>
        protected virtual void Begin() { }

        /// <summary>
        /// Called after each update
        /// </summary>
        protected virtual void End() { }

        protected virtual void OnAdded(Entity entity) { }

        protected virtual void OnRemoved(Entity entity) { }

        internal sealed override void Update(float deltaTime)
        {
            var entities = Context.FindEntities(Aspect);

            // Check for added and removed entities and call the respective methods for them
            // TODO: There is probably more efficient way to check for added and removed entities
            // TODO: Check only if entity has changed
            var added = entities.Except(LastInterested);
            var removed = LastInterested.Except(entities);

            foreach (var removedEntity in removed)
            {
                OnRemoved(removedEntity);
            }

            foreach (var addedEntity in added)
            {
                OnAdded(addedEntity);
            }

            LastInterested = entities;

            // Start of actual processing
            // TODO: Should begin, end and processing happen if there are no entities to process? (probably not)
            Begin();

            var preprocessed = PreprocessEntities(entities);
            ProcessEntities(preprocessed, deltaTime);

            End();
        }

        protected sealed override void Process(float deltaTime) { }

        protected abstract void ProcessEntities(IEnumerable<Entity> entities, float deltaTime);

        protected virtual IEnumerable<Entity> PreprocessEntities(IEnumerable<Entity> entities)
        {
            return entities;
        }
    }
}
