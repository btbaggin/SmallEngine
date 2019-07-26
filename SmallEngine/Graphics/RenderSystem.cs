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
        public RenderSystem(IGraphicsAdapter pAdapter) : base(typeof(RenderComponent))
        {
            _adapter = pAdapter;
        }

        public override void Process()
        {
            var deltaTime = GameTime.RenderTime;
            foreach (var c in Components)
            {
                var r = (RenderComponent)c;
                if (r.IsVisible())
                {
                    r.Draw(_adapter, deltaTime);
                }
            }
        }
    }
}
