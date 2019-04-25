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

        public BitmapRenderComponent() { }

        public BitmapRenderComponent(string pAlias)
        {
            Bitmap = ResourceManager.Request<BitmapResource>(pAlias);
        }

        public void SetBitmap(string pAlias)
        {
            Bitmap = ResourceManager.Request<BitmapResource>(pAlias);
        }

        public void SetBitmap(BitmapResource pBitmap)
        {
            Bitmap = pBitmap;
        }

        protected override void DoDraw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawBitmap(Bitmap, 1, GameObject.ScreenPosition, GameObject.Scale * Game.ActiveCamera.Zoom);
        }

        protected override void DoDraw(IGraphicsAdapter pSystem, Effect pEffect)
        {
            pSystem.DrawImage(Bitmap, pEffect, GameObject.ScreenPosition);
        }

        public override void Dispose()
        {
            ResourceManager.DisposeResource(Bitmap.Alias);
        }
    }
}
