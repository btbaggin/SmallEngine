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
    public abstract class RenderComponent : Component
    {
        public static RenderComparer Comparer => new RenderComparer();

        public bool Visible { get; set; }

        public float Opacity { get; set; }

        public int Order { get; set; }

        public abstract void Draw(IGraphicsAdapter pSystem, float pDeltaTime);

        protected RenderComponent()
        {
            Visible = true;
            Opacity = 1f;
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
