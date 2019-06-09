using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine.Graphics
{
    class RenderSystem : ComponentSystem
    {
        readonly IGraphicsAdapter _adapter;
        public RenderSystem(IGraphicsAdapter pAdapter) : base()
        {
            _adapter = pAdapter;
        }

        protected override void RunUpdate(float pDeltaTime)
        {
            foreach (var c in Components)
            {
                var r = (RenderComponent)c;
                if (r.IsVisible() && r.GameObject.ContainingScene.Active)
                {
                    r.Draw(_adapter, pDeltaTime);
                }
            }
        }

        protected override List<IComponent> DiscoverComponents(IGameObject pObject)
        {
            var r = pObject.GetComponent(typeof(RenderComponent));

            List<IComponent> components = new List<IComponent>();
            if (r != null) components.Add(r);

            return components;
        }
    }
}
