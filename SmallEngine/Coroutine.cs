using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SmallEngine
{
    #region Coroutine wait classes
    public abstract class WaitEvent
    {
        public abstract bool Update(float pDeltaTime);
    }

    public class WaitForSeconds : WaitEvent
    {
        private float _timer;
        private CancellationToken _token;
        /// <summary>
        /// An object that can be yielded to wait for pSeconds before continuing
        /// </summary>
        /// <param name="pSeconds">Number of seconds to wait</param>
        public WaitForSeconds(float pSeconds)
        {
            _timer = pSeconds;
        }

        public WaitForSeconds(float pSeconds, CancellationToken pToken)
        {
            _timer = pSeconds;
            _token = pToken;
        }

        public override bool Update(float pDeltaTime)
        {
            _token.ThrowIfCancellationRequested();
            _timer -= pDeltaTime;
            return _timer <= 0f;
        }
    }

    public class WaitForFrames : WaitEvent
    {
        int _frames;
        /// <summary>
        /// An object that can be yielded to wait for the next frame before continuing
        /// </summary>
        public WaitForFrames(int pFrames)
        {
            _frames = pFrames;
        }

        public override bool Update(float pDeltaTime)
        {
            return (--_frames) <= 0;
        }
    }

    public class WaitForEvent : WaitEvent
    {
        internal bool Triggered { get; private set; }

        /// <summary>
        /// An object that can be yielded to wait until the event is triggered before triggering
        /// </summary>
        /// <param name="pSender">Object that will be sending the event</param>
        /// <param name="pEvent">Event to wait for.  Must take an Object and EventArgs as parameters</param>
        public WaitForEvent(object pSender, string pEvent)
        {
            var t = pSender.GetType();
            var ei = t.GetEvent(pEvent);

            var handler = Delegate.CreateDelegate(ei.EventHandlerType, this, "Trigger");

            ei.AddEventHandler(pSender, handler);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Trigger(object sender, object e)
        {
            Triggered = true;
        }

        public override bool Update(float pDeltaTime)
        {
            return Triggered;
        }
    }
    #endregion

    //http://twistedoakstudios.com/blog/Post83_coroutines-more-than-you-want-to-know
    public static class Coroutine
    {
        private static List<IEnumerator<WaitEvent>> _coroutines = new List<IEnumerator<WaitEvent>>();

        /// <summary>
        /// Start a coroutine
        /// </summary>
        /// <param name="pCoroutine">Coroutine to start</param>
        public static void Start(Func<IEnumerator<WaitEvent>> pCoroutine)
        {
            var e = pCoroutine.Invoke();
            if (e.MoveNext())
            {
                _coroutines.Add(e);
            }
        }

        public static void Start(Func<object, IEnumerator<WaitEvent>> pCoroutine, object pState)
        {
            var e = pCoroutine.Invoke(pState);
            if(e.MoveNext())
            {
                _coroutines.Add(e);
            }
        }

        internal static void Update(float pDeltaTime)
        {
            foreach (IEnumerator<WaitEvent> e in _coroutines.ToList())
            { 
                try
                {
                    if (e.Current.Update(pDeltaTime) && !e.MoveNext())
                    {
                        _coroutines.Remove(e);
                    }
                }catch(OperationCanceledException)
                {
                    _coroutines.Remove(e);
                }
                
            }
        }
    }
}
