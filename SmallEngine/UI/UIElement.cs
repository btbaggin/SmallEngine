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
            get { return _position; }
            set
            {
                var diff = value - Position;
                _position = value;
                foreach(var c in Children)
                {
                    c.SetPositionInternal(diff);
                }
            }
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
                    c.Measure(s, Position);
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
                    c.Measure(s, Position);
                }
            }
        }

        private float _heightPercent;
        public float HeightPercent
        {
            get { return _heightPercent; }
            set
            {
                _heightPercent = value;
                Height = (int)(Game.Form.Height * HeightPercent);
            }
        }

        private float _widthPercent;
        public float WidthPercent
        {
            get { return _widthPercent; }
            set
            {
                _widthPercent = value;
                Width = (int)(Game.Form.Width * WidthPercent);
            }
        }

        public bool Visible { get; set; }

        public bool Enabled { get; set; }

        public Vector2 Margin { get; set; }

        protected List<UIElement> Children { get; private set; }

        protected AnchorDirection Anchor { get; set; }

        protected Vector2 AnchorPoint { get; set; }

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
            Children.Add(pElement);
            //pElement.Measure(new Size(Width, Height), Position);
        }

        private void SetPositionInternal(Vector2 pDiff)
        {
            _position += pDiff;
            foreach (var c in Children)
            {
                c.SetPositionInternal(pDiff);
            }
        }

        public virtual void Measure(SizeF pSize, Vector2 pPosition)
        {
            if (Anchor.HasFlag(AnchorDirection.Left))
            {
                _position = new Vector2(AnchorPoint.X, Position.Y);
            }

            if (Anchor.HasFlag(AnchorDirection.Top))
            {
                _position = new Vector2(Position.X, AnchorPoint.Y);
            }

            if (Anchor.HasFlag(AnchorDirection.Right))
            {
                _position = new Vector2(pSize.Width - AnchorPoint.X, Position.Y);
            }

            if (Anchor.HasFlag(AnchorDirection.Bottom))
            {
                _position = new Vector2(Position.X, pSize.Height - AnchorPoint.Y);
            }
            _position += pPosition;

            if (HeightPercent > 0)
            {
                Height = (int)(pSize.Height * HeightPercent);
            }

            if (WidthPercent > 0)
            {
                Width = (int)(pSize.Width * WidthPercent);
            }

            _position.X = Math.Max(Margin.X, _position.X);
            _position.Y = Math.Max(Margin.Y, _position.Y);
            Width = (int)Math.Min(pSize.Width - Margin.X, Width);
            Height = (int)Math.Min(pSize.Height - Margin.Y, Height);

            foreach (var c in Children)
            {
                c.Measure(new SizeF(Width, Height), Position);
            }
        }
    }
}