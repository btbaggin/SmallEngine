using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    public struct Size
    {
        public float Width { get; set; }

        public float Height { get; set; }

        public Size(float pWidthHeight) : this(pWidthHeight, pWidthHeight) { }

        public Size(float pWidth, float pHeight)
        {
            Width = pWidth;
            Height = pHeight;
        }

        public static implicit operator System.Drawing.Size(Size pSize)
        {
            return new System.Drawing.Size((int)pSize.Width, (int)pSize.Height);
        }

        public static implicit operator Size(System.Drawing.Size pSize)
        {
            return new Size(pSize.Width, pSize.Height);
        }

        public static Size operator *(Size pV1, float pScalar)
        {
            return new Size(pV1.Width * pScalar, pV1.Height * pScalar);
        }

        public static Size operator *(float pScalar, Size pV1)
        {
            return new Size(pV1.Width * pScalar, pV1.Height * pScalar);
        }
    }
}
