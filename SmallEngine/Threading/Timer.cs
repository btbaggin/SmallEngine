using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Threading
{
    [Serializable]
    public struct Timer
    {
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

        public Timer(float pTime)
        {
            _interval = pTime;
            Time = pTime;
        }

        public bool Tick()
        {
            if((Time -= GameTime.DeltaTime) <= 0)
            {
                Time += Interval;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            Time = _interval;
        }
    }
}
