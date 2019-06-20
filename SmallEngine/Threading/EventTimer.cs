using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Threading
{
    public class TimerElapsedEventArgs : EventArgs
    {

    }

    public struct EventTimer
    {
        public EventHandler<TimerElapsedEventArgs> Elapsed { get; set; }
        public float Interval { get; set; }

        private float _timer;
        public EventTimer(float pTime, EventHandler<TimerElapsedEventArgs> pElapsed)
        {
            Interval = pTime;
            Elapsed = pElapsed;
            _timer = pTime;
        }

        public void Tick()
        {
            if ((_timer -= GameTime.DeltaTime) <= 0)
            {
                _timer += Interval;
                Elapsed.Invoke(this, new TimerElapsedEventArgs());
             }
        }
    }
}
