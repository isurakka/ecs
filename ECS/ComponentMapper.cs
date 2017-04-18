using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECS
{
    // TODO: Remove static and support passing mapper to Aspect in some way when caching
    public class ComponentMapper
    {
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
                if (!ShiftForType.TryGetValue(type, out int shift))
                {
                    shift = ShiftForType.Count;
                    ShiftForType[type] = shift;
                }
                ret |= BigInteger.One << shift;
            }
            return ret;
        }

        public bool Interested(BigInteger superset, BigInteger subset) => (superset & subset) == subset;
    }
}
