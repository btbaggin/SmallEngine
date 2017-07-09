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
        public RectangleF Viewport
        {
            get { return new RectangleF(_position.X, _position.Y, _width, _height); }
        }

        private const float _zoomSpeed = 5;
        private float _moveSpeed = 100;

        private Vector2 _position;
        private float _width, _height;

        private float _minWidth, _minHeight;
        private float _maxWidth, _maxHeight;

        public Camera(float pMinWidth, float pMaxWidth, float pMinHeight, float pMaxHeight)
        {
            _position = new Vector2(100, 100);
            _minWidth = pMinWidth;
            _maxWidth = pMaxWidth;
            _minHeight = pMinHeight;
            _maxHeight = pMaxHeight;

            _width = pMaxWidth;
            _height = pMaxHeight;
        }

        public void Update(float pDeltaTime)
        {
            System.Diagnostics.Debug.WriteLine(InputManager.MouseWheelDelta);
            var mw = InputManager.MouseWheelDelta;
            _width += mw * _zoomSpeed;
            _height += mw * _zoomSpeed;

            _width = MathF.Clamp(_width, _minWidth, _maxWidth);
            _height = MathF.Clamp(_height, _minHeight, _maxHeight);
            _moveSpeed = _width;

            if (_position.X < 0) _position.X = 0;
            if (_position.Y < 0) _position.Y = 0;
        }

        public void MoveLeft()
        {
            _position.X -= _moveSpeed * GameTime.DeltaTime;
        }

        public void MoveRight()
        {
            _position.X += _moveSpeed * GameTime.DeltaTime;
        }

        public void MoveUp()
        {
            _position.Y -= _moveSpeed * GameTime.DeltaTime;
        }

        public void MoveDown()
        {
            _position.Y += _moveSpeed * GameTime.DeltaTime;
        }

        public Vector2 ToWorldSpace(Vector2 pCameraSpace)
        {
            return _position + pCameraSpace; //TODO zoom?
        }

        public Vector2 ToCameraSpace(Vector2 pWorldSpace)
        {
            return pWorldSpace - _position;
        }
    }
}
