using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    internal class ComponentManager
    {
        // one instance for each component type
        internal int componentCount = 0;
        internal Dictionary<Type, int> indicesForComponents;

        // first index type index, second index entity id
        internal IComponent[][] components;

        internal ComponentManager()
        {
            indicesForComponents = new Dictionary<Type, int>();
            components = new IComponent[1][];
        }
    }
}
