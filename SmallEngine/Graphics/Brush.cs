using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SmallEngine.Graphics
{
    public abstract class Brush : IDisposable
    {
        internal abstract SharpDX.Direct2D1.Brush DirectXBrush { get; }
        
        public float Opacity
        {
            get { return DirectXBrush.Opacity; }
            set { DirectXBrush.Opacity = value; }
        }

        public void Dispose()
        {
            DirectXBrush.Dispose();
        }
    }
}
