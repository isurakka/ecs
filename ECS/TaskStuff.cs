using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class ComponentAccess<T> : IDisposable
    {
        internal List<IDisposable> Acquires;
        public T Components { get; internal set; }

        public void Dispose()
        {
            foreach (var item in Acquires)
            {
                item.Dispose();
            }
        }
    }

    public abstract class ReadOrWrite
    {

    }

    public class Write<T> : Read<T>
        where T : IComponent
    {

    }

    public class Read<T> : ReadOrWrite
        where T: IComponent
    {

    }
}
