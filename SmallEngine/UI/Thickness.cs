using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.UI
{
    public struct Thickness
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public int Width
        {
            get { return Left + Right; }
        }

        public int Height

        {
            get { return Top + Bottom; }
        }

        public Thickness(int pLeft, int pTop, int pRight, int pBottom)
        {
            Left = pLeft;
            Top = pTop;
            Right = pRight;
            Bottom = pBottom;
        }

        public Thickness(int pLeftRight, int pTopBottom) : this(pLeftRight, pTopBottom, pLeftRight, pTopBottom) { }

        public Thickness(int pSize) : this(pSize, pSize, pSize, pSize) { }
    }
}
