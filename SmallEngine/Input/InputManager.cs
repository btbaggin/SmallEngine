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
        private static Dictionary<string, InputInfo> _mapping;
        private IntPtr _handle;
        private static InputState _inputState;

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

        private static int _mouseDelta;
        public static int MouseDelta
        {
            get { return _mouseDelta; }
            internal set { _mouseDelta += value; }
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
            _handle = pHandle;
            _mapping = new Dictionary<string, InputInfo>();
            HoldDelay = 500;    //Default half second for holding a key

        }
        #endregion

        internal void ProcessInput()
        {
            _inputState = new InputState();
            foreach (string name in _mapping.Keys.ToList())
            {
                InputInfo ii = _mapping[name];
                GetStatus(ref ii, out bool pressed, out bool held);
                if (pressed) { _inputState.AddPressed(name, ii); }
                if (held) { _inputState.AddHeld(name, ii); }
                _mapping[name] = ii;
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

        public static void AddMapping(string pName, Keys pKey)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(pName));
            if (!_mapping.ContainsKey(pName))
            {
                _mapping.Add(pName, new InputInfo(pKey, 0));
            }
        }

        public static void AddMapping(string pName, Keys pKey, int pDelay)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(pName));
            if (!_mapping.ContainsKey(pName))
            {
                _mapping.Add(pName, new InputInfo(pKey, GameTime.MillisToTick(pDelay)));
            }
        }

        public static void AddMapping(string pName, Mouse pMouse)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(pName));
            if(!_mapping.ContainsKey(pName))
            {
                _mapping.Add(pName, new InputInfo(pMouse, 0));
            }
        }

        public static void AddMapping(string pName, Mouse pMouse, int pDelay)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(pName));
            if (!_mapping.ContainsKey(pName))
            {
                _mapping.Add(pName, new InputInfo(pMouse, GameTime.MillisToTick(pDelay)));
            }
        }

        public static void SetDelay(string pName, int pDelay)
        {
            System.Diagnostics.Debug.Assert(_mapping.ContainsKey(pName));
            InputInfo ii = _mapping[pName];
            ii.Delay = GameTime.MillisToTick(pDelay);
            _mapping[pName] = ii;
        }

        public static void UpdateMapping(string pName, Keys pKey)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(pName));
            if(_mapping.ContainsKey(pName))
            {
                InputInfo ii = _mapping[pName];
                ii.Key = pKey;
                ii.Mouse = null;
                _mapping[pName] = ii;
            }
        }

        public static void UpdateMapping(string pName, Mouse pMouse)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(pName));
            if(_mapping.ContainsKey(pName))
            {
                InputInfo ii = _mapping[pName];
                ii.Mouse = pMouse;
                ii.Key = null;
                _mapping[pName] = ii;
            }
        }

        public static void RemoveMapping(string pName)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(pName));
            if(_mapping.ContainsKey(pName))
            {
                _mapping.Remove(pName);
            }
        }
        #endregion
    }
}
