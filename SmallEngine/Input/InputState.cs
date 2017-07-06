using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Input
{
    public class InputState
    {
        private Hashtable _pressedKeys;
        private Hashtable _heldKeys;

        internal InputState()
        {
            _pressedKeys = new Hashtable();
            _heldKeys = new Hashtable();
        }

        internal void AddPressed(InputInfo pI)
        {
            if (pI.Key.HasValue)
            {
                _pressedKeys.Add(pI.Key, pI);
            }
            else
            {
                _pressedKeys.Add(pI.Mouse, pI);
            }
        }

        internal void AddHeld(InputInfo pI)
        {
            if(pI.Key.HasValue)
            {
                _heldKeys.Add(pI.Key, pI);
            }
            else
            {
                _heldKeys.Add(pI.Mouse, pI);
            }
        }

        public bool IsPressed(Keys pKey)
        {
            return _pressedKeys.ContainsKey(pKey);
        }

        public bool IsHeld(Keys pKey)
        {
            return _heldKeys.ContainsKey(pKey);
        }

        public bool IsPressed(Mouse pMouse)
        {
            return _pressedKeys.ContainsKey(pMouse);
        }

        public bool IsHeld(Mouse pMouse)
        {
            return _heldKeys.ContainsKey(pMouse);
        }
    }
}
