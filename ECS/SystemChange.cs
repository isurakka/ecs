using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected SystemChange(System system, ChangeType typeOfChange)
        {
            System = system;
            TypeOfChange = typeOfChange;
        }

        public static SystemChange CreateSystemAdded(System system)
            => new SystemChange(system, ChangeType.Added);

        public static SystemChange CreateSystemRemoved(System system)
            => new SystemChange(system, ChangeType.Removed);
    }
}
