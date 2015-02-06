using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Aspect
    {
        private HashSet<Type> all = new HashSet<Type>();
        private HashSet<Type> any = new HashSet<Type>();

        internal Aspect()
        {

        }

        public static Aspect Empty()
        {
            return new Aspect();
        }

        public Aspect AddAll(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                all.Add(types[i]);
            }

            return this;
        }

        public static Aspect All(params Type[] types)
        {
            return new Aspect().AddAll(types);
        }

        public Aspect AddAny(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                any.Add(types[i]);
            }

            return this;
        }

        public static Aspect Any(params Type[] types)
        {
            return new Aspect().AddAny(types);
        }

        public bool Interested(IEnumerable<Type> other)
        {
            return 
                all.IsSubsetOf(other) &&
                ((any.Count <= 0 || other.Count() <= 0) || any.Overlaps(other));
        }
    }
}
