using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngineTest
{
    class BitmapRenderComponent : RenderComponent
    {
        private BitmapResource _bitmap;
        public BitmapRenderComponent(string pAlias) : base()
        {
            _bitmap = SmallEngine.ResourceManager.Request<BitmapResource>(pAlias);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawBitmap(_bitmap, Opacity, GameObject.Position, GameObject.Scale);
        }
    }
}
