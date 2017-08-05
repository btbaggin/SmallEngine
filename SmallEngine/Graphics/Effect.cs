using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;

namespace SmallEngine.Graphics
{
    public class Effect : IDisposable
    {
        //https://english.r2d2rigo.es/2014/08/19/applying-direct2d-built-in-effects-to-bitmaps-with-sharpdx/
        //https://msdn.microsoft.com/en-us/library/windows/desktop/hh973241(v=vs.85).aspx
        List<SharpDX.Direct2D1.Effect> _effects;
        DeviceContext _context;
        Composite _composite;

        public Effect()
        {
            _effects = new List<SharpDX.Direct2D1.Effect>();
            _context = ((DirectXGraphicSystem)Game.Graphics).Context;
        }

        public void AddSaturation(float pValue)
        {
            _effects.Add(new Saturation(_context) { Value = pValue });
        }

        public void AddHue(float pValue)
        {
            _effects.Add(new HueRotation(_context) { Angle = pValue });
        }

        public void AddShadow(float pAmount, System.Drawing.Color pColor)
        {
            _effects.Add(new Shadow(_context)
            {
                BlurStandardDeviation = pAmount,
                Color = new SharpDX.Mathematics.Interop.RawColor4(pColor.R, pColor.G, pColor.B, pColor.A)
            });
        }

        public void AddBlur(float pAmount)
        {
            _effects.Add(new GaussianBlur(_context) { StandardDeviation = pAmount });
        }

        public void Create()
        {
            _composite = new Composite(_context);
            _composite.InputCount = 2;
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

            _context.DrawImage(_composite.Output, pPosition);
        }

        public BitmapResource ApplyTo(BitmapResource pBitmap)
        {
            var t = _context.Target;
            var b = new Bitmap1(_context,
                new SharpDX.Size2((int)pBitmap.DirectXBitmap.Size.Width, (int)pBitmap.DirectXBitmap.Size.Height),
                new BitmapProperties1()
                {
                    PixelFormat = new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied),
                    BitmapOptions = BitmapOptions.Target
                });

            _effects[0].SetInput(0, pBitmap.DirectXBitmap, true);
            for (int i = 1; i < _effects.Count; i++)
            {
                _effects[i].SetInput(0, _effects[i - 1].Output, true);
            }

            _composite.SetInput(0, pBitmap.DirectXBitmap, true);
            _composite.SetInput(1, _effects[_effects.Count - 1].Output, true);

            _context.Target = b;
            _context.BeginDraw();
            _context.DrawImage(_composite.Output);
            _context.EndDraw();
            _context.Target = t;

            var retval = new BitmapResource() { DirectXBitmap = b };
            return retval;
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
