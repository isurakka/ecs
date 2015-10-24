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
        internal enum Change
        {
            Add,
            Remove,
        }

        internal EntityComponentSystem context;

        internal SortedDictionary<int, List<System>> systems;

        private readonly Dictionary<int, List<Tuple<Change, System>>> systemsToChange;

        internal SystemManager()
        {
            systems = new SortedDictionary<int, List<System>>();
            systemsToChange = new Dictionary<int, List<Tuple<Change, System>>>();
        }

        internal void AddSystem(System system, int layer) => 
            systemsToChange.GetOrAddNew(layer).Add(Tuple.Create(Change.Add, system));

        internal void RemoveSystem(System system, int layer) => 
            systemsToChange.GetOrAddNew(layer).Add(Tuple.Create(Change.Remove, system));

        internal void FlushPendingChanges()
        {
            foreach (var pair in systemsToChange)
            {
                var layer = pair.Key;
                var value = pair.Value;

                foreach (var tuple in value)
                {
                    var system = tuple.Item2;
                    if (tuple.Item1 == Change.Add)
                    {
                        system.Context = context;
                        systems.GetOrAddNew(layer).Add(system);
                        system.SystemAddedInternal();
                    }
                    else
                    {
                        system.Context = null;
                        var systemsInLayer = systems[layer];
                        systemsInLayer.Remove(system);
                        system.SystemRemovedInternal();

                        if (systemsInLayer.Count <= 0)
                        {
                            systems.Remove(layer);
                        }
                    }
                }
            }

            systemsToChange.Clear();
        }

        internal IEnumerable<System> GetSystems(int layer)
        {
            List<System> ret;
            if (!systems.TryGetValue(layer, out ret))
            {
                return Enumerable.Empty<System>();
            }

            return ret;
        }

        internal KeyValuePair<int, List<System>>? NextSystemsInclusive(int layer)
        {
            var first = systems.FirstOrDefault(pair => pair.Key >= layer);
            return first.Value == null ? null : new KeyValuePair<int, List<System>>?(first);
        }

        internal IEnumerable<List<System>> GetSystemsInLayerOrder()
        {
            KeyValuePair<int, List<System>>? nextSystems;
            var nextKey = int.MinValue;
            do
            {
                nextSystems = NextSystemsInclusive(nextKey);
                if (nextSystems == null) continue;

                yield return nextSystems.Value.Value;

                if (nextKey == int.MaxValue)
                {
                    yield break;
                }
                nextKey = nextSystems.Value.Key + 1;
            } while (nextSystems != null);
        }
    }
}
