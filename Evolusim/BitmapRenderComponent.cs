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
        public BitmapResource Bitmap { get; private set; }
        
        public BitmapRenderComponent()
        {
        }

        public void SetBitmap(string pAlias)
        {
            Bitmap = ResourceManager.Request<BitmapResource>(pAlias);
        }

        public void SetBitmapFromGroup(string pGroup)
        {
            Bitmap = ResourceManager.RequestFromGroup<BitmapResource>(pGroup);
        }

        protected override void DoDraw(IGraphicsSystem pSystem)
        {
            pSystem.DrawBitmap(Bitmap, 1, GameObject.ScreenPosition, GameObject.Scale * Game.ActiveCamera.Zoom);
        }

        public override void Dispose()
        {
            ResourceManager.Dispose<BitmapResource>(Bitmap.Alias);
        }
    }
}
