using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    internal class EntityData
    {
        public List<IComponent> Components = new List<IComponent>();
        public HashSet<Type> Types = new HashSet<Type>();
    }
}
