using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Aspect
    {
        private BigInteger all = new BigInteger();
        private BigInteger any = new BigInteger();

        internal Aspect()
        {

        }

        private static int nextTypeIndexShift = 0;
        private static Dictionary<Type, int> typeIndex = new Dictionary<Type,int>();
        private static BigInteger MapType(Type type)
        {
            if (!typeIndex.ContainsKey(type))
            {
                typeIndex.Add(type, nextTypeIndexShift++);
            }

            return BigInteger.One << typeIndex[type];
        }

        public static Aspect Empty()
        {
            return new Aspect();
        }

        public Aspect AddAll(params Type[] types)
        {
            foreach (var type in types)
            {
                all &= MapType(type);
            }

            return this;
        }

        public static Aspect All(params Type[] types)
        {
            return new Aspect().AddAll(types);
        }

        public Aspect AddAny(params Type[] types)
        {
            foreach (var type in types)
            {
                any &= MapType(type);
            }

            return this;
        }

        public static Aspect Any(params Type[] types)
        {
            return new Aspect().AddAny(types);
        }

        public bool Interested(BigInteger other)
        {
            bool allSuccess = (all & other).Equals(all);
            bool anySuccess = !(any & other).IsZero;

            return allSuccess && anySuccess;
        }
    }
}
