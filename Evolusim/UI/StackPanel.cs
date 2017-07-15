using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.UI;

namespace Evolusim.UI
{
    class StackPanel : UIElement
    {
        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        private Orientation _orientation;
        public StackPanel(Orientation pOrienation) : base()
        {
            _orientation = pOrienation;
        }

        public override void Measure(SizeF pSize, Vector2 pPosition)
        {
            foreach(var c in Children)
            {
                c.Measure(pSize, pPosition);
                if(_orientation == Orientation.Horizontal)
                {
                    pPosition.X += c.Width;
                    pSize.Width -= c.Width;
                }
                else
                {
                    pPosition.Y += c.Height;
                    pSize.Height -= c.Height;
                }
            }
        }
    }
}
