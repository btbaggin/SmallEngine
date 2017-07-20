﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public abstract class UIElement
    {
        #region Properties
        private Vector2 _position;
        public Vector2 Position
        {
            get
            {
                if (Parent != null) { return _position + Parent.Position; }
                else { return _position; }
            }
            set { _position = value; }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                var s = new SizeF(Width, Height);
                foreach (var c in Children)
                {
                    c.Measure(s, Position.X, Position.Y);
                }
            }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                var s = new SizeF(Width, Height);
                foreach (var c in Children)
                {
                    c.Measure(s, Position.X, Position.Y);
                }
            }
        }

        public int TotalWidth { get { return Width + (int)(Margin.X * 2); } }

        public int TotalHeight { get { return Height + (int)(Margin.Y * 2); } }

        public float HeightPercent { get; set; }

        public float WidthPercent { get; set; }

        public bool Visible { get; set; }

        public bool Enabled { get; set; }

        public int Order { get; set; }

        public Vector2 Margin { get; set; }

        protected UIElement Parent { get; private set; }

        protected List<UIElement> Children { get; private set; }

        protected AnchorDirection Anchor { get; set; }

        protected Vector2 AnchorPoint { get; set; }

        public ElementOrientation Orientation { get; set; }

        public RectangleF Bounds
        {
            get { return new RectangleF(Position.X, Position.Y, Width, Height); }
        }

        public bool IsMouseOver
        {
            get
            {
                var p = Input.InputManager.MousePosition;
                return Bounds.Contains(new System.Drawing.PointF(p.X, p.Y));
            }
        }
        #endregion

        public UIElement()
        {
            Children = new List<UIElement>();
            Visible = true;
            Enabled = true;
        }

        protected void SetLayout()
        {
            if (Parent != null) { Measure(new SizeF(Parent.Width, Parent.Height), 0, 0); }
            else { Measure(new SizeF(Game.Form.Width, Game.Form.Height), 0, 0); }
        }

        public virtual void Update(float pDeltaTime)
        {
            foreach (var e in Children)
            {
                if (e.Visible && e.Enabled)
                {
                    e.Update(pDeltaTime);
                }
            }
        }

        public virtual void Draw(IGraphicsSystem pSystem)
        {
            foreach (var e in Children)
            {
                if (e.Visible)
                {
                    e.Draw(pSystem);
                }
            }
        }

        public void AddChild(UIElement pElement, AnchorDirection pAnchor, Vector2 pAnchorPoint)
        {
            pElement.Anchor = pAnchor;
            pElement.AnchorPoint = pAnchorPoint;
            pElement.Parent = this;
            Children.Add(pElement);
        }

        public virtual void Measure(SizeF pSize, float pLeft, float pTop)
        {
            if (HeightPercent > 0)
            {
                _height = (int)(pSize.Height * HeightPercent);
                _height -= (int)(Margin.Y * 2);
            }

            if (WidthPercent > 0)
            {
                _width = (int)(pSize.Width * WidthPercent);
                _width -= (int)(Margin.X * 2);
            }

            _width = (int)Math.Min(pSize.Width - Margin.X, Width);
            _height = (int)Math.Min(pSize.Height - Margin.Y, Height);

            if (Anchor.HasFlag(AnchorDirection.Left))
                Position = new Vector2(AnchorPoint.X, _position.Y);

            if (Anchor.HasFlag(AnchorDirection.Top))
                Position = new Vector2(_position.X, AnchorPoint.Y);

            if (Anchor.HasFlag(AnchorDirection.Right))
                Position = new Vector2(pSize.Width - (AnchorPoint.X + Width), _position.Y);

            if (Anchor.HasFlag(AnchorDirection.Bottom))
                Position = new Vector2(_position.X, pSize.Height - (AnchorPoint.Y + Height));
            _position += new Vector2(pLeft, pTop);// pPosition;

            _position.X = Math.Max(Margin.X, _position.X);
            _position.Y = Math.Max(Margin.Y, _position.Y);

            pLeft = 0; pTop = 0;
            pSize = new SizeF(Width, Height);
            foreach (var c in Children)
            {
                c.Measure(pSize, pLeft, pTop);
                if (Orientation == ElementOrientation.Horizontal)
                {
                    pLeft += c.TotalWidth;
                }
                else
                {
                    pTop += c.TotalHeight;
                }
            }
        }
    }
}