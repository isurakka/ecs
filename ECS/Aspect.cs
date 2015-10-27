using ECS;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public static class AspectMapper
    {
        private static readonly Dictionary<Type, int> shiftForType = new Dictionary<Type, int>();

        public static BigInteger TypesToBigInteger(params Type[] types)
        {
            return TypesToBigInteger((IEnumerable<Type>)types);
        }

        public static BigInteger TypesToBigInteger(IEnumerable<Type> types)
        {
            var ret = BigInteger.Zero;
            foreach (var type in types)
            {
                Debug.Assert(typeof(IComponent).IsAssignableFrom(type),
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

    public class Aspect
    {
        private readonly BigInteger all;
        private readonly BigInteger any;

        internal Aspect(BigInteger all, BigInteger any)
        {
            this.all = all;
            this.any = any;
        }

        public static Aspect Empty()
        {
            return new Aspect(
                BigInteger.Zero,
                BigInteger.Zero);
        }

        public static Aspect All(params Type[] types)
        {
            return new Aspect(
                AspectMapper.TypesToBigInteger(types),
                BigInteger.Zero);
        }

        public Aspect AndWithAll(params Type[] types)
        {
            return new Aspect(
                all | AspectMapper.TypesToBigInteger(types),
                any);
        }

        public static Aspect Any(params Type[] types)
        {
            return new Aspect(
                BigInteger.Zero,
                AspectMapper.TypesToBigInteger(types));
        }

        public Aspect AndWithAny(params Type[] types)
        {
            return new Aspect(
                all,
                any | AspectMapper.TypesToBigInteger(types));
        }

        // TODO: Is this dumb?
        public bool Interested(BigInteger bi)
        {
            return (all & bi) == all && 
                    (any == BigInteger.Zero || (any & bi) != BigInteger.Zero);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return (all.GetHashCode() * 397) ^ any.GetHashCode();
            }
        }
    }
}
