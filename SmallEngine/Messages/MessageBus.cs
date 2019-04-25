using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public abstract class MessageBus
    {
        protected readonly List<WeakReference<IMessageReceiver>> _receivers;
        private readonly ManualResetEvent _mre;
        private volatile bool _processing;

        public bool Suspended { get; private set; }

        protected MessageBus()
        {
            _receivers = new List<WeakReference<IMessageReceiver>>();
            _mre = new ManualResetEvent(false);
        }

        public void Register(IMessageReceiver pReceiver)
        {
            _receivers.Add(new WeakReference<IMessageReceiver>(pReceiver));
        }

        public abstract void SendMessage(IMessage pM);

        protected void ResumeProcessing()
        {
            if(!Suspended) _mre.Set();
        }

        public void Start()
        {
            _processing = true;
            var thread = new Thread(ProcessMessages);
            thread.Start();
        }

        public void Stop()
        {
            _processing = false;
            _mre.Set();
        }

        public void Suspend()
        {
            Suspended = true;
        }

        public void Resume()
        {
            Suspended = false;
            _mre.Set();
        }

        private void ProcessMessages()
        {
            while(_processing)
            {
                if (!Suspended && TryGetNextMessage(out IMessage m))
                {
                    ProcessMessage(m);
                }
                else
                {
                    _mre.Reset();
                    _mre.WaitOne();
                }
            }
        }

        protected abstract bool TryGetNextMessage(out IMessage pMessage);
        protected abstract void ProcessMessage(IMessage pMessage);
    }
}
