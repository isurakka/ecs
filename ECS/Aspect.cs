using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;

namespace ECS
{

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
            => cache ?? (cache = new Cached(componentMapper.TypesToBigInteger(all), componentMapper.TypesToBigInteger(any)));

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
