using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class EntityChange
    {
        public enum ChangeType
        {
            EntityAdded,
            EntityRemoved,
            ComponentAdded,
            ComponentRemoved,
        }

        public Entity Entity { get; }
        public IComponent RelevantComponent { get; }

        private Type componentType;
        public Type ComponentType => componentType ?? (componentType = RelevantComponent?.GetType());

        public ChangeType TypeOfChange { get; }

        private EntityChange(Entity entity, IComponent relevantComponent, Type componentType, ChangeType typeOfChange)
        {
            Entity = entity;
            RelevantComponent = relevantComponent;
            this.componentType = componentType;
            TypeOfChange = typeOfChange;
        }

        public static EntityChange CreateEntityAdded(Entity entity)
            => new EntityChange(entity, null, null, ChangeType.EntityAdded);

        public static EntityChange CreateEntityRemoved(Entity entity)
            => new EntityChange(entity, null, null, ChangeType.EntityRemoved);

        public static EntityChange CreateComponentAdded(Entity entity, IComponent component, Type componentType)
            => new EntityChange(entity, component, componentType, ChangeType.ComponentAdded);

        public static EntityChange CreateComponentRemoved(Entity entity, IComponent component, Type componentType)
            => new EntityChange(entity, component, componentType, ChangeType.ComponentRemoved);
    }
}
