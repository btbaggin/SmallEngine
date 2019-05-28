using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public class SingleUseMessage : IMessage
    {
        public IMessageReceiver Recipient { get; }

        public IMessageReceiver Sender { get; }

        public string Type { get; }

        readonly object _data;
        volatile int _valid;
        public SingleUseMessage(string pType, IMessageReceiver pRecipient, IMessageReceiver pSender, object pData)
        {
            Type = pType;
            Recipient = pRecipient;
            Sender = pSender;
            _data = pData;
            _valid = 1;
        }

        public bool Consume()
        {
            return System.Threading.Interlocked.CompareExchange(ref _valid, 0, 1) == 1;
        }

        public T GetData<T>()
        {
            return (T)_data;
        }
    }
}
