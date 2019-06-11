﻿using System;
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
        static InputState _inputState = new InputState(new byte[7]);
        static InputState _previousState = new InputState(new byte[7]);

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

        public static int WheelDelta { get; private set; }
        #endregion

        internal static void SetHandle(IntPtr pHandle)
        {
            _handle = pHandle;
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static void SetState(byte[] pInput)
        {
            _previousState = _inputState;
            _inputState = new InputState(pInput);

            //Get mouse position
            WheelDelta = 0;
            GetCursorPos(out Point p);
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
            if (_mode == Mode.Normal && ButtonPressed(MouseButtons.Left))
            {
                _mode = Mode.PossibleDrag;
                _dragStart = _mousePos;
            }

            if (_mode == Mode.PossibleDrag && ButtonDown(MouseButtons.Left))
            {
                var _dragDistance = _mousePos - _dragStart;
                if (Math.Abs(_dragDistance.X) > System.Windows.SystemParameters.MinimumHorizontalDragDistance ||
                   Math.Abs(_dragDistance.Y) > System.Windows.SystemParameters.MinimumVerticalDragDistance)
                {
                    _mode = Mode.Drag;
                }
            }
            else if (ButtonUp(MouseButtons.Left))
            {
                _mode = Mode.Normal;
            }
        }

        #region Public functions
        public static InputState GetInputState()
        {
            return _inputState;
        }

        public static bool ButtonPressed(MouseButtons pMouse)
        {
            return _inputState.IsPressed(pMouse) && !_previousState.IsPressed(pMouse);
        }

        public static bool ButtonReleased(MouseButtons pMouse)
        {
            return !_inputState.IsPressed(pMouse) && _previousState.IsPressed(pMouse);
        }

        public static bool ButtonDown(MouseButtons pMouse)
        {
            return _inputState.IsPressed(pMouse);
        }

        public static bool ButtonUp(MouseButtons pMouse)
        {
            return !_inputState.IsPressed(pMouse);
        }

        public static bool IsDragging(MouseButtons pMouse)
        {
            return _mode == Mode.Drag;
        }
        #endregion
    }
}
