using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public struct Timer
    {
        public float TickTime { get; set; }

        private float _timer;
        public Timer(float pTime)
        {
            TickTime = pTime;
            _timer = pTime;
        }

        public bool Tick()
        {
            if((_timer -= GameTime.DeltaTime) <= 0)
            {
                _timer += TickTime;
                return true;
            }

            return false;
        }
    }
}
