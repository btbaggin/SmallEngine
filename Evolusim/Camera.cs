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
        #endregion

        private const float _zoomSpeed = 5;
        private float _moveXSpeed;
        private float _moveYSpeed;

        private Vector2 _position;

        private float _minWidth, _minHeight;
        private float _maxWidth, _maxHeight;

        public Camera(float pMinWidth, float pMaxWidth, float pMinHeight, float pMaxHeight)
        {
            _position = Vector2.Zero;
            _minWidth = pMinWidth;
            _maxWidth = pMaxWidth;
            _minHeight = pMinHeight;
            _maxHeight = pMaxHeight;

            Width = pMaxWidth;
            Height = pMaxHeight;
        }

        public void Update(float pDeltaTime)
        {
            var mw = InputManager.MouseWheelDelta;
            Width += mw * _zoomSpeed;
            Height += mw * _zoomSpeed;

            Width = MathF.Clamp(Width, _minWidth, _maxWidth);
            Height = MathF.Clamp(Height, _minHeight, _maxHeight);
            _moveXSpeed = Width;
            _moveYSpeed = Height;

            if (_position.X < 0) _position.X = 0;
            if (_position.Y < 0) _position.Y = 0;
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
            //TODO
            return pWorldSpace - _position;
        }
    }
}
