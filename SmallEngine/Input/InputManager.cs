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
        static List<InputInfo> _keys = new List<InputInfo>();
        static IntPtr _handle;
        static InputState _inputState;
        static InputState _previousState;

        #region Win32 functions
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int pKey);
        public static bool IsKeyPressed(int pKey)
        {
            return 0 != (GetAsyncKeyState(pKey) & 0x8000);
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

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

        public static int MouseWheelDelta { get; private set; }

        private static long _holdDelay;
        public static int HoldDelay
        {
            get { return (int)GameTime.TickToMillis(_holdDelay); }
            set { _holdDelay = GameTime.MillisToTick(value); }
        }

        #endregion

        #region Constructor
        public static void Initialize(IntPtr pHandle)
        {
            _handle = pHandle;
            HoldDelay = 500;    //Default half second for holding a key

        }
        #endregion

        internal static void ProcessInput()
        {
            MouseWheelDelta = 0;

            var keyInput = new byte[256];
            GetKeyboardState(keyInput);

            var mouseInput = new byte[7];
            mouseInput[1] = IsKeyPressed((int)Mouse.Left) ? (byte)0xff : (byte)0x00;
            mouseInput[2] = IsKeyPressed((int)Mouse.Right) ? (byte)0xff : (byte)0x00;
            mouseInput[4] = IsKeyPressed((int)Mouse.Middle) ? (byte)0xff : (byte)0x00;
            mouseInput[5] = IsKeyPressed((int)Mouse.X1) ? (byte)0xff : (byte)0x00;
            mouseInput[6] = IsKeyPressed((int)Mouse.X2) ? (byte)0xff : (byte)0x00;

            _previousState = _inputState;
            _inputState = new InputState(keyInput, mouseInput);

            //Get mouse position
            GetCursorPos(out POINT p);
            ScreenToClient(_handle, ref p);
            _mousePos = new Vector2(p.X, p.Y);
            CheckDrag();
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
        #endregion
    }
}