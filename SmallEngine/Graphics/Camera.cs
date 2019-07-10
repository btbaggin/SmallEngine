using SmallEngine.Input;
using SmallEngine.Graphics;

namespace SmallEngine.Graphics
{
    public class Camera : IUpdatable
    {
        #region Properties
        public Rectangle Viewport
        {
            get { return new Rectangle(Position, Width, Height); }
        }

        public float Width { get; set; }

        public float Height { get; set; }

        public Vector2 Position { get; set; }

        public float ZoomSpeed { get; set; }

        public bool AllowZoom { get; set; }

        public float Zoom { get; set; }

        public float MinZoom { get; set; }

        public float MaxZoom { get; set; }
        #endregion

        public Camera(float pMinZoom, float pMaxZoom)
        {
            System.Diagnostics.Debug.Assert(pMaxZoom >= pMinZoom);

            Position = Vector2.Zero;

            MinZoom = pMinZoom;
            MaxZoom = pMaxZoom;
            Width = Game.Form.Width;
            Height = Game.Form.Height;
            AllowZoom = true;
            Zoom = 1;
            ZoomSpeed = .05f;
        }

        public void Update(float pDeltaTime)
        {
            if(AllowZoom)
            {
                var mw = Mouse.WheelDelta;
                Zoom += mw * ZoomSpeed * pDeltaTime;
                Zoom = MathF.Clamp(Zoom, MinZoom, MaxZoom);

                var oldWidth = Width;
                var oldHeight = Height;
                Width = Game.Form.Width / Zoom;
                Height = Game.Form.Height / Zoom;
                Position += new Vector2((oldWidth - Width) / 2, (oldHeight - Height) / 2);
            }
        }

        public Vector2 ToWorldSpace(Vector2 pCameraSpace)
        {
            var p = pCameraSpace / Zoom;
            return p + Position;
        }

        public Vector2 ToCameraSpace(Vector2 pWorldSpace)
        {
            var p = pWorldSpace - Position;
            return p * Zoom;
        }

        public Rectangle ToCameraSpace(Rectangle pWorldRect)
        {
            var pos = ToCameraSpace(pWorldRect.Location);
            return new Rectangle(pos.X, pos.Y, pWorldRect.Width * Zoom, pWorldRect.Height * Zoom);
        }

        public bool IsVisible(IGameObject pGameObject)
        {
            var p = ToCameraSpace(pGameObject.Position);
            return p.X + (pGameObject.Scale.Width * Zoom) >= 0 && p.X <= Width &&
                   p.Y + (pGameObject.Scale.Height * Zoom) >= 0 && p.Y <= Height;
        }
    }
}
