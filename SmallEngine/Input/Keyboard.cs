using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Input
{
    public static class Keyboard
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        static InputState _inputState = new InputState(new byte[256]);
        static InputState _previousState = new InputState(new byte[256]);
        static byte[] _keyInput;

        internal static void ProcessInput()
        {
            _keyInput = new byte[256];
            GetKeyboardState(_keyInput);

            _previousState = _inputState;
            _inputState = new InputState(_keyInput);
        }

        #region KeyMapping exposure
        static KeyMapping _mapping = new KeyMapping();
        public static void AddMapping(string pName, Keys pKey)
        {
            _mapping.AddMapping(pName, pKey);
        }

        public static Keys GetKey(string pName)
        {
            return _mapping.GetKey(pName);
        }

        public static bool KeyPressed(string pName)
        {
            return _mapping.IsPressed(pName);
        }

        public static bool KeyDown(string pName)
        {
            return _mapping.KeyDown(pName);
        }

        public static bool KeyUp(string pName)
        {
            return _mapping.KeyUp(pName);
        }
        #endregion

        public static bool KeyPressed(Keys pKey)
        {
            return _inputState.IsPressed(pKey) && !_previousState.IsPressed(pKey);
        }

        public static bool KeyReleased(Keys pKey)
        {
            return !_inputState.IsPressed(pKey) && _previousState.IsPressed(pKey);
        }

        public static bool KeyDown(Keys pKey)
        {
            return _inputState.IsPressed(pKey);
        }

        public static bool KeyUp(Keys pKey)
        {
            return !_inputState.IsPressed(pKey);
        }

        public static bool AnyKeyPressed(out Keys pPressed)
        {
            for(int i = (int)Keys.Backspace; i < 256; i++)
            {
                if (KeyPressed((Keys)i))
                {
                    pPressed = (Keys)i;
                    return true;
                }
            }

            pPressed = 0;
            return false;
        }

        public static InputState GetInputState()
        {
            return _inputState;
        }

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags);

        public static string ToUnicode(Keys pKey)
        {
            StringBuilder sb = new StringBuilder();
            uint lScanCode = MapVirtualKey((uint)pKey, 0);
            var result = ToUnicode((uint)pKey, lScanCode, _keyInput, sb, 5, 0);
            if(result == 1) return sb.ToString();

            return "";
        }
    }
}
