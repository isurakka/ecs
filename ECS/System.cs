using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class System
    {
        private EntityComponentSystem context;
        protected internal EntityComponentSystem Context
        {
            get
            {
                return context;
            }
            internal set
            {
                context = value;
            }
        }

        public System()
        {
            
        }

        internal virtual void Update(float deltaTime)
        {
            Process(deltaTime);
        }

        protected abstract void Process(float deltaTime);
    }
}
