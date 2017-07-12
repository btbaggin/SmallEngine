using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SmallEngine;
using SmallEngine.Input;

namespace Evolusim
{
    class Camera
    {
        #region Properties
        public RectangleF Viewport
        {
            get { return new RectangleF(_position.X, _position.Y, Width, Height); }
        }

        public float Width { get; private set; }

        public float Height { get; private set; }

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            private set { _position = value; }
        }
        #endregion

        private float _zoomXSpeed;
        private float _zoomYSpeed;
        private float _moveXSpeed;
        private float _moveYSpeed;

        private float _minWidth, _minHeight;
        private float _maxWidth, _maxHeight;

        public Camera(float pMinWidth, float pMaxWidth, float pMinHeight, float pMaxHeight)
        {
            Position = Vector2.Zero;
            _minWidth = pMinWidth;
            _maxWidth = pMaxWidth;
            _minHeight = pMinHeight;
            _maxHeight = pMaxHeight;

            Width = pMaxWidth;
            Height = pMaxHeight;

            _zoomYSpeed = 5;
            _zoomXSpeed = 5 * (Width / Height);
        }

        public void Update(float pDeltaTime)
        {
            var mw = InputManager.MouseWheelDelta;
            Width += mw * _zoomXSpeed;
            Height += mw * _zoomYSpeed;

            Width = MathF.Clamp(Width, _minWidth, _maxWidth);
            Height = MathF.Clamp(Height, _minHeight, _maxHeight);
            _moveXSpeed = Width;
            _moveYSpeed = Height;

            if (_position.X < 0) _position.X = 0;
            if (_position.Y < 0) _position.Y = 0;
            if (_position.X + Width > Evolusim.WorldSize) _position.X = Evolusim.WorldSize - Width;
            if (_position.Y + Height > Evolusim.WorldSize) _position.Y = Evolusim.WorldSize - Height;
        }

        public void MoveLeft()
        {
            _position.X -= _moveXSpeed * GameTime.DeltaTime;
        }

        public void MoveRight()
        {
            _position.X += _moveXSpeed * GameTime.DeltaTime;
        }

        public void MoveUp()
        {
            _position.Y -= _moveYSpeed * GameTime.DeltaTime;
        }

        public void MoveDown()
        {
            _position.Y += _moveYSpeed * GameTime.DeltaTime;
        }

        public Vector2 ToWorldSpace(Vector2 pCameraSpace)
        {
            var dx = Width / Game.Form.Width;
            var dy = Height / Game.Form.Height;
            return new Vector2(pCameraSpace.X * dx, pCameraSpace.Y * dy) + _position;
        }

        public Vector2 ToCameraSpace(Vector2 pWorldSpace)
        {
            var dx = Width / Game.Form.Width;
            var dy = Height / Game.Form.Height;
            return new Vector2(pWorldSpace.X / dx, pWorldSpace.Y / dy) - _position;
        }
    }
}
