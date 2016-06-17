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
    // TODO: Remove static and support passing mapper to Aspect in some way when caching
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
                    $"{nameof(AspectMapper)} should only be used with {nameof(IComponent)} types");

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
        private readonly ImmutableHashSet<Type> all;
        private readonly ImmutableHashSet<Type> any;

        private Aspect(ImmutableHashSet<Type> all, ImmutableHashSet<Type> any)
        {
            this.all = all;
            this.any = any;
        }

        /// <summary>
        /// Interested of all of the specified types. Empty array means everything is interesting.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Aspect All(params Type[] types)
            => new Aspect(
                ImmutableHashSet.Create(types),
                ImmutableHashSet.Create<Type>());

        /// <summary>
        /// Interested of any of the specified types. Empty array means everything is interesting.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Aspect Any(params Type[] types)
            => new Aspect(
                ImmutableHashSet.Create<Type>(),
                ImmutableHashSet.Create(types));

        public Aspect AndWithAll(params Type[] types)
            => new Aspect(
                all.Union(types),
                any);

        public Aspect AndWithAny(params Type[] types)
            => new Aspect(
                all,
                any.Union(types));

        private Cached cache;
        public Cached Cache
            => cache ?? (cache = new Cached(AspectMapper.TypesToBigInteger(all), AspectMapper.TypesToBigInteger(any)));

        public class Cached
        {
            private readonly BigInteger all;
            private readonly BigInteger any;

            public Cached(BigInteger all, BigInteger any)
            {
                this.all = all;
                this.any = any;
            }

            public bool Interested(BigInteger bi)
                => (all & bi) == all &&
                    (any == BigInteger.Zero || (any & bi) != BigInteger.Zero);

            public bool Interested(BigInteger? bi)
                => bi != null && Interested(bi.Value);
        }
    }
}
