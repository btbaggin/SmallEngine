using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    /// <summary>
    /// Message bus that will dispose any messages after it has reached it's Capacity
    /// It will also group like messages together and only process one message of a type at a time
    /// </summary>
    public sealed class DisposingMessageBus : MessageBus
    {
        readonly RingBuffer<IMessage> _messages;
        public int Capacity { get; private set; }

        public DisposingMessageBus(int pCapacity, int pThreads) : base(pThreads)
        {
            Capacity = pCapacity;
            _messages = new RingBuffer<IMessage>(pCapacity);
        }

        public sealed override void SendMessage(IMessage pM)
        {
            if(_messages.IsEmpty || _messages.Peek().Type != pM.Type)
            {
                _messages.Push(pM);
                base.SendMessage(pM);
            }
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
