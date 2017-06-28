using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Input
{
    class InputState
    {
        private Hashtable _pressedKeys;
        private Hashtable _heldKeys;

        internal InputState()
        {
            _pressedKeys = new Hashtable();
            _heldKeys = new Hashtable();
        }

        internal void AddPressed(string pName, InputInfo pI)
        {
            _pressedKeys.Add(pName, pI);
        }

        internal void AddHeld(string pName, InputInfo pI)
        {
            _heldKeys.Add(pName, pI);
        }

        public bool IsPressed(string pName)
        {
            return _pressedKeys.ContainsKey(pName);
        }

        public bool IsHeld(string pName)
        {
            return _heldKeys.ContainsKey(pName);
        }
    }
}
