using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Input
{
    struct InputInfo
    {
        public Keys? Key;
        public Mouse? Mouse;
        public bool IsPressed;
        public long LastPressed;
        public long LastRegistered;
        public long LastReleased;
        public long Delay;

        public InputInfo(Keys pKey, long pDelay)
        {
            Key = pKey;
            Mouse = null;
            IsPressed = false;
            LastPressed = 0;
            LastRegistered = 0;
            LastReleased = 0;
            Delay = pDelay;
        }

        public InputInfo(Mouse pMouse, long pDelay)
        {
            Mouse = pMouse;
            Key = null;
            IsPressed = false;
            LastPressed = 0;
            LastRegistered = 0;
            LastReleased = 0;
            Delay = pDelay;
        }
    }
}
