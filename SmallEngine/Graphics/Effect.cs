using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using SharpDX.DXGI;

namespace SmallEngine.Graphics
{
    public class Effect : IDisposable
    {
        //https://english.r2d2rigo.es/2014/08/19/applying-direct2d-built-in-effects-to-bitmaps-with-sharpdx/
        //https://msdn.microsoft.com/en-us/library/windows/desktop/hh973241(v=vs.85).aspx
        readonly List<SharpDX.Direct2D1.Effect> _effects;
        readonly DeviceContext _context;
        Composite _composite;

        //TODO move effects over to normal HLSL
        public Effect()
        {
            _effects = new List<SharpDX.Direct2D1.Effect>();
            _context = ((DirectXAdapter)Game.Graphics).SecondaryContext;
        }

        public Effect AddSaturation(float pValue)
        {
            _effects.Add(new Saturation(_context) { Value = pValue });
            return this;
        }

        public Effect AddSepia(float pValue)
        {
            var e =new SharpDX.Direct2D1.Effect(_context, SharpDX.Direct2D1.Effect.Sepia);
            e.SetValue(0, pValue);
            _effects.Add(e);
            return this;
        }

        public Effect AddGrayscale()
        {
            _effects.Add(new SharpDX.Direct2D1.Effect(_context, SharpDX.Direct2D1.Effect.Grayscale));
            return this;    
        }

        //TODO open up guids?

        public Effect AddHue(float pValue)
        {
            _effects.Add(new HueRotation(_context) { Angle = pValue });
            return this;
        }

        public Effect AddShadow(float pAmount, System.Drawing.Color pColor)
        {
            _effects.Add(new Shadow(_context)
            {
                BlurStandardDeviation = pAmount,
                Color = new SharpDX.Mathematics.Interop.RawColor4(pColor.R, pColor.G, pColor.B, pColor.A)
            });
            return this;
        }

        public Effect AddBlur(float pAmount)
        {
            _effects.Add(new GaussianBlur(_context) { StandardDeviation = pAmount });
            return this;
        }

        public Effect Create()
        {
            _composite = new Composite(_context);
            _composite.InputCount = 2;
            return this;
        }

        public void Draw(BitmapResource pBitmap, Vector2 pPosition)
        {
            _effects[0].SetInput(0, pBitmap.DirectXBitmap, true);
            for (int i = 1; i < _effects.Count; i++)
            {
                _effects[i].SetInput(0, _effects[i - 1].Output, true);
            }

            _composite.SetInput(0, pBitmap.DirectXBitmap, true);
            _composite.SetInput(1, _effects[_effects.Count - 1].Output, true);

            if(pBitmap.Source.HasValue)
            {
                _context.DrawImage(_composite.Output, pPosition, pBitmap.Source.Value, InterpolationMode.Linear, CompositeMode.SourceOver);
            }
            else
            {
                _context.DrawImage(_composite.Output, pPosition);
            }
        }

        public BitmapResource ApplyTo(BitmapResource pBitmap)
        {
            var b = new Bitmap1(_context,
                new SharpDX.Size2(pBitmap.Width, pBitmap.Height),
                new BitmapProperties1() {
                    PixelFormat = pBitmap.DirectXBitmap.PixelFormat,
                    BitmapOptions = BitmapOptions.Target
                });

            var t = _context.Target;
            _context.Target = b;
            _context.BeginDraw();
            Draw(pBitmap, Vector2.Zero);
            _context.EndDraw();
            _context.Target = t;

            return new BitmapResource() { DirectXBitmap = b };
        }

        public void Dispose()
        {
            foreach(var e in _effects)
            {
                e.Dispose();
            }
            _composite.Dispose();
        }
    }
}
