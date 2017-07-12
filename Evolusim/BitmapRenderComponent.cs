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

        public override void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawBitmap(_bitmap, 1, Evolusim.MainCamera.ToCameraSpace(GameObject.Position), GameObject.Scale);
        }
    }
}
