using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public sealed class QueueingMessageBus : MessageBus
    {
        readonly ConcurrentQueue<IMessage> _messages;

        public QueueingMessageBus(int pThreads) : base(pThreads)
        {
            _messages = new ConcurrentQueue<IMessage>();
        }

        public sealed override void SendMessage(IMessage pM)
        {
            _messages.Enqueue(pM);
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
            return _messages.TryDequeue(out pMessage);
        }
    }
}
