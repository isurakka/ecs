using ECS;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Aspect
    {
        private readonly ImmutableHashSet<Type> all;
        private readonly ImmutableHashSet<Type> any;

        internal Aspect(ImmutableHashSet<Type> all, ImmutableHashSet<Type> any)
        {
            this.all = all;
            this.any = any;
        }

        public static Aspect Empty()
        {
            return new Aspect(
                ImmutableHashSet<Type>.Empty,
                ImmutableHashSet<Type>.Empty);
        }

        public static Aspect All(params Type[] types)
        {
            return new Aspect(
                ImmutableHashSet.Create(types), 
                ImmutableHashSet<Type>.Empty);
        }

        public Aspect AndWithAll(params Type[] types)
        {
            return new Aspect(
                all.Union(types),
                any);
        }

        public static Aspect Any(params Type[] types)
        {
            return new Aspect(
                ImmutableHashSet<Type>.Empty,
                ImmutableHashSet.Create(types));
        }

        public Aspect AndWithAny(params Type[] types)
        {
            return new Aspect(
                all,
                any.Union(types));
        }

        // TODO: Is this dumb?
        public bool InterestedInMappedValue(ComponentTypesToBigIntegerMapper mapper, BigInteger bi)
        {
            var allBi = mapper.TypesToBigInteger(all);
            var anyBi = mapper.TypesToBigInteger(any);

            return (allBi & bi) == allBi && 
                    (anyBi == BigInteger.Zero || (anyBi & bi) != BigInteger.Zero);
        }
    }
}
