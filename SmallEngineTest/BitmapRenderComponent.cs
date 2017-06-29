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
        public BitmapResource Bitmap { get; set; }
        public BitmapRenderComponent(string pAlias) : base()
        {
            Bitmap = SmallEngine.ResourceManager.Request<BitmapResource>(pAlias);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
           pSystem.DrawBitmap(Bitmap, Opacity, GameObject.Position, GameObject.Scale);
        }
    }
}
