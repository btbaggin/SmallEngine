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

        public Vector2[] Verticies { get; set; }

        public virtual void Draw(IGraphicsAdapter pSystem, float pDeltaTime)
        {
            BitmapResource bitmap;
            if(_body != null)
            {
                pSystem.SetTransform(_body.CreateTransform());
                bitmap = _body.Mesh.Material.Bitmap; //TODO assign this to Bitmap at some point?
            }
            else
            {
                pSystem.ResetTransform();
                bitmap = Bitmap;
            }
            pSystem.DrawBitmap(bitmap, Opacity, GameObject.Position, GameObject.Scale);
        }

        //TODO
        //public IDrawable GetFocusElement(Vector2 pPosition)
        //{
        //    if (GameObject != null && Visible && Opacity > 0f)
        //    {
        //        var s = GameObject.Scale * Game.ActiveCamera.Zoom;
        //        var sp = Game.ActiveCamera.ToCameraSpace(GameObject.Position);
        //        if (pPosition.X >= sp.X &&
        //            pPosition.X <= sp.X + s.X &&
        //            pPosition.Y >= sp.Y &&
        //            pPosition.Y <= sp.Y + s.Y)
        //        {
        //            return this;
        //        }
        //    }

        //    return null;
        //}

        public bool IsVisible()
        {
            var onScreen = Game.ActiveCamera.IsVisible(GameObject);
            return onScreen && Visible && Opacity > 0f;
        }
    }
}
