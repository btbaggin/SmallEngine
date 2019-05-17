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
    }
}
