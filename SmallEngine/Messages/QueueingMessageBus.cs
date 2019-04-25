using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public sealed class QueueingMessageBus : MessageBus
    {
        readonly Queue<IMessage> _messages;

        public QueueingMessageBus() : base()
        {
            _messages = new Queue<IMessage>();
        }

        public sealed override void SendMessage(IMessage pM)
        {
            _messages.Enqueue(pM);
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
            if(_messages.Count > 0)
            {
                pMessage = _messages.Dequeue();
                return true;
            }

            pMessage = default(GameMessage);
            return false;
        }
    }
}
