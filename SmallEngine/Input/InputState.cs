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
            _pressedKeys.Add(pI.Value, pI);
        }

        internal void AddHeld(InputInfo pI)
        {
            _heldKeys.Add(pI.Value, pI);
        }

        public bool IsPressed(Keys pKey)
        {
            return _pressedKeys.ContainsKey((int)pKey);
        }

        internal bool IsPressed(int pValue)
        {
            return _pressedKeys.ContainsKey(pValue);
        }

        public bool IsHeld(Keys pKey)
        {
            return _heldKeys.ContainsKey((int)pKey);
        }

        public bool IsPressed(Mouse pMouse)
        {
            return _pressedKeys.ContainsKey((int)pMouse);
        }

        public bool IsHeld(Mouse pMouse)
        {
            return _heldKeys.ContainsKey((int)pMouse);
        }
    }
}