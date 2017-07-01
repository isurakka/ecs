using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace ECS
{
    // TODO: Remove static and support passing mapper to Aspect in some way when caching
    public class ComponentMapper
    {
        //public int NextTypeIndex => ShiftForType.Count;
        internal List<Type> AllComponentTypes => ShiftForType.Keys.ToList();
        private readonly ConcurrentDictionary<Type, int> ShiftForType = new ConcurrentDictionary<Type, int>();

        public BigInteger TypesToBigInteger(params Type[] types)
        {
            return TypesToBigInteger((IEnumerable<Type>)types);
        }

        public BigInteger TypesToBigInteger(IEnumerable<Type> types)
        {
            var ret = BigInteger.Zero;
            foreach (var type in types)
            {
                var shift = GetIndexForType(type);
                ret |= BigInteger.One << shift;
            }
            return ret;
        }

        public int GetIndexForType(Type type)
        {
            Debug.Assert(ShiftForType.Values.GroupBy(v => v).All(group => group.Count() == 1));

            // TODO: Is this thread unsafe?
            return ShiftForType.GetOrAdd(type, t => ShiftForType.Count);

            /*
            if (!ShiftForType.TryGetValue(type, out int shift))
            {
                shift = ShiftForType.Count;
                ShiftForType[type] = shift;
            }
            

            return shift;
            */
        }

        public void AddTypeToBigInteger(ref BigInteger bi, Type type)
        {
            bi = bi | TypesToBigInteger(type);
        }

        public void RemoveTypeFromBigInteger(ref BigInteger bi, Type type)
        {
            bi = bi & ~TypesToBigInteger(type);
        }

        public bool Interested(BigInteger superset, BigInteger subset) => (superset & subset) == subset;
        public bool Intersects(BigInteger a, BigInteger b) => (a & b) != BigInteger.Zero;
    }
}
