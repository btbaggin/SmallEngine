using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class Image : UIElement
    {
        readonly BitmapResource _bitmap;
        public Image(string pAlias)
        {
            _bitmap = ResourceManager.Request<BitmapResource>(pAlias);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawBitmap(_bitmap, 1, Position, new Vector2(Width, Height));
        }

        public override void Update(float pDeltaTime) { }
    }
}
