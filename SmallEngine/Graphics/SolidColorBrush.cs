using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace SmallEngine.Graphics
{
    public sealed class SolidColorBrush : Brush
    {
        readonly SharpDX.Direct2D1.SolidColorBrush _brush;
        internal override SharpDX.Direct2D1.Brush DirectXBrush => _brush;

        readonly static Dictionary<int, SolidColorBrush> _cache = new Dictionary<int, SolidColorBrush>();

        public Color Color
        {
            get { return _brush.Color; }
            set { _brush.Color = value; }
        }

        private SolidColorBrush(Color pColor, IGraphicsAdapter pTarget)
        {
            if (pTarget.Method == RenderMethods.DirectX)
            {
                var dx = (DirectXAdapter)pTarget;
                _brush = new SharpDX.Direct2D1.SolidColorBrush(dx.Context, new SharpDX.Color4(pColor.R / 255f, pColor.G / 255f, pColor.B / 255f, pColor.A / 255f));
                _brush.Opacity = pColor.A / 255f;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static SolidColorBrush Create(Color pColor)
        {
            if (!_cache.ContainsKey(pColor._color)) _cache.Add(pColor._color, new SolidColorBrush(pColor, Game.Graphics));
            return _cache[pColor._color];
        }

        public override void Dispose() { }

        internal static void DisposeAllBrushes()
        {
            foreach (var kv in _cache) kv.Value.DirectXBrush.Dispose();
        }
    }
}
