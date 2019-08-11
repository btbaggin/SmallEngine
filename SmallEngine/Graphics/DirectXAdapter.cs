using System.Collections.Generic;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SmallEngine.Graphics
{
    public sealed class DirectXAdapter : IGraphicsAdapter
    {
        const int BUFFER_COUNT = 2;

        Image _renderTarget;
        SwapChain1 _swapChain;
        Surface _backBuffer;
        DeviceDebug _debug;
        SharpDX.Direct2D1.Device _2dDevice;

        #region Properties
        public RenderMethods Method
        {
            get { return RenderMethods.DirectX; }
        }

        public SharpDX.Direct3D11.Device1 Device { get; private set; }

        public SharpDX.Direct2D1.DeviceContext Context { get; private set; }

        public SharpDX.Direct2D1.DeviceContext SecondaryContext { get; private set; }

        public SharpDX.Direct2D1.Factory Factory2D { get; private set; }

        public SharpDX.DirectWrite.Factory FactoryDWrite { get; private set; }
        #endregion

        static DirectXAdapter()
        {
#if DEBUG
            SharpDX.Configuration.EnableObjectTracking = true;
#endif
        }

        #region Creation functions
        //https://www.gamedev.net/forums/topic/648058-sharpdx-direct2d-example-with-devicecontext-and-hwnd/
        private GameForm _form;
        public bool Initialize(GameForm pForm, bool pFullScreen)
        {
            _form = pForm;
            try
            {
#if DEBUG
                Device defaultDevice = new Device(DriverType.Hardware, DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport);
                _debug = defaultDevice.QueryInterface<DeviceDebug>();
#else
                Device defaultDevice = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
#endif
                Device = defaultDevice.QueryInterface<SharpDX.Direct3D11.Device1>();

                var dxgiDevice2 = Device.QueryInterface<SharpDX.DXGI.Device2>();
                var dxgiFactory2 = dxgiDevice2.Adapter.GetParent<SharpDX.DXGI.Factory2>();

                // SwapChain description
                var desc = new SwapChainDescription1()
                {
                    Width = 0,
                    Height = 0,
                    Format = Format.B8G8R8A8_UNorm,
                    Stereo = false,
                    SampleDescription = new SampleDescription(1, 0),
                    BufferCount = BUFFER_COUNT,
                    Scaling = Scaling.None,
                    SwapEffect = SwapEffect.FlipSequential,
                    Usage = Usage.RenderTargetOutput
                };

                _swapChain = new SwapChain1(dxgiFactory2, Device, pForm.Handle, ref desc, null);
                _2dDevice = new SharpDX.Direct2D1.Device(dxgiDevice2);
                Context = new SharpDX.Direct2D1.DeviceContext(_2dDevice, DeviceContextOptions.EnableMultithreadedOptimizations);
                SecondaryContext = new SharpDX.Direct2D1.DeviceContext(_2dDevice, DeviceContextOptions.None);
#if DEBUG
                Factory2D = new SharpDX.Direct2D1.Factory(FactoryType.SingleThreaded, DebugLevel.Information);
#else
                Factory2D = new SharpDX.Direct2D1.Factory(FactoryType.SingleThreaded);
#endif
                var dpi = Factory2D.DesktopDpi;
                var prop = new BitmapProperties1(new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied), dpi.Height, dpi.Width, BitmapOptions.CannotDraw | BitmapOptions.Target);
                _backBuffer = _swapChain.GetBackBuffer<Surface>(0);
                _renderTarget = new Bitmap1(Context, _backBuffer, prop);
                Context.Target = _renderTarget;

                // Ignore all windows events
                using (Factory factory = _swapChain.GetParent<Factory>())
                {
                    factory.MakeWindowAssociation(pForm.Handle, WindowAssociationFlags.IgnoreAll);
                }

                FactoryDWrite = new SharpDX.DirectWrite.Factory();

                return true;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error creating render target: " + e.Message);
                return false;
            }
        }

        public void Resize(object sender, WindowEventArgs pE)
        {
            try
            {
                Device.ImmediateContext.ClearState();

                _backBuffer.Dispose();
                _renderTarget.Dispose();
                Context.Dispose();

                _swapChain.ResizeBuffers(BUFFER_COUNT, (int)pE.Size.Width, (int)pE.Size.Height, Format.Unknown, SwapChainFlags.AllowModeSwitch);

                Context = new SharpDX.Direct2D1.DeviceContext(_2dDevice, DeviceContextOptions.None);
                var dpi = Factory2D.DesktopDpi;
                var prop = new BitmapProperties1(new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied), dpi.Height, dpi.Width, BitmapOptions.CannotDraw | BitmapOptions.Target);
                _backBuffer = _swapChain.GetBackBuffer<Surface>(0);
                _renderTarget = new Bitmap1(Context, _backBuffer, prop);
                Context.Target = _renderTarget;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error creating render target: " + e.Message);
            }
        }

        public void Dispose()
        {
#if DEBUG
            _debug.ReportLiveDeviceObjects(ReportingLevel.Detail | ReportingLevel.IgnoreInternal);
            _debug.Dispose();
#endif
            Device.Dispose();
            _swapChain.Dispose();
            _backBuffer.Dispose();
            _renderTarget.Dispose();
            Context.Dispose();
            Factory2D.Dispose();
            FactoryDWrite.Dispose();
        }
#endregion

#region Overridden functions
        public void DrawText(string pText, Rectangle pRect, Font pFont)
        {
            Context.DrawText(pText, pFont.Format, pRect, pFont.Brush.DirectXBrush);
        }

        public void DrawText(string pText, Rectangle pRect, Font pFont, bool pClip)
        {
            var opts = pClip ? DrawTextOptions.Clip : DrawTextOptions.None;
            Context.DrawText(pText, pFont.Format, pRect, pFont.Brush.DirectXBrush, opts);
        }

        public void DrawText(string pText, Vector2 pPosition, Font pFont)
        {
            Context.DrawText(pText, pFont.Format, new RawRectangleF(pPosition.X, pPosition.Y, 16535, 16535), pFont.Brush.DirectXBrush);
        }


        public void DrawFixedText(FixedText pText, Vector2 pPoint)
        {
            Context.DrawTextLayout(pPoint, pText.Layout, pText.Brush.DirectXBrush);
        }

        public void DrawFixedText(FixedText pText, Vector2 pPoint, bool pClip)
        {
            var opts = pClip ? DrawTextOptions.Clip : DrawTextOptions.None;
            Context.DrawTextLayout(pPoint, pText.Layout, pText.Brush.DirectXBrush, opts);
        }

        public BitmapResource FromByte(byte[] pData, int pWidth, int pHeight)
        {
            Bitmap b = new Bitmap(Context, new Size2(pWidth, pHeight), new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
            b.CopyFromMemory(pData, pWidth * 4);
            return new BitmapResource() { DirectXBitmap = b };
        }

        public Bitmap LoadBitmap(string pFile)
        {
            System.Diagnostics.Debug.Assert(System.IO.File.Exists(pFile));

            // Loads from file using System.Drawing.Image
            using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(pFile))
            {
                var width = bitmap.Width;
                var height = bitmap.Height;
                var sourceArea = new System.Drawing.Rectangle(0, 0, width, height);
                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied));
                var size = new Size2(width, height);

                // Transform pixels from BGRA to RGBA
                int stride = width * sizeof(int);
                using (var tempStream = new DataStream(height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < width; x++)
                        {
                            //Convert BGRA to RGBA
                            int bgra = System.Runtime.InteropServices.Marshal.ReadInt32(bitmapData.Scan0, offset);
                            offset += 4;

                            byte B = (byte)bgra;
                            byte G = (byte)(bgra >> 8);
                            byte R = (byte)(bgra >> 16);
                            byte A = (byte)(bgra >> 24);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }

                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;

                    return new Bitmap(Context, size, tempStream, stride, bitmapProperties);
                }
            }
        }

        public void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Size pScale)
        {
            var x = pPosition.X;
            var y = pPosition.Y;
            Context.DrawBitmap(pBitmap.DirectXBitmap, 
                               new RawRectangleF(x, y, x + pScale.Width, y + pScale.Height), 
                               pOpacity, 
                               BitmapInterpolationMode.NearestNeighbor, 
                               pBitmap.Source);
        }

        public BitmapResource TileBitmap(BitmapResource pBitmap, int pTileWidth, int pTileHeight, int pXCount, int pYCount)
        {
            Bitmap b = new Bitmap(Context,
                                  new Size2(pXCount * pTileWidth, pYCount * pTileHeight),
                                  new BitmapProperties(pBitmap.DirectXBitmap.PixelFormat));

            for (int x = 0; x < pXCount; x++)
            {
                for (int y = 0; y < pYCount; y++)
                {
                    var posX = x * pTileWidth;
                    var posY = y * pTileHeight;
                    b.CopyFromBitmap(pBitmap.DirectXBitmap, new RawPoint(posX, posY), pBitmap.Source.Value);
                }
            }

            return new BitmapResource() { DirectXBitmap = b };
        }

        public void DrawPoint(Vector2 pPoint, Brush pBrush)
        {
            Context.FillRectangle(new RawRectangleF(pPoint.X - 1, pPoint.Y - 1, pPoint.X + 1, pPoint.Y + 1), pBrush.DirectXBrush);
        }

        public void DrawLine(Vector2 pPoint1, Vector2 pPoint2, Pen pPen)
        {
            Context.DrawLine(pPoint1, pPoint2, pPen.DirectXBrush, pPen.Size);
        }

        public void DrawRect(Rectangle pRect, Brush pBrush)
        {
            Context.FillRectangle(pRect, pBrush.DirectXBrush);
        }

        public void DrawRectOutline(Rectangle pRect, Pen pPen)
        {
            Context.DrawRectangle(pRect, pPen.DirectXBrush, pPen.Size);
        }

        public void DrawElipse(Vector2 pPoint, float pRadius, Brush pBrush)
        {
            var e = new Ellipse(pPoint, pRadius, pRadius);
            Context.FillEllipse(e, pBrush.DirectXBrush);
        }

        public void DrawElipseOutline(Vector2 pPoint, float pRadius, Pen pPen)
        {
            var e = new Ellipse(pPoint, pRadius, pRadius);
            Context.DrawEllipse(e, pPen.DirectXBrush, pPen.Size);
        }

        public void SetTransform(Transform pTransform)
        {
            Context.Transform = pTransform.Matrix;
        }

        public void ResetTransform()
        {
            SetTransform(Transform.Identity);
        }

        public void BeginDraw()
        {
            Context.BeginDraw();
            Context.Clear(Color4.Black);
        }

        public void EndDraw()
        {
            Context.EndDraw();
            _swapChain.Present(_form.SyncInterval, PresentFlags.None, new PresentParameters());
        }

        public void SetFullScreen(bool pFullScreen)
        {
            _swapChain.SetFullscreenState(pFullScreen, null);
        }
#endregion
    }
}