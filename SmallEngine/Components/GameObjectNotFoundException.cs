using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    [Serializable]
    public class GameObjectNotFoundException : Exception
    {
        public GameObjectNotFoundException(string pName) : base("GameObject with name " + pName + " was not found within the scene")
        { 
        }

        protected GameObjectNotFoundException(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
        }
    }
}
