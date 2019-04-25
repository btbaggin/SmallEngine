using SmallEngine.Input;

namespace SmallEngine
{
    public class Camera
    {
        #region Properties
        public Rectangle Viewport
        {
            get { return new Rectangle(_position, Width, Height); }
        }

        public Rectangle Bounds { get; set; }

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

        readonly float _minZoom, _maxZoom;
        private float _inverseZoom;

        private IGameObject _followObject;

        public Camera(float pMinZoom, float pMaxZoom)
        {
            Position = Vector2.Zero;

            _minZoom = pMinZoom;
            _maxZoom = pMaxZoom;
            Width = Game.Form.Width;
            Height = Game.Form.Height;
            AllowZoom = true;
            Zoom = 1;
            ZoomSpeed = .05f;
            MoveSpeed = 1000;
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
                _inverseZoom = (_maxZoom - _minZoom) / Zoom;

                var oldWidth = Width;
                var oldHeight = Height;
                Width = Game.Form.Width / Zoom;
                Height = Game.Form.Height / Zoom;
                Position += new Vector2((oldWidth - Width) / 2, (oldHeight - Height) / 2);
            }

            if (_position.X < Bounds.Left) _position.X = Bounds.Left;
            if (_position.Y < Bounds.Top) _position.Y = Bounds.Top;
            if (_position.X + Width > Bounds.Right) _position.X = Bounds.Right - Width;
            if (_position.Y + Height > Bounds.Bottom) _position.Y = Bounds.Bottom - Height;
        }

        public void MoveLeft()
        {
            _position.X -= _inverseZoom * GameTime.DeltaTime * MoveSpeed;
        }

        public void MoveRight()
        {
            _position.X += _inverseZoom * GameTime.DeltaTime * MoveSpeed;
        }

        public void MoveUp()
        {
            _position.Y -= _inverseZoom * GameTime.DeltaTime * MoveSpeed;
        }

        public void MoveDown()
        {
            _position.Y += _inverseZoom * GameTime.DeltaTime * MoveSpeed;
        }

        public Vector2 ToWorldSpace(Vector2 pCameraSpace)
        {
            var p = pCameraSpace / Zoom;
            return p + _position;
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
