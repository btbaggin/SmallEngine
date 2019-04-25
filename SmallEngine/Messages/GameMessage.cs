using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public struct GameMessage : IMessage
    {
        public string Type { get; private set; }
        private readonly object _value;

        public GameMessage(string pMessageType, object pValue)
        {
            Type = pMessageType;
            _value = pValue;
        }

        public T GetData<T>()
        {
            return (T)_value;
        }
    }
}
