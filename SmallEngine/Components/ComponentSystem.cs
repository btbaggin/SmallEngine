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
        /// <summary>
        /// List of components that is registered to this system
        /// </summary>
        protected List<IComponent> Components { get; set; }

        protected ComponentSystem()
        {
            Scene.Register(this);
            Components = new List<IComponent>();
        }

        /// <summary>
        /// Creates a new system to process components and registers all components of type pType
        /// </summary>
        /// <param name="pType">The type of components to register for processing</param>
        protected ComponentSystem(Type pType)
        {
            Scene.Register(this);
            Components = Component.GetComponentsOfType(pType);
        }

        /// <summary>
        /// Implement logic for processing components
        /// </summary>
        public abstract void Process();
    }
}
