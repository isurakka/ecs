namespace ECS
{
    public class SystemChange
    {
        public enum ChangeType
        {
            Added,
            Removed,
        }

        public System System { get; }
        public ChangeType TypeOfChange { get; }
        public SystemExecution Execution { get; }

        private SystemChange(System system, ChangeType typeOfChange, SystemExecution execution)
        {
            System = system;
            TypeOfChange = typeOfChange;
            Execution = execution;
        }

        public static SystemChange CreateSystemAdded(System system, SystemExecution execution)
            => new SystemChange(system, ChangeType.Added, execution);

        public static SystemChange CreateSystemRemoved(System system)
            => new SystemChange(system, ChangeType.Removed, SystemExecution.Synchronous); // execution parameter doesn't matter here
    }
}
