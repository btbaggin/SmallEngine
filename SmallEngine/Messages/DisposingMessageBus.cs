using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public sealed class DisposingMessageBus : MessageBus
    {
        readonly RingBuffer<IMessage> _messages;

        public DisposingMessageBus(int pCapacity) : base()
        {
            _messages = new RingBuffer<IMessage>(pCapacity);
        }

        public sealed override void SendMessage(IMessage pM)
        {
            _messages.Push(pM);
            ResumeProcessing();
        }

        protected sealed override void ProcessMessage(IMessage pMessage)
        {
            foreach (var l in _receivers)
            {
                if (l.TryGetTarget(out IMessageReceiver receiver))
                {
                    receiver.ReceiveMessage(pMessage);
                }
            }
        }

        protected sealed override bool TryGetNextMessage(out IMessage pMessage)
        {
            if(!_messages.IsEmpty)
            {
                pMessage = _messages.Pop();
                return true;
            }

            pMessage = default(IMessage);
            return false;
        }
    }
}
