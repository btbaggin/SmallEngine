using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Input;

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

        [ImportComponent]
        private Physics.RigidBodyComponent _body;

        public bool Visible { get; set; } = true;

        public float Opacity { get; set; } = 1f;

        public int Order { get; set; }

        public BitmapResource Bitmap { get; set; }

        public Color Color { get; set; }

        public virtual void Draw(IGraphicsAdapter pSystem, float pDeltaTime)
        {
            if(Bitmap != null)
            {
                if (_body != null) pSystem.SetTransform(_body.CreateTransform());
                else pSystem.ResetTransform();

                pSystem.DrawBitmap(Bitmap, Opacity, GameObject.Position, GameObject.Scale);
            }
        }

        public bool IsVisible()
        {
            var onScreen = Game.ActiveCamera.IsVisible(GameObject);
            return onScreen && Visible && Opacity > 0f;
        }
    }
}
