using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Linq;

namespace ECS
{
    public class EntityChangeMonitor
    {
        private readonly Type[] MonitoredTypes;
        internal readonly BigInteger MonitoredTypesBi;
        internal readonly EntityComponentSystem Context;
        internal HashSet<Entity> Interested;
        internal List<(bool, Entity)> Changes = new List<(bool, Entity)>();
        private bool active = false;

        internal EntityChangeMonitor(EntityComponentSystem context, Type[] monitoredTypes)
        {
            MonitoredTypes = monitoredTypes;
            MonitoredTypesBi = context.componentMapper.TypesToBigInteger(MonitoredTypes);
            Context = context;
        }

        internal void TriggerInitialChanges()
        {
            Debug.Assert(!active);
            Interested = new HashSet<Entity>(Context.GetEntities(MonitoredTypes));
            foreach (var interested in Interested)
            {
                Changes.Add((true, interested));
            }
            active = true;
        }

        internal void TriggerRemoval()
        {
            Debug.Assert(active);
            foreach (var interested in Interested)
            {
                Changes.Add((false, interested));
            }
            active = false;
        }

        internal void AddChange(EntityChange entityChange)
        {
            Debug.Assert(active);
            Debug.Assert(Context.EntityComponentBits[entityChange.Entity.Id].HasValue);

            switch (entityChange.TypeOfChange)
            {
                case EntityChange.ChangeType.EntityAdded:
                    if (Context.componentMapper.Interested(Context.EntityComponentBits[entityChange.Entity.Id].Value, MonitoredTypesBi))
                    {
                        Interested.Add(entityChange.Entity);
                        Changes.Add((true, entityChange.Entity));
                    }
                    break;
                case EntityChange.ChangeType.EntityRemoved:
                    var removed = Interested.Remove(entityChange.Entity);
                    if (removed)
                    {
                        Changes.Add((false, entityChange.Entity));
                    }
                    break;
                case EntityChange.ChangeType.ComponentAdded:
                    if (!Interested.Contains(entityChange.Entity) && Context.componentMapper.Interested(Context.EntityComponentBits[entityChange.Entity.Id].Value, MonitoredTypesBi))
                    {
                        Interested.Add(entityChange.Entity);
                        Changes.Add((true, entityChange.Entity));
                    }
                    break;
                case EntityChange.ChangeType.ComponentRemoved:
                    if (Interested.Contains(entityChange.Entity) && !Context.componentMapper.Interested(Context.EntityComponentBits[entityChange.Entity.Id].Value, MonitoredTypesBi))
                    {
                        Interested.Remove(entityChange.Entity);
                        Changes.Add((false, entityChange.Entity));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public List<(bool, Entity)> FlushChanges()
        {
            var changes = Changes.ToList();
            Changes.Clear();
            return changes;
        }
    }
}