using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class ComponentTypesToBigIntegerMapper
    {
        private readonly Dictionary<Type, int> shiftForType;
        private readonly Guid guid;

        public ComponentTypesToBigIntegerMapper()
        {
            guid = Guid.NewGuid();
            shiftForType = new Dictionary<Type, int>();
        }

        public BigInteger TypesToBigInteger(params Type[] types)
        {
            return TypesToBigInteger((IEnumerable<Type>)types);
        }

        // TODO: Maybe memoize result for one parameter as that is most used if this becomes bottleneck
        public BigInteger TypesToBigInteger(IEnumerable<Type> types)
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

        protected bool Equals(ComponentTypesToBigIntegerMapper other) => guid.Equals(other.guid);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentTypesToBigIntegerMapper) obj);
        }

        public override int GetHashCode() => guid.GetHashCode();

        public static bool operator ==(ComponentTypesToBigIntegerMapper left, ComponentTypesToBigIntegerMapper right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ComponentTypesToBigIntegerMapper left, ComponentTypesToBigIntegerMapper right)
        {
            return !Equals(left, right);
        }
    }
}
