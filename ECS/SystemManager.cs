using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class SystemManager
    {
        private SortedDictionary<int, List<System>> systems = new SortedDictionary<int, List<System>>();

        internal SystemManager()
        {

        }

        public void AddSystem(System system, int priority = 0)
        {
            if (systems.Values.Any(syss => syss.Any(sys => sys.GetType() == system.GetType())))
            {
                throw new ArgumentException("System of this type is already added");
            }

            if (systems[priority] == null)
            {
                systems[priority] = new List<System>();
            }

            systems[priority].Add(system);
        }
    }
}
