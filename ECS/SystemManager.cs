using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    internal class SystemManager
    {
        internal EntityComponentSystem context;

        internal SortedDictionary<int, List<System>> systems = new SortedDictionary<int, List<System>>();

        private ConcurrentBag<Tuple<int, System>> toAddSystem = new ConcurrentBag<Tuple<int, System>>();
        private ConcurrentBag<Tuple<int, System>> toRemoveSystem = new ConcurrentBag<Tuple<int, System>>();

        internal SystemManager()
        {

        }

        internal void SetSystem(System system, int priority)
        {
            toAddSystem.Add(Tuple.Create(priority, system));
        }

        internal void RemoveSystem(System system, int priority)
        {
            toRemoveSystem.Add(Tuple.Create(priority, system));
        }

        internal void ProcessQueues()
        {
            Tuple<int, System> tuple;

            while (toRemoveSystem.TryTake(out tuple))
            {
                var priority = tuple.Item1;
                var system = tuple.Item2;

                // TODO: Remove everything on the first loop instead of looping for each system?
                bool removed = false;
                for (int i = 0; i < systems[priority].Count; i++)
                {
                    if (systems[priority][i] != system)
                    {
                        continue;
                    }

                    systems[priority].RemoveAt(i);
                    removed = true;

                    if (systems[priority].Count <= 0)
                    {
                        systems.Remove(priority);
                    }

                    break;
                }

                if (!removed)
                {
                    throw new ArgumentException("Could not remove the system specified");
                }
            }

            while (toAddSystem.TryTake(out tuple))
            {
                var priority = tuple.Item1;
                var system = tuple.Item2;
                system.Context = context;

                if (!systems.ContainsKey(priority))
                {
                    systems[priority] = new List<System>();
                }
                else if (systems[priority].Any(s => s.GetType() == system.GetType()))
                {
                    throw new InvalidOperationException("System of this type is already added to this priority level");
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
