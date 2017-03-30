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

    public sealed class Write
    {
        private Write() {  }
    }

    public abstract class Read
    {
        private Read() { }
    }
}
