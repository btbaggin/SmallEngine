using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Input;

namespace SmallEngine.Graphics
{
    public class RenderComparer : IComparer<IDrawable>
    {
        public int Compare(IDrawable c1, IDrawable c2)
        {
            return c1.Order.CompareTo(c2.Order);
        }
    }
    public abstract class RenderComponent : Component, IDrawable
    {
        public static RenderComparer Comparer => new RenderComparer();

        public bool Visible { get; set; }

        public float Opacity { get; set; }

        public int Order { get; set; }

        public void Draw(IGraphicsSystem pSystem)
        {
            if (!IsVisible()) return;
            DoDraw(pSystem);
        }

        public void Draw(IGraphicsSystem pSystem, Effect pEffect)
        {
            if (!IsVisible()) return;
            DoDraw(pSystem, pEffect);
        }

        protected abstract void DoDraw(IGraphicsSystem pSystem);

        protected abstract void DoDraw(IGraphicsSystem pSystem, Effect pEffect);

        public RenderComponent()
        {
            Visible = true;
            Opacity = 1f;
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            //TODO don't add renders to updatable?
            base.OnAdded(pGameObject);
            SceneManager.Current.AddDrawable(this);
        }

        public IDrawable GetFocusElement(Vector2 pPoint)
        {
            if (IsVisible())
            {
                var s = GameObject.Scale * Game.ActiveCamera.Zoom;
                var sp = GameObject.ScreenPosition;
                if (pPoint.X >= sp.X &&
                    pPoint.X <= sp.X + s.X &&
                    pPoint.Y >= sp.Y &&
                    pPoint.Y <= sp.Y + s.Y)
                {
                    return this;
                }
            }

            return null;
        }

        private bool IsVisible()
        {
            var onScreen = Game.ActiveCamera.IsVisible(GameObject);
            return onScreen && Visible && Opacity > 0f;
        }
    }
}
