using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace ECS
{
    public enum ComponentAccessMode
    {
        Read,
        Write,
    }

    public class ComponentAccess : IDisposable
    {
        internal IDisposable[] Acquires;
        internal BigInteger AcquiredTypesHash;
        internal IObserver<BigInteger> TypesFreed;
        public List<Entity> Entities { get; internal set; }

        public void Dispose()
        {
            foreach (var item in Acquires)
            {
                item.Dispose();
            }

            TypesFreed.OnNext(AcquiredTypesHash);
        }
    }
}
