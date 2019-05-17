using System.Collections.Concurrent;

namespace SmallEngine.Messages
{
    /// <summary>
    /// Message bus that will only process one message of a type at a time
    /// If a message of the same type is sent before one is processed, it will be dropped
    /// </summary>
    public sealed class GroupingMessageBus : MessageBus
    {

        readonly ConcurrentQueue<IMessage> _messages;

        public GroupingMessageBus(int pThreads) : base(pThreads)
        {
            _messages = new ConcurrentQueue<IMessage>();
        }

        public sealed override void SendMessage(IMessage pM)
        {
            if(_messages.Count == 0 || (_messages.TryPeek(out IMessage m) && m.Type != pM.Type))
            {
                _messages.Enqueue(pM);
                base.SendMessage(pM);
            }
        }

        protected sealed override void ProcessMessage(IMessage pMessage)
        {
            if (pMessage.Recipient == null)
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
