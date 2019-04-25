using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class ImageElement : UIElement
    {
        readonly BitmapResource _bitmap;
        public ImageElement(string pAlias)
        {
            _bitmap = ResourceManager.Request<BitmapResource>(pAlias);
            SetLayout();
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            base.Draw(pSystem);
            pSystem.DrawBitmap(_bitmap, 1, Position, new Vector2(Width, Height));
        }
    }
}
