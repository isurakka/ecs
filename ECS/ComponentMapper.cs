using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECS
{
    // TODO: Remove static and support passing mapper to Aspect in some way when caching
    public class ComponentMapper
    {
        //public int NextTypeIndex => ShiftForType.Count;
        private readonly Dictionary<Type, int> ShiftForType = new Dictionary<Type, int>();

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
            if (!ShiftForType.TryGetValue(type, out int shift))
            {
                shift = ShiftForType.Count;
                ShiftForType[type] = shift;
            }

            return shift;
        }

        public bool Interested(BigInteger superset, BigInteger subset) => (superset & subset) == subset;
        public bool Intersects(BigInteger a, BigInteger b) => (a & b) != BigInteger.Zero;
    }
}
