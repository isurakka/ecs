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
                    }

                    break;
                }
            }

            while (toAddSystem.Count > 0)
            {
                var tuple = toRemoveSystem.Dequeue();
                var priority = tuple.Item1;
                var system = tuple.Item2;

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

        internal void UpdateSystem(KeyValuePair<int, List<System>> systemPair, Dictionary<Entity, EntityData> entities, float deltaTime)
        {
            foreach (var system in systemPair.Value)
            {
                // Is this too slow?
                var interested = entities
                    .Where(pair => system.Aspect.Interested(pair.Value.Types))
                    .Select(pair => pair.Key);

                system.processAll(interested, deltaTime);
            } 
        }

        internal bool Interested(System system, Entity entity)
        {
            //return system.Aspect.Interested(entity.)
        }
    }
}
