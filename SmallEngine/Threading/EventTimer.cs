using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Threading
{
    public class TimerElapsedEventArgs : EventArgs { }

    public struct EventTimer
    {
        public EventHandler<TimerElapsedEventArgs> Elapsed { get; set; }
        float _interval;
        public float Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                Time = value;
            }
        }

        public float Time { get; private set; }

        public EventTimer(float pTime, EventHandler<TimerElapsedEventArgs> pElapsed)
        {
            Elapsed = pElapsed;
            _interval = pTime;
            Time = pTime;
        }

        public void Tick()
        {
            if ((Time -= GameTime.DeltaTime) <= 0)
            {
                Time += Interval;
                Elapsed.Invoke(this, new TimerElapsedEventArgs());
            }
        }

        public void Reset()
        {
            Time = _interval;
        }
    }
}
