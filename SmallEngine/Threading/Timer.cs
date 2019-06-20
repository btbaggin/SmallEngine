using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Threading
{
    public struct Timer
    {
        public float Interval { get; set; }

        private float _timer;
        public Timer(float pTime)
        {
            Interval = pTime;
            _timer = pTime;
        }

        public bool Tick()
        {
            if((_timer -= GameTime.DeltaTime) <= 0)
            {
                _timer += Interval;
                return true;
            }

            return false;
        }
    }
}
