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
                SetPositionInternal(diff);
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
                foreach (var c in _children)
                {
                    c.Measure(s);
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
                foreach (var c in _children)
                {
                    c.Measure(s);
                }
            }
        }

        public float HeightPercent { get; set; }

        public float WidthPercent { get; set; }

        public bool Visible { get; set; }

        public bool Enabled { get; set; }

        public Vector2 Margin { get; set; }
        #endregion

        private List<UIElement> _children;
        private AnchorDirection _anchor;
        private Vector2 _anchorPoint;

        public UIElement()
        {
            _children = new List<UIElement>();
        }

        public virtual void Update(float pDeltaTime)
        {
            foreach (var e in _children)
            {
                if (e.Visible && e.Enabled)
                {
                    e.Update(pDeltaTime);
                }
            }
        }

        public virtual void Draw(IGraphicsSystem pSystem)
        {
            foreach (var e in _children)
            {
                if (e.Visible)
                {
                    e.Draw(pSystem);
                }
            }
        }

        public void AddChild(UIElement pElement, AnchorDirection pAnchor, Vector2 pAnchorPoint)
        {
            pElement._anchor = pAnchor;
            pElement._anchorPoint = pAnchorPoint;
            _children.Add(pElement);
        }

        private void SetPositionInternal(Vector2 pDiff)
        {
            _position -= pDiff;
            foreach (var c in _children)
            {
                c.SetPositionInternal(pDiff);
            }
        }

        public void Measure(SizeF pSize)
        {
            if (_anchor.HasFlag(AnchorDirection.Left))
            {
                _position = new Vector2(_anchorPoint.X, Position.Y);
            }

            if (_anchor.HasFlag(AnchorDirection.Top))
            {
                _position = new Vector2(Position.X, _anchorPoint.Y);
            }

            if (_anchor.HasFlag(AnchorDirection.Right))
            {
                _position = new Vector2(pSize.Width - _anchorPoint.X, Position.Y);
            }

            if (_anchor.HasFlag(AnchorDirection.Bottom))
            {
                _position = new Vector2(Position.X, pSize.Height - _anchorPoint.Y);
            }

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
            Width = (int)Math.Min(pSize.Width - Width, Margin.X);
            Height = (int)Math.Min(pSize.Height - Height, Margin.Y);

            foreach (var c in _children)
            {
                c.Measure(new SizeF(Width, Height));
            }
        }
    }
}