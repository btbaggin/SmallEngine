﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace SmallEngine.UI
{
    public abstract class UIElement : IDisposable, IComparable<UIElement>
    {
        #region Properties
        public Vector2 Position { get; protected set; }

        private int _width;
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                ContainingScene.InvalidateMeasure(); //TODO make better
            }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                ContainingScene.InvalidateMeasure();
            }
        }

        private Thickness _margin;
        public Thickness Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                ContainingScene.InvalidateMeasure();
            }
        }

        public Visibility Visible { get; set; }

        public HorizontalAlignments HorizontalAlignment { get; set; }

        public VerticalAlignments VerticalAlignment { get; set; }

        public bool Enabled { get; set; }

        protected UIElement Parent { get; private set; }

        public List<UIElement> Children { get; private set; }

        public Rectangle Bounds { get; private set; }

        public float ActualHeight
        {
            get { return Bounds.Height; }
        }

        public float ActualWidth
        {
            get { return Bounds.Width; }
        }

        short _zindex;
        public short ZIndex
        {
            get { return _zindex; }
            set
            {
                _zindex = value;
                if(Parent != null)
                {
                    Parent._orderedItems.Remove(this);
                    Parent._orderedItems.AddOrdered(this);
                }
            }
        }

        public string Name { get; private set; }

        public Scene ContainingScene { get; private set; }
        #endregion

        bool _disposed;
        readonly List<UIElement> _orderedItems = new List<UIElement>();
        protected UIElement(string pName)
        {
            Children = new List<UIElement>();
            _margin = new Thickness(5);
            Visible = Visibility.Visible;
            Enabled = true;
            Name = pName;
            HorizontalAlignment = HorizontalAlignments.Center;
            ContainingScene = Scene.OnUIElementCreated(this);
        }

        protected void InvalidateMeasure()
        {
            if (Parent != null) { Measure(Parent.DesiredSize); }
            else { Measure(new Size(Game.Form.Width, Game.Form.Height)); }
        }

        public void FinalizeLayout()
        {
            Scene.RegisterUI(this);
        }

        public abstract void Draw(IGraphicsAdapter pSystem);

        public abstract void Update();

        protected void AddChild(UIElement pElement)
        {
            pElement.Parent = this;
            Children.Add(pElement);
            //Used to draw items by ZIndex
            //ZIndex shouldn't affect how the children are displayed within container controls so we keep 2 lists
            _orderedItems.AddOrdered(pElement); 
        }
        protected void HandleInputEvent(Keys pKey)
        {
            Keyboard.MarkUIEventHandled(pKey);
        }

        public bool IsMouseOver()
        {
            if(Bounds.Contains(Mouse.Position))
            {
                foreach(var c in Children)
                {
                    if (c.IsMouseOver()) return false;
                }

                return true;
            }

            return false;
        }

        #region Layout methods
        public Size DesiredSize { get; set; }

        public Size ActualSize { get; set; }

        public void Measure(Size pSize)
        {
            if (Visible == Visibility.Collapsed) return;

            //Measure children
            var s = MeasureOverride(pSize);
            DesiredSize = new Size(s.Width + Margin.Width, s.Height + Margin.Height);
        }

        public virtual Size MeasureOverride(Size pSize)
        {
            //Default to taking up minimum space as possible
            float desiredHeight = Height;
            float desiredWidth = Width;

            //Only auto size if we haven't already set dimensions
            bool setHeight = Height != 0;
            bool setWidth = Width != 0;

            //Measure all children
            foreach (var c in Children)
            {
                c.Measure(pSize);

                var s = c.DesiredSize;
                if(!setHeight) desiredWidth += s.Width;
                if(!setWidth) desiredHeight += s.Height;
            }

            return new Size(desiredWidth, desiredHeight);
        }

        public void Arrange(Rectangle pBounds)
        {
            var x = Margin.Left + pBounds.Left;
            var y = Margin.Top + pBounds.Top;

            if (Visible == Visibility.Collapsed)
            {
                ActualSize = new Size(0, 0);
                Bounds = new Rectangle(x, y, 0, 0);
                return;
            }
            
            //Size will be either how much we want, or the available space left
            var width = Math.Min(pBounds.Width, DesiredSize.Width);
            var height = Math.Min(pBounds.Height, DesiredSize.Height);

            //Orient based off alignments
            //TODO these don't work with children
            if (HorizontalAlignment == HorizontalAlignments.Center)
            {
                x += (pBounds.Width - width) / 2; 
            }
            else if (HorizontalAlignment == HorizontalAlignments.Right)
            {
                x = pBounds.Right - Margin.Width - width;
            }

            if(VerticalAlignment == VerticalAlignments.Center)
            {
                y += (pBounds.Height - height) / 2;
            }
            else if(VerticalAlignment == VerticalAlignments.Bottom)
            {
                y = pBounds.Bottom - Margin.Height - height;
            }

            //Space left over does not include margins
            ActualSize = new Size((int)width, (int)height);

            width -= Margin.Width;
            height -= Margin.Height;

            Bounds = new Rectangle(x, y, width, height);
            Position = Bounds.Location;

            //Arrange children
            ArrangeOverride(Bounds);
        }

        public virtual void ArrangeOverride(Rectangle pBounds)
        {
            foreach(var c in Children)
            {
                c.Arrange(pBounds);
            }
        }
        #endregion

        #region Internal Methods
        internal void DrawInternal(IGraphicsAdapter pSystem)
        {
            if (_disposed) return;
            Draw(pSystem);
            foreach (var e in _orderedItems)
            {
                if (e.Visible == Visibility.Visible)
                {
                    e.DrawInternal(pSystem);
                }
            }
        }

        internal void UpdateInternal()
        {
            foreach (var e in Children)
            {
                e.UpdateInternal();
            }

            Update();

            if (Enabled && Bounds.Contains(Mouse.Position))
            {
                Mouse.MarkButtonHandled(MouseButtons.Left);
                Mouse.MarkButtonHandled(MouseButtons.Right);
            }

        }
        #endregion  

        public virtual void Dispose()
        {
            _disposed = true;
            foreach(var c in Children)
            {
                c.Dispose();
            }
        }

        public int CompareTo(UIElement other)
        {
            return ZIndex.CompareTo(other.ZIndex);
        }
    }
}