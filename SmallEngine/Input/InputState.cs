﻿using System;
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
        readonly byte[] _mouse;

        internal InputState(byte[] pKeys, byte[] pMouse)
        {
            _keys = pKeys;
            _mouse = pMouse;
        }

        public bool IsPressed(Keys pKey)
        {

            var code = GetVirtualKeyCode(pKey);
            return (_keys[code] & 0x80) != 0;
        }

        public bool IsPressed(Mouse pMouse)
        {
            var code = GetVirtualKeyCode(pMouse);
            return (_mouse[code] & 0x80) != 0;
        }

        internal static byte GetVirtualKeyCode(Keys pKey)
        {
            int value = (int)pKey;
            return (byte)(value & 0xFF);
        }

        internal static byte GetVirtualKeyCode(Mouse pMouse)
        {
            int value = (int)pMouse;
            return (byte)(value & 0xFF);
        }
    }
}