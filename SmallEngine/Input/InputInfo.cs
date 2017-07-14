using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Input
{
    struct InputInfo
    {
        public Int32 Value;
        public bool IsPressed;
        public long LastPressed;
        public long Delay;

        public InputInfo(Keys pKey, long pDelay)
        {
            Value = (int)pKey;
            IsPressed = false;
            LastPressed = 0;
            Delay = pDelay;
        }

        public InputInfo(Mouse pMouse, long pDelay)
        {
            Value = (int)pMouse;
            IsPressed = false;
            LastPressed = 0;
            Delay = pDelay;
        }

        public Keys Key { get { return (Keys)Value; } }

        public Mouse Mouse { get { return (Mouse)Value; } }
    }
}