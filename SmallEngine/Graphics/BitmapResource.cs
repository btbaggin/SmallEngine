using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    public class BitmapResource : Resource
    {
        public SharpDX.Direct2D1.Bitmap DirectXBitmap { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        internal override void Create()
        {
            if(Game.Render == Game.RenderSystem.DirectX)
            {
                DirectXBitmap = ((DirectXGraphicSystem)Game.Graphics).LoadBitmap(Path, out int width, out int height);
                Width = width;
                Height = height;
            }
            else
            {

            }
        }

        internal override Task CreateAsync()
        {
            return Task.Run(() => Create());
        }

        public override void Dispose()
        {
            DirectXBitmap.Dispose();
        }
    }
}
