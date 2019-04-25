using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public class UnknownEnumException : Exception
    {
        public UnknownEnumException(Type pType, object pValue) : base("Enum " + pType.Name + " does not support value " + pValue)
        {
        }
    }
}
