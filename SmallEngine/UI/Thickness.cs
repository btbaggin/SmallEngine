using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.UI
{
    public struct Thickness
    {
        public byte Left { get; set; }
        public byte Top { get; set; }
        public byte Right { get; set; }
        public byte Bottom { get; set; }

        public int Width
        {
            get { return Left + Right; }
        }

        public int Height
        {
            get { return Top + Bottom; }
        }

        public Thickness(byte pLeft, byte pTop, byte pRight, byte pBottom)
        {
            Left = pLeft;
            Top = pTop;
            Right = pRight;
            Bottom = pBottom;
        }

        public Thickness(byte pLeftRight, byte pTopBottom) : this(pLeftRight, pTopBottom, pLeftRight, pTopBottom) { }

        public Thickness(byte pSize) : this(pSize, pSize, pSize, pSize) { }
    }
}
