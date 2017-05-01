﻿using System;
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
        internal List<IDisposable> Acquires;
        internal ConcurrentDictionary<Guid, AsyncCountdownEvent> FreedEvents;
        public List<Entity> Entities { get; internal set; }

        public void Dispose()
        {
            foreach (var item in Acquires)
            {
                item.Dispose();
            }

            foreach (var item in FreedEvents)
            {
                item.Value.Signal();
            }
        }
    }
}