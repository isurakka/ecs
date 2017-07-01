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
        private Action cleanUp;
        public readonly Dictionary<int, IDictionary<Type, object>> Entities;

        internal ComponentAccess(Action cleanUp, Dictionary<int, IDictionary<Type, object>> entities)
        {
            this.cleanUp = cleanUp;
            Entities = entities;
        }

        public void Dispose()
        {
            cleanUp();
        }
    }
}
