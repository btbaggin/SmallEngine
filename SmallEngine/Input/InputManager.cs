using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SmallEngine.Input
{
    public class InputManager
    {
        //TODO add if mouse is dragging
        private static List<InputInfo> _keys;
        private IntPtr _handle;
        private static InputState _inputState;
        private static InputState _previousState;

        #region Win32 functions
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys pKey);
        public bool IsKeyPressed(Keys pKey)
        {
            return 0 != (GetAsyncKeyState(pKey) & 0x8000);
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Mouse pMouse);
        public bool IsMousePressed(Mouse pMouse)
        {
            return 0 != (GetAsyncKeyState(pMouse) & 0x8000);
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int X, int Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);
        #endregion

        #region Properties
        private static Vector2 _mousePos;
        public static Vector2 MousePosition
        {
            get { return _mousePos; }
        }

        private static bool _cursorShown;
        public static bool CursorShown
        {
            get { return _cursorShown; }
            set
            {
                if (value != _cursorShown)
                {
                    ShowCursor(value);
                    _cursorShown = value;
                }
            }
        }

        private static int _mouseWheel;
        public static int MouseWheel
        {
            get { return _mouseWheel; }
            internal set
            {
                _mouseWheel += value;
                MouseWheelDelta = value;
            }
        }

        private static int _mouseWheelDelta;
        public static int MouseWheelDelta
        {
            get { return _mouseWheelDelta; }
            internal set { _mouseWheelDelta = value; }
        }

        private static long _holdDelay;
        public static int HoldDelay
        {
            get { return (int)GameTime.TickToMillis(_holdDelay); }
            set { _holdDelay = GameTime.MillisToTick(value); }
        }

        #endregion

        #region Constructor
        public InputManager(IntPtr pHandle)
        {
            _keys = new List<InputInfo>();
            _handle = pHandle;
            HoldDelay = 500;    //Default half second for holding a key

        }
        #endregion

        internal void ProcessInput()
        {
            _previousState = _inputState;
            _inputState = new InputState();
            foreach (var ii in _keys)
            {
                var i = ii;
                GetStatus(ref i, out bool pressed, out bool held);
                if (pressed) { _inputState.AddPressed(ii); }
                if (held) { _inputState.AddHeld(ii); }
            }

            //Get mouse position
            GetCursorPos(out POINT p);
            ScreenToClient(_handle, ref p);
            _mousePos = new Vector2(p.X, p.Y);
        }

        private void GetStatus(ref InputInfo pKey, out bool pPressed, out bool pHeld)
        {
            var time = GameTime.CurrentTime;
            var keyPressed = pKey.Key.HasValue ? IsKeyPressed(pKey.Key.Value) : IsMousePressed(pKey.Mouse.Value);

            //Pressed if the key is down and enough time has passed since it was last registered
            pPressed = keyPressed && time > pKey.LastRegistered + pKey.Delay;
            //Held if its pressed, its currently begin held, and its been held for long enough to trigger hold
            pHeld = (keyPressed && pKey.LastReleased < pKey.LastPressed && time > pKey.LastPressed + _holdDelay);   //TODO can we get this from windows?

            //If status has changed set variables
            if (pKey.IsPressed != keyPressed)
            {
                if(keyPressed) { pKey.LastPressed = time; } else { pKey.LastReleased = time; }
                pKey.IsPressed = keyPressed;
            }

            //Set when the last registered action was
            if (pPressed || pHeld)
            {
                pKey.LastRegistered = time;
            }
        }

        #region Public functions
        public static InputState GetInputState()
        {
            return _inputState;
        }

        public static bool KeyPressed(Keys pKey)
        {
            return _inputState.IsPressed(pKey) && !_previousState.IsPressed(pKey);
        }

        public static bool KeyPressed(Mouse pMouse)
        {
            return _inputState.IsPressed(pMouse) && !_previousState.IsPressed(pMouse);
        }

        public static bool KeyReleased(Keys pKey)
        {
            return !_inputState.IsPressed(pKey) && _previousState.IsPressed(pKey);
        }

        public static bool KeyReleased(Mouse pMouse)
        {
            return !_inputState.IsPressed(pMouse) && _previousState.IsPressed(pMouse);
        }

        public static bool KeyDown(Keys pKey)
        {
            return _inputState.IsPressed(pKey);
        }

        public static bool KeyDown(Mouse pMouse)
        {
            return _inputState.IsPressed(pMouse);
        }

        public static bool KeyUp(Keys pKey)
        {
            return !_inputState.IsPressed(pKey);
        }

        public static bool KeyUp(Mouse pMouse)
        {
            return !_inputState.IsPressed(pMouse);
        }

        public static void Listen(Keys pKey)
        {
            if(!CheckExists(pKey))
            {
                _keys.Add(new InputInfo(pKey, 0));
            }
        }

        public static void Listen(Mouse pMouse)
        {
            if(!CheckExists(pMouse))
            {
                _keys.Add(new InputInfo(pMouse, 0));
            }
        }

        private static bool CheckExists(Keys pKey)
        {
            foreach(InputInfo k in _keys)
            {
                if(k.Key.HasValue && k.Key.Value == pKey)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckExists(Mouse pMouse)
        {
            foreach (InputInfo k in _keys)
            {
                if (k.Mouse.HasValue && k.Mouse.Value == pMouse)
                {
                    return true;
                }
            }
            return false;
        }

        public static void StopListening(Keys pKey)
        {
            InputInfo toRemove = new InputInfo();
            bool found = false;
            foreach (var ii in _keys)
            {
                if (ii.Key.HasValue && ii.Key == pKey)
                {
                    toRemove = ii;
                    found = true;
                    break;
                }
            }

            if (found) _keys.Remove(toRemove);
        }

        public static void StopListening(Mouse pMouse)
        {
            InputInfo toRemove = new InputInfo();
            bool found = false;
            foreach (var ii in _keys)
            {
                if (ii.Mouse.HasValue && ii.Mouse == pMouse)
                {
                    toRemove = ii;
                    found = true;
                    break;
                }
            }

            if (found) _keys.Remove(toRemove);
        }
        #endregion
    }
}
