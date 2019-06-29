using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Serialization
{
    [Serializable]
    public sealed class ResourceNotLoadedException : Exception
    {
        public ResourceNotLoadedException(string pAlias) : base($"Attempted to deserialize Resource {pAlias} before it was loaded into the ResourceManager")
        {
        }

        private ResourceNotLoadedException(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
        }
    }
}
