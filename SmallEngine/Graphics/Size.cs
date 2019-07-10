using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    [Serializable]
    public struct Size
    {
        public float Width { get; set; }

        public float Height { get; set; }

        public bool IsZero
        {
            get { return Width == 0 && Height == 0; }
        }

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

        public static bool operator ==(Size pSize1, Size pSize2)
        {
            return pSize1.Width == pSize2.Width && pSize1.Height == pSize2.Height;
        }

        public static bool operator !=(Size pSize1, Size pSize2)
        {
            return !(pSize1 == pSize2);
        }

        public override string ToString()
        {
            return $"Width: {Width} Height: {Height}";
        }
    }
}
