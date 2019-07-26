using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace SmallEngine.Input
{
    public class InputState
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        internal static InputState CurrentState = new InputState(new byte[256]);
        internal static InputState PreviousState = new InputState(new byte[256]);

        //We keep a second copy of the input state
        //This way we can swap it when doing checking UI elements
        //We need to swap it because we mark keys as handled
        //If we didn't retain the original state certain methods like KeyPressed would always return true
        static InputState CurrentStateCopy = new InputState(new byte[256]);
        static InputState PreviousStateCopy = new InputState(new byte[256]);
        static byte[] _keyInput;

#if DEBUG
        static bool _swapped = false;
#endif

        internal static byte[] GetInput()
        {
            _keyInput = new byte[256];
            GetKeyboardState(_keyInput);
            return _keyInput;
        }

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

        protected static byte GetVirtualKeyCode(int pKey)
        {
            int value = pKey;
            return (byte)(value & 0xFF);
        }

        internal void Handle(Keys pKey)
        {
            var code = GetVirtualKeyCode((int)pKey);
            _keys[code] = 0;
        }

        internal void Handle(MouseButtons pButton)
        {
            var code = GetVirtualKeyCode((int)pButton);
            _keys[code] = 0;
        }

        public InputState Copy()
        {
            byte[] data = new byte[256];
            Array.Copy(_keys, data, 256);
            return new InputState(data);
        }

        #region Static methods
        public static InputState GetInputState()
        {
            return CurrentState;
        }

        internal static byte[] GetKeyState()
        {
            return _keyInput;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetState(byte[] pInput)
        {
            PreviousState = CurrentState;
            CurrentState = new InputState(pInput);

            PreviousStateCopy = CurrentStateCopy;
            CurrentStateCopy = CurrentState.Copy();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SwapUIStates()
        {
            var t = CurrentState;
            var t2 = PreviousState;
            CurrentState = CurrentStateCopy;
            PreviousState = PreviousStateCopy;
            CurrentStateCopy = t;
            PreviousStateCopy = t2;
#if DEBUG
            _swapped = !_swapped;
#endif
        }

        internal static void MarkKeyHandled(Keys pKey)
        {
            //We want to mark the original state as handled
            //This will cause non-UI elements to see the key as handled
            //UIElements will still have the original state so they know they handled the key
#if DEBUG 
            if (!_swapped) throw new InvalidOperationException();
#endif
            CurrentStateCopy.Handle(pKey);
            PreviousStateCopy.Handle(pKey);
        }

        internal static void MarkButtonHandled(MouseButtons pButton)
        {
            CurrentStateCopy.Handle(pButton);
            PreviousStateCopy.Handle(pButton);
        }
        #endregion
    }
}