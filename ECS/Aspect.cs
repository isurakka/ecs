using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class AspectFactory
    {
        private ComponentTypesToBigIntegerMapper mapper;

        internal AspectFactory(ComponentTypesToBigIntegerMapper mapper)
        {
            this.mapper = mapper;
        }

        public Aspect All(params Type[] types)
        {
            return new Aspect(mapper).AddAll(types);
        }

        public Aspect Any(params Type[] types)
        {
            return new Aspect(mapper).AddAny(types);
        }
    }

    public class Aspect
    {
        private BigInteger all;
        private BigInteger any;
        private readonly ComponentTypesToBigIntegerMapper mapper;

        internal Aspect(ComponentTypesToBigIntegerMapper mapper)
        {
            all = new BigInteger();
            any = new BigInteger();
            this.mapper = mapper;
        }

        public Aspect AddAll(params Type[] types)
        {
            all &= mapper.TypesToBigInteger(types);
            return this;
        }

        public Aspect AddAny(params Type[] types)
        {
            any |= mapper.TypesToBigInteger(types);
            return this;
        }

        public bool Interested(BigInteger bi)
        {
            return (all & bi) == all && 
                   (any & bi) != BigInteger.Zero;
        }
    }
}
