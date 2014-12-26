using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    internal class SystemManager
    {
        internal SortedDictionary<int, List<System>> systems = new SortedDictionary<int, List<System>>();

        private Queue<Tuple<int, System>> toAddSystem = new Queue<Tuple<int, System>>();
        private Queue<Tuple<int, System>> toRemoveSystem = new Queue<Tuple<int, System>>();

        internal SystemManager()
        {

        }

        internal void AddSystem(System system, int priority)
        {
            toAddSystem.Enqueue(new Tuple<int, System>(priority, system));
        }

        internal void RemoveSystem(System system, int priority)
        {
            toRemoveSystem.Enqueue(new Tuple<int, System>(priority, system));
        }

        internal void ProcessQueues()
        {
            while (toRemoveSystem.Count > 0)
            {
                var tuple = toRemoveSystem.Dequeue();
                var priority = tuple.Item1;
                var system = tuple.Item2;

                // TODO: Remove everything on the first loop instead of looping for each system
                bool removed = false;
                for (int i = 0; i < systems[priority].Count; i++)
                {
                    if (systems[priority][i] != system)
                    {
                        continue;
                    }

                    systems[priority].RemoveAt(i);
                    if (systems[priority].Count <= 0)
                    {
                        systems.Remove(priority);
                        removed = true;
                    }

                    break;
                }

                if (!removed)
                {
                    throw new ArgumentException("Could not remove the system specified");
                }
            }

            while (toAddSystem.Count > 0)
            {
                var tuple = toRemoveSystem.Dequeue();
                var priority = tuple.Item1;
                var system = tuple.Item2;

                if (systems.Values.Any(systemsPriority => systemsPriority.Any(sys => sys.GetType() == system.GetType())))
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

        internal IEnumerable<IEnumerable<System>> SystemsByPriority()
        {
            foreach (var pair in systems)
            {
                yield return pair.Value;
            }
        }
    }
}
