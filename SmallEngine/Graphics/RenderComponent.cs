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

    public abstract class RenderComponent : Component
    {
        internal static List<RenderComponent> Renderers;

        public bool Visible { get; set; }
        public float Opacity { get; set; }
        public int Order { get; set; }
        public void BeginDraw(IGraphicsSystem pSystem)
        {
            var center = GameObject.Position + (GameObject.Scale / 2);
            pSystem.SetTransform(GameObject.Rotation, new Vector2(center.X, center.Y));
        }

        public abstract void Draw(IGraphicsSystem pSystem);

        public void EndDraw(IGraphicsSystem pSystem)
        {
            pSystem.ResetTransform();
        }

        static RenderComponent()
        {
            Renderers = new List<RenderComponent>();
        }

        public RenderComponent()
        {
            Visible = true;
            Opacity = 1f;
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            Renderers.Add(this);
            base.OnAdded(pGameObject);
        }

        public override void OnRemoved()
        {
            Renderers.Remove(this);
            base.OnRemoved();
        }

        public override void OnActiveChanged(bool pActive)
        {
            if(pActive) { Renderers.Add(this); }
            else { Renderers.Remove(this); }   
        }
    }
}
