namespace ECS
{
    public abstract class System
    {
        protected internal EntityComponentSystem Context { protected get; set; }

        protected internal bool MissedUpdates { get; internal set; }

        internal virtual void SystemAddedInternal() => SystemAdded();

        protected virtual void SystemAdded() {  }

        internal virtual void SystemRemovedInternal() => SystemRemoved();

        protected virtual void SystemRemoved() {  }

        internal virtual void Update(float deltaTime) => Process(deltaTime);

        protected abstract void Process(float deltaTime);
    }
}
