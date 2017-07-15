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
        private static List<InputInfo> _keys;
        private IntPtr _handle;
        private static InputState _inputState;
        private static InputState _previousState;

        #region Win32 functions
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int pKey);
        public bool IsKeyPressed(int pKey)
        {
            return 0 != (GetAsyncKeyState(pKey) & 0x8000);
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
            CheckDrag();
        }

        private void GetStatus(ref InputInfo pKey, out bool pPressed, out bool pHeld)
        {
            var time = GameTime.CurrentTime;
            var keyPressed = IsKeyPressed(pKey.Value);

            //Pressed if the key is down and enough time has passed since it was last registered
            pPressed = keyPressed && time > pKey.LastPressed + pKey.Delay;
            //Held if its pressed, its currently begin held, and its been held for long enough to trigger hold
            pHeld = (keyPressed && _previousState.IsPressed(pKey.Value) && time > pKey.LastPressed + _holdDelay);   //TODO can we get this from windows?

            //If status has changed set variables
            if (pKey.IsPressed != keyPressed)
            {
                if (keyPressed)
                {
                    pKey.LastPressed = time;
                }
                pKey.IsPressed = keyPressed;
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

        public static bool IsDragging(Mouse pMouse)
        {
            return _mode == Mode.Drag;
        }

        private static Vector2 _dragStart;
        private static Mode _mode = Mode.Normal;
        private enum Mode
        {
            PossibleDrag,
            Drag,
            Normal
        }

        private static void CheckDrag()
        {
            if (_mode == Mode.Normal && KeyPressed(Mouse.Left))
            {
                _mode = Mode.PossibleDrag;
                _dragStart = _mousePos;
            }

            if (_mode == Mode.PossibleDrag && KeyDown(Mouse.Left))
            {
                var _dragDistance = _mousePos - _dragStart;
                if (Math.Abs(_dragDistance.X) > System.Windows.SystemParameters.MinimumHorizontalDragDistance ||
                   Math.Abs(_dragDistance.Y) > System.Windows.SystemParameters.MinimumVerticalDragDistance)
                {
                    _mode = Mode.Drag;
                }
            }
            else if (KeyUp(Mouse.Left))
            {
                _mode = Mode.Normal;
            }
        }

        public static void Listen(Keys pKey)
        {
            if (!CheckExists(pKey))
            {
                _keys.Add(new InputInfo(pKey, 0));
            }
        }

        public static void Listen(Mouse pMouse)
        {
            if (!CheckExists(pMouse))
            {
                _keys.Add(new InputInfo(pMouse, 0));
            }
        }

        private static bool CheckExists(Keys pKey)
        {
            foreach (InputInfo k in _keys)
            {
                if (k.Value == (int)pKey)
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
                if (k.Value == (int)pMouse)
                {
                    return true;
                }
            }
            return false;
        }

        public static void StopListening(Keys pKey)
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                if (_keys[i].Value == (int)pKey)
                {
                    _keys.RemoveAt(i);
                }
            }
        }

        public static void StopListening(Mouse pMouse)
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                if (_keys[i].Value == (int)pMouse)
                {
                    _keys.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}