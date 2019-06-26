using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    public abstract class ComponentSystem
    {
        protected IEnumerable<IComponent> Components { get; set; } = new List<IComponent>();

        protected ComponentSystem()
        {
            Scene.Register(this);
        }

        public void Process()
        {
            lock(Components)
            {
                DoProcess();
            }
        }
        protected abstract void DoProcess();
    }
}
