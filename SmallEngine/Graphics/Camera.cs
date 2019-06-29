using SmallEngine.Input;
using SmallEngine.Graphics;

namespace SmallEngine.Graphics
{
    public class Camera : IUpdatable
    {
        #region Properties
        public Rectangle Viewport
        {
            get { return new Rectangle(_position, Width, Height); }
        }

        public float Width { get; set; }

        public float Height { get; set; }

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
            System.Diagnostics.Debug.Assert(_maxZoom >= _minZoom);

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
                Position = _followObject.Position - new Vector2(Width / 2, Height / 2);
            }

            if(AllowZoom)
            {
                var mw = Mouse.WheelDelta;
                Zoom += mw * ZoomSpeed * pDeltaTime;
                Zoom = MathF.Clamp(Zoom, _minZoom, _maxZoom);
                _inverseZoom = (_maxZoom - _minZoom) / Zoom;

                var oldWidth = Width;
                var oldHeight = Height;
                Width = Game.Form.Width / Zoom;
                Height = Game.Form.Height / Zoom;
                Position += new Vector2((oldWidth - Width) / 2, (oldHeight - Height) / 2);
            }

            var bounds = Physics.PhysicsHelper.WorldBounds;
            if (Position.X < bounds.Left) _position.X = bounds.Left;
            if (Position.Y < bounds.Top) _position.Y = bounds.Top;
            if (Position.X + Width > bounds.Right) _position.X = bounds.Right - Width;
            if (Position.Y + Height > bounds.Bottom) _position.Y = bounds.Bottom - Height;
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
            return p.X + (pGameObject.Scale.Width * Zoom) > 0 && p.X <= Width &&
                   p.Y + (pGameObject.Scale.Height * Zoom) > 0 && p.Y <= Height;
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
