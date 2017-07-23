using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class BitmapRenderComponent : RenderComponent
    {
        private BitmapResource _bitmap;
        
        public BitmapRenderComponent()
        {
        }

        public void SetBitmap(string pAlias)
        {
            _bitmap = ResourceManager.Request<BitmapResource>(pAlias);
        }

        protected override void DoDraw(IGraphicsSystem pSystem)
        {
            pSystem.DrawBitmap(_bitmap, 1, GameObject.ScreenPosition, GameObject.Scale);
        }

        public override void Dispose()
        {
            ResourceManager.Dispose<BitmapResource>(_bitmap.Alias);
        }
    }
}
