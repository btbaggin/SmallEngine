using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    [Serializable]
    public class UnknownEnumException : Exception
    {
        public UnknownEnumException(Type pType, object pValue) : base("Enum " + pType.Name + " does not support value " + pValue)
        {
        }

        protected UnknownEnumException(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
        }
    }
}
