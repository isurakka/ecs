using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public interface IComponent
    {

    }

    public class ComponentEqualityComparer : IEqualityComparer<IComponent>
    {
        static ComponentEqualityComparer instance;
        public static ComponentEqualityComparer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComponentEqualityComparer();
                }

                return instance;
            }
        }

        private ComponentEqualityComparer()
        {

        }

        public bool Equals(IComponent x, IComponent y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.GetType() == y.GetType();
        }

        public int GetHashCode(IComponent obj)
        {
            return obj.GetType().GetHashCode();
        }
    }
}
