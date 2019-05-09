using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public abstract class MessageBus
    {
        #region MessageThread
        struct MessageThread
        {
            readonly ManualResetEvent _reset;
            readonly Thread _thread;

            public MessageThread(ParameterizedThreadStart pThread)
            {
                _reset = new ManualResetEvent(false);
                _thread = new Thread(pThread);
            }

            public void Start()
            {
                _thread.Start(this);
            }

            public void Resume()
            {
                _reset.Set();
            }

            public void Wait()
            {
                _reset.Reset();
                _reset.WaitOne();
            }
        }
        #endregion

        protected readonly ConcurrentBag<WeakReference<IMessageReceiver>> _receivers;
        private volatile bool _processing;
        private readonly MessageThread[] _threads;

        public bool Suspended { get; private set; }

        protected MessageBus(int pThreads)
        {
            _receivers = new ConcurrentBag<WeakReference<IMessageReceiver>>();
            _threads = new MessageThread[pThreads];

        }

        public void Register(IMessageReceiver pReceiver)
        {
            _receivers.Add(new WeakReference<IMessageReceiver>(pReceiver));
        }

        public void Start()
        {
            _processing = true;
            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new MessageThread(ProcessMessages);
                _threads[i].Start();
            }
        }

        public void Stop()
        {
            _processing = false;
            for(int i= 0; i < _threads.Length; i++)
            {
                _threads[i].Resume();
            }
        }

        public void Suspend()
        {
            Suspended = true;
        }

        public void Resume()
        {
            Suspended = false;
            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i].Resume();
            }
        }

        private void ProcessMessages(object pThread)
        {
            var thread = (MessageThread)pThread;
            while(_processing)
            {
                if (!Suspended && TryGetNextMessage(out IMessage m))
                {
                    ProcessMessage(m);
                }
                else
                {
                    thread.Wait();
                }
            }
        }

        public virtual void SendMessage(IMessage pM)
        {
            if (!Suspended)
            {
                for (int i = 0; i < _threads.Length; i++)
                {
                    _threads[i].Resume();
                }
            }
        }
        protected abstract bool TryGetNextMessage(out IMessage pMessage);
        protected abstract void ProcessMessage(IMessage pMessage);
    }
}
