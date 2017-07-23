using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    class RenderComponentComparer : IComparer<RenderComponent>
    {
        public int Compare(RenderComponent x, RenderComponent y)
        {
            int result = x.Order.CompareTo(y.Order);

            if (result == 0)
                return 1;   // Handle equality as being greater
            else
                return result;
        }
    }

    public abstract class RenderComponent : Component, IDrawable
    {
        public bool Visible { get; set; }
        public float Opacity { get; set; }
        public int Order { get; set; }
        private void BeginDraw(IGraphicsSystem pSystem)
        {
            var center = GameObject.Position + (GameObject.Scale / 2);
            pSystem.SetTransform(GameObject.Rotation, new Vector2(center.X, center.Y));
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            if (!IsVisible()) return;
            BeginDraw(pSystem);
            DoDraw(pSystem);
            EndDraw(pSystem);
        }

        private void EndDraw(IGraphicsSystem pSystem)
        {
            pSystem.ResetTransform();
        }

        protected abstract void DoDraw(IGraphicsSystem pSystem);

        public RenderComponent()
        {
            Visible = true;
            Opacity = 1f;
        }

        private bool IsVisible()
        {
            var onScreen = Game.ActiveCamera.IsVisible(GameObject);
            return onScreen && Visible && Opacity > 0f;
        }
    }
}
