using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    internal class ComponentTypesToBigIntegerMapper
    {
        Dictionary<Type, int> shiftForType;

        public ComponentTypesToBigIntegerMapper()
        {
            shiftForType = new Dictionary<Type, int>();
        }

        public BigInteger TypesToBigInteger(params Type[] types)
        {
            var ret = BigInteger.Zero;
            foreach (var type in types)
            {
                Debug.Assert(type == typeof(IComponent), 
                    $"{nameof(ComponentTypesToBigIntegerMapper)} should only be used with {nameof(IComponent)} types");

                int shift;
                var success = shiftForType.TryGetValue(type, out shift);
                if (!success)
                {
                    shift = shiftForType.Count;
                    shiftForType[type] = shift;
                }
                ret |= BigInteger.One << shift;
            }
            return ret;
        }
    }
}
