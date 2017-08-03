using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    public abstract class RenderComponent : Component
    {
        public bool Visible { get; set; }

        public float Opacity { get; set; }

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

        private bool IsVisible()
        {
            var onScreen = Game.ActiveCamera.IsVisible(GameObject);
            return onScreen && Visible && Opacity > 0f;
        }
    }
}
