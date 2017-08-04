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
        Composite _compositeEffect;
        List<SharpDX.Direct2D1.Effect> _effects;
        DeviceContext _context;

        internal SharpDX.Direct2D1.Effect DirectXEffect { get { return _compositeEffect; } }

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

        public void Apply()
        {
            _compositeEffect = new Composite(_context);
            //_compositeEffect.InputCount = 2;
            //for(int i = 1; i < _effects.Count; i++)
            //{
            //    _effects[i - 1].SetInputEffect(0, _effects[i], true);
            //}
            //_compositeEffect.SetInputEffect(0, _effects[_effects.Count - 1], true);
        }

        public void Draw(BitmapResource pBitmap)
        {
            System.Diagnostics.Debug.Assert(_compositeEffect != null);
            _compositeEffect.SetInput(0, pBitmap.DirectXBitmap, true);
        }

        public BitmapResource ApplyTo(BitmapResource pBitmap)
        {
            System.Diagnostics.Debug.Assert(_compositeEffect != null);

            var t = _context.Target;
            var b = new Bitmap1(_context,
                new SharpDX.Size2((int)pBitmap.DirectXBitmap.Size.Width, (int)pBitmap.DirectXBitmap.Size.Height),
                new BitmapProperties1()
                {
                    PixelFormat = new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied),
                    BitmapOptions = BitmapOptions.Target
                });

            _context.BeginDraw();
            _context.Target = b;
            //foreach (var e in _effects)
            //{
            //    e.SetInput(0, pBitmap.DirectXBitmap, true);
            //}
            _effects[0].SetInput(0, pBitmap.DirectXBitmap, true);
            // _compositeEffect.SetInput(1, pBitmap.DirectXBitmap, true);
            //TODO why isnt this working
            _context.DrawImage(_effects[0]);
            _context.EndDraw();

            var retval = new BitmapResource() { DirectXBitmap = b };
            _context.Target = t;
            return retval;
        }

        public void Dispose()
        {
            foreach(var e in _effects)
            {
                e.Dispose();
            }
            _compositeEffect.Dispose();
        }
    }
}
