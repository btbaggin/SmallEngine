using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Input;
using SmallEngine.Components;

namespace SmallEngine.Graphics
{
    public class RenderComparer : IComparer<RenderComponent>
    {
        public int Compare(RenderComponent x, RenderComponent y)
        {
            return x.Order.CompareTo(y.Order);
        }
    }
    public class RenderComponent : DependencyComponent
    {
        public static RenderComparer Comparer => new RenderComparer();

        [ImportComponent(false)]
        private Physics.RigidBodyComponent _body;

        public bool Visible { get; set; } = true;

        public float Opacity { get; set; } = 1f;

        public int Order { get; set; }

        public BitmapResource Bitmap { get; set; }

        public Rectangle Frame { get; set; }

        public Color Color { get; set; }

        public virtual void Draw(IGraphicsAdapter pSystem, float pDeltaTime)
        {
            if(Bitmap != null)
            {
                var t = Transform.Create(GameObject);
                pSystem.SetTransform(t);

                var position = GameObject.Position;
                position += _body.Velocity * pDeltaTime * GameTime.DeltaTime; //Interpolate position based on the amount of leftover update time
                position = Game.ActiveCamera.ToCameraSpace(position);

                var scale = GameObject.Scale * Game.ActiveCamera.Zoom;
                if (Frame.Width == 0) pSystem.DrawBitmap(Bitmap, Opacity, position, scale);
                else pSystem.DrawBitmap(Bitmap, Opacity, position, GameObject.Scale, Frame);
            }
        }

        public bool IsVisible()
        {
            var onScreen = Game.ActiveCamera.IsVisible(GameObject);
            return onScreen && Visible && Opacity > 0f;
        }
    }
}
