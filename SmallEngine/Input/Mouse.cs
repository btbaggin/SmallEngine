using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Input
{
    public static class Mouse
    {
        static IntPtr _handle;

        #region Win32
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Point lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        struct Point
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);
        #endregion

        #region Properties
        private static Vector2 _mousePos;
        public static Vector2 Position
        {
            get { return _mousePos; }
            internal set { _mousePos = value; }
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
        public static int WheelAbsolute
        {
            get { return _mouseWheel; }
            internal set
            {
                _mouseWheel += value;
                WheelDelta = value;
            }
        }

        public static int WheelDelta { get; internal set; }
        #endregion

        internal static void SetHandle(IntPtr pHandle)
        {
            _handle = pHandle;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static Vector2 GetMousePosition()
        {
            GetCursorPos(out Point p);
            ScreenToClient(_handle, ref p);
            return new Vector2(p.X, p.Y);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static void CheckDrag()
        {
            //Get mouse position

            CheckDrag(MouseButtons.Left);
            CheckDrag(MouseButtons.Right);
        }

        static Vector2 _dragStart;
        static Mode[] _modes = new Mode[6];
        private enum Mode : byte
        {
            Normal = 0,
            PossibleDrag,
            Drag,
            EndingDrag
        }

        private static void CheckDrag(MouseButtons pButton)
        {
            var i = (int)pButton;
            switch(_modes[i])
            {
                case Mode.Normal:
                    if (ButtonPressed(pButton))
                    {
                        _modes[i] = Mode.PossibleDrag;
                        _dragStart = _mousePos;
                    }
                    break;

                case Mode.PossibleDrag:
                    if (ButtonDown(pButton))
                    {
                        var _dragDistance = _mousePos - _dragStart;
                        if (Math.Abs(_dragDistance.X) > System.Windows.SystemParameters.MinimumHorizontalDragDistance ||
                            Math.Abs(_dragDistance.Y) > System.Windows.SystemParameters.MinimumVerticalDragDistance)
                        {
                            _modes[i] = Mode.Drag;
                        }
                    }
                    else
                    {
                        _modes[i] = Mode.Normal;
                    }
                    break;

                case Mode.Drag:
                    if (ButtonUp(pButton)) _modes[i] = Mode.EndingDrag;
                    break;

                case Mode.EndingDrag:
                    //EndingDrag allows for 1 frame after the mouse has been released until IsDragging returns false
                    //This prevents a mouse up event from happening every time the drag ends
                    _modes[i] = Mode.Normal;
                    break;
            }
        }

        #region Public functions
        public static bool ButtonPressed(MouseButtons pMouse)
        {
            return InputState.CurrentState.IsPressed(pMouse) && !InputState.PreviousState.IsPressed(pMouse);
        }

        public static bool ButtonReleased(MouseButtons pMouse)
        {
            return !InputState.CurrentState.IsPressed(pMouse) && InputState.PreviousState.IsPressed(pMouse);
        }

        public static bool ButtonDown(MouseButtons pMouse)
        {
            return InputState.CurrentState.IsPressed(pMouse);
        }

        public static bool ButtonUp(MouseButtons pMouse)
        {
            return !InputState.CurrentState.IsPressed(pMouse);
        }

        public static bool IsDragging(MouseButtons pButton, out Vector2 pAnchor)
        {
            var i = (int)pButton;
            pAnchor = _dragStart;
            return _modes[i] == Mode.Drag || _modes[i] == Mode.EndingDrag;
        }
        #endregion
    }
}
