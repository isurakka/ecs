using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Entity
    {
        internal uint id;

        internal Entity(uint id)
        {
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Entity e = (Entity)obj;
            return EqualityComparer<Entity>.Default.Equals(e);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Entity>.Default.GetHashCode();
        }
    }

    public class EntityEqualityComparer : EqualityComparer<Entity>
    {
        public override bool Equals(Entity x, Entity y)
        {
            //if (x == null || y == null)
            //{
            //    return false;
            //}

            if (x.id == y.id)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode(Entity obj)
        {
            return unchecked((int)obj.id);
        }
    }
}