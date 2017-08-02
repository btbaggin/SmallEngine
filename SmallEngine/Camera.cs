﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SmallEngine;
using SmallEngine.Input;

namespace SmallEngine
{
    public class Camera
    {
        #region Properties
        public RectangleF Viewport
        {
            get { return new RectangleF(_position.X, _position.Y, Width, Height); }
        }

        public RectangleF Bounds { get; set; }

        public float Width { get; private set; }

        public float Height { get; private set; }

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float MoveSpeed { get; set; }

        public float ZoomSpeed { get; set; }

        public float Zoom { get; set; }

        public bool AllowZoom { get; set; }

        public bool IsFollowing
        {
            get { return _followObject != null; }
        }
        #endregion

        //private float _minWidth, _minHeight;
        //private float _maxWidth, _maxHeight;

        private float _minZoom, _maxZoom;

        private IGameObject _followObject;

        public Camera(float pMinZoom, float pMaxZoom)//float pMinWidth, float pMaxWidth, float pMinHeight, float pMaxHeight)//Eh...
        {
            Position = Vector2.Zero;
            //_minWidth = pMinWidth;
            //_maxWidth = pMaxWidth;
            //_minHeight = pMinHeight;
            //_maxHeight = pMaxHeight;

            _minZoom = pMinZoom;
            _maxZoom = pMaxZoom;
            Width = Game.Form.Width;//pMaxWidth;
            Height = Game.Form.Height;//pMaxHeight;
            AllowZoom = true;
            Zoom = 1;
            ZoomSpeed = .05f;
            MoveSpeed = 10;
        }

        public void Update(float pDeltaTime)
        {
            if(IsFollowing)
            {
                _position = _followObject.Position - new Vector2(Width / 2, Height / 2);
            }

            if(AllowZoom)
            {
                var mw = InputManager.MouseWheelDelta;
                Zoom += mw * ZoomSpeed * pDeltaTime;
                Zoom = MathF.Clamp(Zoom, _minZoom, _maxZoom);

                Width = Game.Form.Width / Zoom;
                Height = Game.Form.Height / Zoom;
            }

            //Width = MathF.Clamp(Width, _minWidth, _maxWidth);
            //Height = MathF.Clamp(Height, _minHeight, _maxHeight);

            if (_position.X < Bounds.Left) _position.X = Bounds.Left;
            if (_position.Y < Bounds.Top) _position.Y = Bounds.Top;
            if (_position.X + Width > Bounds.Right) _position.X = Bounds.Right - Width;
            if (_position.Y + Height > Bounds.Bottom) _position.Y = Bounds.Bottom - Height;
        }

        public void MoveLeft()
        {
            _position.X -= Width * GameTime.DeltaTime * MoveSpeed;
        }

        public void MoveRight()
        {
            _position.X += Width * GameTime.DeltaTime * MoveSpeed;
        }

        public void MoveUp()
        {
            _position.Y -= Height * GameTime.DeltaTime * MoveSpeed;
        }

        public void MoveDown()
        {
            _position.Y += Height * GameTime.DeltaTime * MoveSpeed;
        }

        public Vector2 ToWorldSpace(Vector2 pCameraSpace)
        {
            var dx = Width / Game.Form.Width;
            var dy = Height / Game.Form.Height;
            return new Vector2(pCameraSpace.X * dx, pCameraSpace.Y * dy) + _position;
        }

        public Vector2 ToCameraSpace(Vector2 pWorldSpace)
        {
            var p = pWorldSpace - _position;
            return p * Zoom;
        }

        public bool IsVisible(IGameObject pGameObject)
        {
            var p = pGameObject.Position - _position;
            return p.X + (pGameObject.Scale.X * Zoom) > 0 && p.X < Width &&
                   p.Y + (pGameObject.Scale.Y * Zoom) > 0 && p.Y < Height;
        }

        public void Follow(IGameObject pObject)
        {
            _followObject = pObject;
        }

        public void StopFollow()
        {
            _followObject = null;
        }
    }
}
