using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmallEngine
{
    #region "WindowEventArgs"
    public class WindowEventArgs : EventArgs
    {
        public bool Enabled { get; private set; }
        public bool Activated { get; private set; }
        public bool Maximized { get; private set; }
        public System.Drawing.Size Size { get; private set; }
        public WindowEventArgs(bool pEnabled, bool pActivated, bool pMaximized, System.Drawing.Size pSize)
        {
            Enabled = pEnabled;
            Activated = pActivated;
            Maximized = pMaximized;
            Size = pSize;
        }
    }
    #endregion  

    public class GameForm : Form
    {
        public EventHandler<WindowEventArgs> WindowActivateChanged;
        public EventHandler<WindowEventArgs> WindowDestory;
        public EventHandler<WindowEventArgs> WindowEnableChanged;
        public EventHandler<WindowEventArgs> WindowMaximizeChanged;
        public EventHandler<WindowEventArgs> WindowSizeChanged;

        #region "Properties"
        public bool Vsync { get; set; }

        private bool _fullScreen;
        public bool FullScreen
        {
            get { return _fullScreen; }
            set
            {
                if (value != _fullScreen)
                {
                    _fullScreen = value;
                    Game.Graphics.SetFullScreen(_fullScreen);
                }
            }
        }

        private bool _enabled;
        public new bool Enabled
        {
            get { return _enabled; }
            private set
            {
                if(_enabled != value)
                {
                    _enabled = value;
                    WindowEnableChanged?.Invoke(this, new WindowEventArgs(Enabled, Activated, Maximized, Size));
                }
            }
        }

        private bool _activated;
        public new bool Activated
        {
            get { return _activated; }
            set
            {
                if(_activated != value)
                {
                    _activated = value;
                    WindowActivateChanged?.Invoke(this, new WindowEventArgs(Enabled, Activated, Maximized, Size));
                }
            }
        }

        private System.Drawing.Size _size;
        public new System.Drawing.Size Size
        {
            get { return _size; }
            set
            {
                if(_size != value)
                {
                    _size = value;
                    ClientSize = _size;
                    _width = ClientSize.Width;
                    _height = ClientSize.Height;
                    WindowSizeChanged?.Invoke(this, new WindowEventArgs(Enabled, Activated, Maximized, Size));
                }
            }
        }

        private int _width;
        public new int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                Size = new System.Drawing.Size(Width, Height);
            }
        }

        private int _height;
        public new int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                Size = new System.Drawing.Size(Width, Height);
            }
        }

        private bool _maximized;
        public bool Maximized
        {
            get { return _maximized; }
            private set
            {
                if(_maximized != value)
                {
                    _maximized = value;
                    WindowMaximizeChanged?.Invoke(this, new WindowEventArgs(Enabled, Activated, Maximized, Size));
                }
            }
        }

        bool _allowResize;
        /// <summary>
        /// Gets or sets if the games window allows for user resizing
        /// </summary>
        public bool AllowUserResizing
        {
            get { return _allowResize; }
            set
            {
                _allowResize = value;
                MaximizeBox = _allowResize;
                FormBorderStyle = _allowResize ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
            }
        }
        #endregion

        #region "Constuctor"
        public GameForm() : this("") { }

        public GameForm(string pText) : this(pText, 480, 360) { }

        public GameForm(string pText, int pWidth, int pHeight)
        {
            Text = pText;
            DoubleBuffered = true;
            Width = pWidth;
            Height = pHeight;
            AllowUserResizing = true;
        }
        #endregion

        #region "Message constants"
        const int WM_DESTROY = 0x2;
        const int WM_SIZE = 0x0005;
        const int WM_ACTIVATE = 0x6;
        const int WM_MOUSEWHEEL = 0x020A;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_MBUTTONDOWN = 0x0207;
        const int WM_MBUTTONUP = 0x0208;
        const int WM_RBUTTONDOWN = 0x0204;
        const int WM_RBUTTONUP = 0x0205;
        const int WM_ENABLE = 0x000A;

        const int SIZE_MAXIMIZED = 2;
        const int SIZE_MINIMIZED = 1;
        #endregion

        #region "WndPrc"
        protected override void WndProc(ref Message pM)
        {
            switch (pM.Msg)
            {
                case WM_ACTIVATE:
                    Activated = !(GetWLowWord(pM) == 0);
                    return;

                case WM_MOUSEWHEEL:
                    Input.InputManager.MouseDelta = (short)GetWHighWord(pM);
                    return;

                case WM_ENABLE:
                    Enabled = (pM.WParam.ToInt32() != 0);
                    return;

                case WM_SIZE:
                    switch (pM.WParam.ToInt32())
                    {
                        case SIZE_MAXIMIZED:
                            Maximized = true;
                            break;

                        case SIZE_MINIMIZED:
                            Maximized = false;
                            break;
                    }

                    if (AllowUserResizing)
                    {
                        Size = new System.Drawing.Size(GetLLowWord(pM), GetLHighWord(pM));
                    }

                    return;

                case WM_DESTROY:
                    WindowDestory?.Invoke(this, new WindowEventArgs(false, false, false, System.Drawing.Size.Empty));
                    return;

                default:
                    base.WndProc(ref pM);
                    return;

            }
        }

        private static int GetLLowWord(Message pM)
        {
            return (pM.LParam.ToInt32() & 0xFFFF);
        }

        private static int GetLHighWord(Message pM)
        {
            return (pM.LParam.ToInt32() >> 16) & 0xFFFF;
        }

        private static int GetWLowWord(Message pM)
        {
            return (pM.WParam.ToInt32() & 0xFFFF);
        }

        private static int GetWHighWord(Message pM)
        {
            return (int)((pM.WParam.ToInt64() >> 16) & 0xFFFF);
        }
        #endregion
    }
}
