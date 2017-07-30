using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmallEngine
{
    public class MessageBus
    {
        private static List<WeakReference<IMessageReceiver>> _receivers;
        private static Queue<GameMessage> _messages;
        private static ManualResetEvent _mre;
        private Thread _thread;
        private volatile bool _processing;

        public MessageBus()
        {
            _receivers = new List<WeakReference<IMessageReceiver>>();
            _messages = new Queue<GameMessage>();
            _mre = new ManualResetEvent(false);
        }

        public static void Register(IMessageReceiver pReceiver)
        {
            _receivers.Add(new WeakReference<IMessageReceiver>(pReceiver));
        }

        public static void SendMessage(GameMessage pM)
        {
            _messages.Enqueue(pM);
            _mre.Set();
        }

        internal void Start()
        {
            _processing = true;
            _thread = new Thread(ProcessMessages);
            _thread.Start();
        }

        internal void Stop()
        {
            _processing = false;
            _mre.Set();
        }

        private void ProcessMessages()
        {
            while(_processing)
            {
                if (_messages.Count > 0)
                {
                    var m = _messages.Dequeue();
                    foreach (var l in _receivers)
                    {
                        if (l.TryGetTarget(out IMessageReceiver receiver))
                        {
                            receiver.ReceiveMessage(m);
                        }
                    }
                }
                else
                {
                    _mre.Reset();
                    _mre.WaitOne();
                }
            }
        }
    }

    public struct GameMessage
    {
        public string MessageType { get; private set; }
        private object _value;

        public GameMessage(string pMessageType, object pValue)
        {
            MessageType = pMessageType;
            _value = pValue;
        }

        public T GetValue<T>()
        {
            return (T)_value;
        }
    }
}
