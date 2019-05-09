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

        public DisposingMessageBus(int pCapacity, int pThreads) : base(pThreads)
        {
            _messages = new RingBuffer<IMessage>(pCapacity);
        }

        public sealed override void SendMessage(IMessage pM)
        {
            _messages.Push(pM);
            base.SendMessage(pM);
        }

        protected sealed override void ProcessMessage(IMessage pMessage)
        {
            if(pMessage.Recipient == null)
            {
                foreach (var l in _receivers)
                {
                    if (l.TryGetTarget(out IMessageReceiver receiver))
                    {
                        receiver.ReceiveMessage(pMessage);
                    }
                }
            }
            else
            {
                pMessage.Recipient.ReceiveMessage(pMessage);
            }
        }

        protected sealed override bool TryGetNextMessage(out IMessage pMessage)
        {
            lock(_messages)
            {
                if (!_messages.IsEmpty)
                {
                    pMessage = _messages.Pop();
                    return true;
                }
            }

            pMessage = default(IMessage);
            return false;
        }
    }
}
