﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmallEngine.Graphics;

namespace SmallEngine
{
    #region WindowEventArgs
    public class WindowEventArgs : EventArgs
    {
        public bool Enabled { get; private set; }
        public bool Activated { get; private set; }
        public bool Maximized { get; private set; }
        public Size Size { get; private set; }
        public WindowEventArgs(bool pEnabled, bool pActivated, bool pMaximized, Size pSize)
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
        public EventHandler<WindowEventArgs> WindowActivateChanged { get; set; }
        public EventHandler<WindowEventArgs> WindowDestory { get; set; }
        public EventHandler<WindowEventArgs> WindowEnableChanged { get; set; }
        public EventHandler<WindowEventArgs> WindowMaximizeChanged { get; set; }
        public EventHandler<WindowEventArgs> WindowSizeChanged { get; set; }

        #region Properties
        bool _vsync;
        public bool Vsync
        {
            get { return _vsync; }
            set
            {
                _vsync = value;
                SyncInterval = value ? 1 : 0;
            }
        }

        internal int SyncInterval;

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

        private Size _size;
        public new Size Size
        {
            get { return _size; }
            set
            {
                if(_size.Width != value.Width || _size.Height != value.Height)
                {
                    _size = value;
                    ClientSize = _size;
                    Width = ClientSize.Width;
                    Height = ClientSize.Height;
                    WindowSizeChanged?.Invoke(this, new WindowEventArgs(Enabled, Activated, Maximized, Size));
                }
            }
        }

        public new int Width { get; private set; }

        public new int Height { get; private set; }

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

        #region Constuctor
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

        #region Message constants
        const int WM_DESTROY = 0x2;
        const int WM_SIZE = 0x0005;
        const int WM_ACTIVATE = 0x6;
        const int WM_MOUSEWHEEL = 0x020A;
        const int WM_ENABLE = 0x000A;

        const int SIZE_MAXIMIZED = 2;
        const int SIZE_MINIMIZED = 1;
        #endregion

        #region WndPrc
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ACTIVATE:
                    Activated = GetWLowWord(m) != 0;
                    return;

                case WM_MOUSEWHEEL:
                    Input.Mouse.WheelAbsolute = (short)GetWHighWord(m);
                    return;

                case WM_ENABLE:
                    Enabled = (m.WParam.ToInt32() != 0);
                    return;

                case WM_SIZE:
                    switch (m.WParam.ToInt32())
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
                        Size = new System.Drawing.Size(GetLLowWord(m), GetLHighWord(m));
                    }

                    return;

                case WM_DESTROY:
                    WindowDestory?.Invoke(this, new WindowEventArgs(false, false, false, System.Drawing.Size.Empty));
                    return;

                default:
                    base.WndProc(ref m);
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
