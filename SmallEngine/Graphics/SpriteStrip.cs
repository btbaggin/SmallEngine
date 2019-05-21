using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    public struct SpriteStrip
    {
        public BitmapResource Bitmap { get; private set; }

        public Size FrameSize { get; private set; }

        public SpriteStrip(BitmapResource pBitmap, Size pSize)
        {
            Bitmap = pBitmap;
            FrameSize = pSize;
        }
    }
}
