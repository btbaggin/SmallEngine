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
        readonly byte[] _keys;

        internal InputState(byte[] pKeys)
        {
            _keys = pKeys;
        }

        public bool IsPressed(Keys pKey)
        {

            var code = GetVirtualKeyCode((int)pKey);
            return (_keys[code] & 0x80) != 0;
        }

        public bool IsPressed(MouseButtons pMouse)
        {
            var code = GetVirtualKeyCode((int)pMouse);
            return (_keys[code] & 0x80) != 0;
        }

        private static byte GetVirtualKeyCode(int pKey)
        {
            int value = pKey;
            return (byte)(value & 0xFF);
        }
    }
}