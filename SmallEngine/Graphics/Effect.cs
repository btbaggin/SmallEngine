using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using SharpDX.Mathematics.Interop;

namespace SmallEngine.Graphics
{
    public class Effect : IDisposable
    {
        #region Effect Guids
        public static Guid Border => new Guid("2a2d49c0-4acf-43c7-8c6a-7c4a27874d27");
        public static Guid Saturation => new Guid("5cb2d9cf-327d-459f-a0ce-40c0b2086bf7");
        public static Guid ArithmeticComposite => new Guid("fc151437-049a-4784-a24a-f1c4daf20987");
        public static Guid DistantDiffuse => new Guid("3e7efd62-a32d-46d4-a83c-5278889ac954");
        public static Guid Scale => new Guid("9daf9369-3846-4d0e-a44e-0c607934a5d7");
        public static Guid LinearTransfer => new Guid("ad47c8fd-63ef-4acc-9b51-67979c036c06");
        public static Guid Composite => new Guid("48fc9f51-f6ac-48f1-8b58-3b28ac46f76d");
        public static Guid Turbulence => new Guid("cf2bb6ae-889a-4ad7-ba29-a2fd732c9fc9");
        public static Guid HueRotation => new Guid("0f4458ec-4b32-491b-9e85-bd73f44d3eb6");
        public static Guid DistantSpecular => new Guid("428c1ee5-77b8-4450-8ab5-72219c21abda");
        public static Guid Atlas => new Guid("913e2be4-fdcf-4fe2-a5f0-2454f14ff408");
        public static Guid Brightness => new Guid("8cea8d1e-77b0-4986-b3b9-2f0c0eae7887");
        public static Guid Premultiply => new Guid("06eab419-deed-4018-80d2-3e1d471adeb2");
        public static Guid SpotDiffuse => new Guid("818a1105-7932-44f4-aa86-08ae7b2f2c93");
        public static Guid TableTransfer => new Guid("5bf818c3-5e43-48cb-b631-868396d6a1d4");
        public static Guid ColorManagement => new Guid("1a28524c-fdd6-4aa4-ae8f-837eb8267b37");
        public static Guid GaussianBlur => new Guid("1feb6d69-2fe6-4ac9-8c58-1d7f93e7a6a5");
        public static Guid OpacityMetadata => new Guid("6c53006a-4450-4199-aa5b-ad1656fece5e");
        public static Guid DpiCompensation => new Guid("6c26c5c7-34e0-46fc-9cfd-e5823706e228");
        public static Guid Crop => new Guid("e23f7110-0e9a-4324-af47-6a2c0c46f35b");
        public static Guid DirectionalBlur => new Guid("174319a6-58e9-49b2-bb63-caf2c811a3db");
        public static Guid DiscreteTransfer => new Guid("90866fcd-488e-454b-af06-e5041b66c36c");
        public static Guid PointSpecular => new Guid("09c3ca26-3ae2-4f09-9ebc-ed3865d53f22");
        public static Guid DisplacementMap => new Guid("edc48364-0417-4111-9450-43845fa9f890");
        public static Guid LuminanceToAlpha => new Guid("41251ab7-0beb-46f8-9da7-59e93fcce5de");
        public static Guid UnPremultiply => new Guid("fb9ac489-ad8d-41ed-9999-bb6347d110f7");
        public static Guid SpotSpecular => new Guid("edae421e-7654-4a37-9db8-71acc1beb3c1");
        public static Guid ConvolveMatrix => new Guid("407f8c08-5533-4331-a341-23cc3877843e");
        public static Guid Morphology => new Guid("eae6c40d-626a-4c2d-bfcb-391001abe202");
        public static Guid Histogram => new Guid("881db7d0-f7ee-4d4d-a6d2-4697acc66ee8");
        public static Guid ColorMatrix => new Guid("921f03d6-641c-47df-852d-b4bb6153ae11");
        public static Guid Tile => new Guid("b0784138-3b76-4bc5-b13b-0fa2ad02659f");
        public static Guid Blend => new Guid("81c5b77b-13f8-4cdd-ad20-c890547ac65d");
        public static Guid GammaTransfer => new Guid("409444c4-c419-41a0-b0c1-8cd0c0a18e42");
        public static Guid Shadow => new Guid("c67ea361-1863-4e69-89db-695d3e9a5b6b");
        public static Guid Flood => new Guid("61c23c20-ae69-4d8e-94cf-50078df638f2");
        public static Guid BitmapSource => new Guid("5fb6c24d-c6dd-4231-9404-50f4d5c3252d");
        public static Guid PointDiffuse => new Guid("b9e303c3-c08c-4f91-8b7b-38656bc48c20");
        public static Guid YCbCr => new Guid("99503cc1-66c7-45c9-a875-8ad8a7914401");
        public static Guid Exposure => new Guid("b56c8cfa-f634-41ee-bee0-ffa617106004");
        public static Guid Invert => new Guid("e0c3784d-cb39-4e84-b6fd-6b72f0810263");
        public static Guid EdgeDetection => new Guid("eff583ca-cb07-4aa9-ac5d-2cc44c76460f");
        public static Guid LookupTable3D => new Guid("349e0eda-0088-4a79-9ca3-c7e300202020");
        public static Guid Contrast => new Guid("b648a78a-0ed5-4f80-a94a-8e825aca6b77");
        public static Guid Sharpen => new Guid("c9b887cb-c5ff-4dc5-9779-273dcf417c7d");
        public static Guid Vignette => new Guid("c00c40be-5e67-4ca3-95b4-f4b02c115135");
        public static Guid TemperatureTint => new Guid("89176087-8af9-4a08-aeb1-895f38db1766");
        public static Guid HueToRgb => new Guid("7b78a6bd-0141-4def-8a52-6356ee0cbdd5");
        public static Guid Straighten => new Guid("4da47b12-79a3-4fb0-8237-bbc3b2a4de08");
        public static Guid Emboss => new Guid("b1c5eb2b-0348-43f0-8107-4957cacba2ae");
        public static Guid HighlightsShadows => new Guid("cadc8384-323f-4c7e-a361-2e2b24df6ee4");
        public static Guid ChromaKey => new Guid("74c01f5b-2a0d-408c-88e2-c7a3c7197742");
        public static Guid Posterize => new Guid("2188945e-33a3-4366-b7bc-086bd02d0884");
        public static Guid Sepia => new Guid("3a1af410-5f1d-4dbe-84df-915da79b7153");
        public static Guid RgbToHue => new Guid("23f3e5ec-91e8-4d3d-ad0a-afadc1004aa1");
        public static Guid Grayscale => new Guid("36dde0eb-3725-42e0-836d-52fb20aee644");
        public static Guid AffineTransform2D => new Guid("6aa97485-6354-4cfc-908c-e4a74f62c96c");
        public static Guid PerspectiveTransform3D => new Guid("c2844d0b-3d86-46e7-85ba-526c9240f3fb");
        public static Guid Transform3D => new Guid("e8467b04-ec61-4b8a-b5de-d4d73debea5a");
        #endregion

        readonly SharpDX.Direct2D1.Effect _scaleEffect;
        readonly List<SharpDX.Direct2D1.Effect> _effects;
        readonly DeviceContext _context;
        bool _useTiledScaling; //We need to use a special scaling technique for tiled bitmaps otherwise they will grow/shrink with the camera zoom

        public Effect()
        {
            _effects = new List<SharpDX.Direct2D1.Effect>();
            _context = ((DirectXAdapter)Game.Graphics).SecondaryContext;
            _scaleEffect = new SharpDX.Direct2D1.Effect(_context, Scale);
        }

        #region Predefined effects
        public Effect AddSaturation(float pValue)
        {
            _effects.Add(new Saturation(_context) { Value = pValue });
            return this;
        }

        public Effect AddTint(Color pColor)
        {
            var e =  new ColorMatrix(_context);
            var m = new RawMatrix5x4();
            m.M11 = pColor.R * (pColor.A / 255f); //TODO this isn't working
            m.M12 = pColor.G * (pColor.A / 255f);
            m.M13 = pColor.B * (pColor.A / 255f);
            m.M14 = 1;
            e.SetValue(0, m);
            _effects.Add(e);
            return this;
        }

        public Effect AddGrayscale()
        {
            _effects.Add(new SharpDX.Direct2D1.Effect(_context, SharpDX.Direct2D1.Effect.Grayscale));
            return this;    
        }

        public Effect AddShadow(float pAmount, System.Drawing.Color pColor)
        {
            _effects.Add(new Shadow(_context)
            {
                BlurStandardDeviation = pAmount,
                Color = new RawColor4(pColor.R, pColor.G, pColor.B, pColor.A)
            });
            return this;
        }

        public Effect AddBlur(float pAmount)
        {
            _effects.Add(new GaussianBlur(_context) { StandardDeviation = pAmount });
            return this;
        }

        public Effect AddTile(BitmapResource pResource)
        {
            var e = new SharpDX.Direct2D1.Effect(_context, Tile);

            var rect = pResource.Source.HasValue ? pResource.Source.Value : new Rectangle(0, 0, pResource.Width, pResource.Height);
            e.SetValue(0, rect);
            _effects.Add(e);
            _useTiledScaling = true;
            return this;
        }
        #endregion

        #region General effect methods
        public Effect AddEffect(Guid pEffect)
        {
            var e = new SharpDX.Direct2D1.Effect(_context, pEffect);
            _effects.Add(e);
            return this;
        }

        public void SetValue(int pIndex, Color pColor)
        {
            _effects[_effects.Count - 1].SetValue(pIndex, pColor);
        }

        public void SetValue(int pIndex, Vector2 pVector)
        {
            _effects[_effects.Count - 1].SetValue(pIndex, pVector);
        }

        public void SetValue(int pIndex, float pValue)
        {
            _effects[_effects.Count - 1].SetValue(pIndex, pValue);
        }
        #endregion

        public Effect Create()
        {
            for (int i = 1; i < _effects.Count; i++)
            {
                _effects[i].SetInput(0, _effects[i - 1].Output, false);
                _effects[i].Cached = true;
            }
            return this;
        }

        public void Draw(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Size pScale)
        {
            DrawDirectX(((DirectXAdapter)Game.Graphics).Context, pBitmap, pPosition, pScale, pOpacity);
        }

        public BitmapResource ApplyTo(BitmapResource pBitmap, Size pScale)
        {
            var b = new Bitmap1(_context,
                new SharpDX.Size2((int)pScale.Width, (int)pScale.Height),
                new BitmapProperties1() {
                    PixelFormat = pBitmap.DirectXBitmap.PixelFormat,
                    BitmapOptions = BitmapOptions.Target
                });

            var t = _context.Target;
            _context.Target = b;
            _context.BeginDraw();
            DrawDirectX(_context, pBitmap, Vector2.Zero, pScale, 1);
            _context.EndDraw();
            _context.Target = t;

            return new BitmapResource() { DirectXBitmap = b };
        }

        Size _cachedSize;
        private void DrawDirectX(DeviceContext pContext, BitmapResource pBitmap, Vector2 pPosition, Size pSize, float pOpacity)
        {
            //We only need to redo our effects if we have changed sizes
            //Running the effects takes a long time so need to cache when we can
            if(_cachedSize != pSize)
            {
                _effects[0].SetInput(0, pBitmap.DirectXBitmap, false);

                float widthFactor;
                float heightFactor;
                if (_useTiledScaling)
                {
                    widthFactor = Game.ActiveCamera.Zoom;
                    heightFactor = Game.ActiveCamera.Zoom;
                }
                else
                {
                    widthFactor = pSize.Width / pBitmap.Width;
                    heightFactor = pSize.Height / pBitmap.Height;
                }

                _scaleEffect.SetValue(0, new RawVector2(widthFactor, heightFactor));
                _scaleEffect.SetInput(0, _effects[_effects.Count - 1].Output, false);
                _cachedSize = pSize;

                pBitmap.EffectImage = _scaleEffect.Output;
            }

            if (pBitmap.Source.HasValue)
            {
                var r = pBitmap.Source.Value;
                var source = new RawRectangleF(r.Left, r.Top, r.Left + pSize.Width, r.Top + pSize.Height);
                pContext.DrawImage(pBitmap.EffectImage, pPosition, source, InterpolationMode.Linear, CompositeMode.SourceOver);
            }
            else
            {
                pContext.DrawImage(pBitmap.EffectImage, pPosition);
            }
        }

        public void Dispose()
        {
            foreach(var e in _effects)
            {
                e.Dispose();
            }
        }
    }
}
