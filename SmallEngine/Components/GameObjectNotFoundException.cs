using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    /// <summary>
    /// Exception that is thrown when a scene is unable to find a specific game object
    /// </summary>
    [Serializable]
    public sealed class GameObjectNotFoundException : Exception
    {
        public GameObjectNotFoundException(string pName) : base("GameObject with name " + pName + " was not found within the scene")
        { 
        }

        private GameObjectNotFoundException(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
        }
    }
}
