using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SmallEngine.Components
{
    public interface IComponent : IDisposable
    {
        /// <summary>
        /// Indicates if the component is active or not
        /// Inactive components will be removed from system processing
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// The game object this component is attached to
        /// Will be null if the component has not yet been added to a game object
        /// </summary>
        IGameObject GameObject { get; }

        Type RegistrationType { get; }

        IComparer<IComponent> Comparer { get; }

        /// <summary>
        /// Gets called when the component has been added to a game object
        /// This method is called even when components are deserialized 
        /// so it's an appropriate spot to initialize variables
        /// 
        /// Also imported components are guaranteed to be set
        /// </summary>
        /// <param name="pGameObject">Game object this component is being added to</param>
        void OnAdded(IGameObject pGameObject);

        /// <summary>
        /// Gets called after the component has been removed from a game object
        /// </summary>
        void OnRemoved();

        /// <summary>
        /// Gets called when the active property changed
        /// Removes component from any systems that are processing it
        /// </summary>
        /// <param name="pActive"></param>
        void OnActiveChanged(bool pActive);
    }
}
