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
}
