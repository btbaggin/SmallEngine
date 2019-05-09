using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public struct GameMessage : IMessage
    {
        public IMessageReceiver Recipient { get; private set; }

        public IMessageReceiver Sender { get; private set; }

        public string Type { get; private set; }

        private readonly object _value;

        public GameMessage(string pMessageType, object pValue, IMessageReceiver pSender) : this(pMessageType, pValue, pSender, null) { }

        public GameMessage(string pMessageType, object pValue, IMessageReceiver pSender, IMessageReceiver pRecipient)
        {
            Type = pMessageType;
            _value = pValue;
            Recipient = pRecipient;
            Sender = pSender;
        }

        public T GetData<T>()
        {
            return (T)_value;
        }
    }
}
