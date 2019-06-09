using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    public class Pen : IDisposable
    {
        internal SolidColorBrush DirectXBrush { get; private set; }

        public Color Color
        {
            get { return DirectXBrush.Color; }
            set { DirectXBrush.Color = value; }
        }

        public float Size { get; set; }

        private Pen(Color pColor, float pOutlineSize, IGraphicsAdapter pTarget)
        {
            if (pTarget.Method == RenderMethods.DirectX)
            {
                var dx = (DirectXAdapter)pTarget;
                DirectXBrush = new SolidColorBrush(dx.Context, new SharpDX.Color4(pColor.R / 255f, pColor.G / 255f, pColor.B / 255f, pColor.A / 255f));
                DirectXBrush.Opacity = pColor.A / 255f;
            }
            else
            {
                throw new NotImplementedException();
            }

            Size = pOutlineSize;
        }


        public static Pen Create(Color pColor, float pOutlineSize, IGraphicsAdapter pAdapter)
        {
            return new Pen(pColor, pOutlineSize, pAdapter);
        }

        public void Dispose()
        {
            DirectXBrush.Dispose();
        }
    }
}
