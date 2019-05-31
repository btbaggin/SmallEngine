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
        public Image(string pAlias) : this(null, pAlias) { }

        public Image(string pName, string pAlias) : base(pName)
        {
            _bitmap = ResourceManager.Request<BitmapResource>(pAlias);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawBitmap(_bitmap, 1, Position, Bounds.Size);
        }

        public override void Update() { }

        public override void Dispose()
        {
            _bitmap.Dispose();
            base.Dispose();
        }
    }
}