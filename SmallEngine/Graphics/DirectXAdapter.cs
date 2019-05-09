using System.Collections.Generic;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SmallEngine.Graphics
{
    public class DirectXAdapter : IGraphicsAdapter
    {
        private Image _renderTarget;
        private SwapChain _swapChain;
        private SharpDX.Direct3D11.DeviceContext1 _d3Context;
        private Surface _backBuffer;

        private SharpDX.Direct2D1.Device _2dDevice;

        #region Properties
        public SharpDX.Direct3D11.Device1 Device { get; private set; }

        public SharpDX.Direct2D1.DeviceContext Context { get; private set; }

        public SharpDX.Direct2D1.Factory Factory2D { get; private set; }

        public SharpDX.DirectWrite.Factory FactoryDWrite { get; private set; }
        #endregion

        #region Creation functions
        //https://www.gamedev.net/forums/topic/648058-sharpdx-direct2d-example-with-devicecontext-and-hwnd/
        private GameForm _form;
        public bool Initialize(GameForm pForm, bool pFullScreen)
        {
            _form = pForm;
            _form.WindowSizeChanged += Resize;
            try
            {
                var defaultDevice = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport);

                Device = defaultDevice.QueryInterface<SharpDX.Direct3D11.Device1>();
                _d3Context = Device.ImmediateContext.QueryInterface<SharpDX.Direct3D11.DeviceContext1>();

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
                    BufferCount = 2,
                    Scaling = Scaling.None,
                    SwapEffect = SwapEffect.FlipSequential,
                    Usage = Usage.RenderTargetOutput
                };

                _swapChain = new SharpDX.DXGI.SwapChain1(dxgiFactory2, Device, pForm.Handle, ref desc, null);
                _2dDevice = new SharpDX.Direct2D1.Device(dxgiDevice2);
                Context = new SharpDX.Direct2D1.DeviceContext(_2dDevice, DeviceContextOptions.None);
                Factory2D = new SharpDX.Direct2D1.Factory(FactoryType.SingleThreaded);
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
                _backBuffer.Dispose();
                _renderTarget.Dispose();
                Context.Dispose();
                _swapChain.ResizeBuffers(0, 0, 0, Format.Unknown, SwapChainFlags.AllowModeSwitch);

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
            Context.DrawText(pText, pFont.Format, pRect, pFont.Brush);
        }

        public BitmapResource FromByte(byte[] pData, int pWidth, int pHeight)
        {
            Bitmap b = new Bitmap(Context, new Size2(pWidth, pHeight), new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
            b.CopyFromMemory(pData, pWidth * 4);
            var br = new BitmapResource() { DirectXBitmap = b };
            return br;
        }

        public Bitmap LoadBitmap(string pFile, out int pWidth, out int pHeight)
        {
            System.Diagnostics.Debug.Assert(System.IO.File.Exists(pFile));

            // Loads from file using System.Drawing.Image
            using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(pFile))
            {
                pWidth = bitmap.Width;
                pHeight = bitmap.Height;
                var sourceArea = new System.Drawing.Rectangle(0, 0, pWidth, pHeight);
                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied));
                var size = new Size2(pWidth, pHeight);

                // Transform pixels from BGRA to RGBA
                int stride = pWidth * sizeof(int);
                using (var tempStream = new DataStream(pHeight * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < pHeight; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < pWidth; x++)
                        {
                            // Not optimized 
                            byte B = System.Runtime.InteropServices.Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = System.Runtime.InteropServices.Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = System.Runtime.InteropServices.Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = System.Runtime.InteropServices.Marshal.ReadByte(bitmapData.Scan0, offset++);
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

        public void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Vector2 pScale)
        {
            var x = pPosition.X;
            var y = pPosition.Y;
            Context.DrawBitmap(pBitmap.DirectXBitmap, new RawRectangleF(x, y, x + pScale.X, y + pScale.Y), pOpacity, BitmapInterpolationMode.NearestNeighbor);
        }

        public void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Vector2 pScale, Rectangle pSourceRect)
        {
            var x = pPosition.X;
            var y = pPosition.Y;
            Context.DrawBitmap(pBitmap.DirectXBitmap,
                                      new RawRectangleF(x, y, x + pScale.X, y + pScale.Y),
                                      pOpacity,
                                      BitmapInterpolationMode.NearestNeighbor,
                                      pSourceRect);
        }

        public void DrawImage(BitmapResource pBitmap, Effect pEffect, Vector2 pPosition)
        {
            pEffect.Draw(pBitmap, pPosition);
        }

        public void DrawPoint(Vector2 pPoint, Brush pBrush)
        {
            Context.FillRectangle(new RawRectangleF(pPoint.X - 1, pPoint.Y - 1, pPoint.X + 1, pPoint.Y + 1), pBrush.ColorBrush);
        }

        public void DrawLine(Vector2 pPoint1, Vector2 pPoint2, Brush pBrush)
        {
            Context.DrawLine(pPoint1, pPoint2, pBrush.ColorBrush);
        }

        public void DrawFillRect(Rectangle pRect, Brush pBrush)
        {
            Context.FillRectangle(pRect, pBrush.ColorBrush);
        }

        public void DrawRect(Rectangle pRect, Brush pBrush, float pStroke)
        {
            Context.DrawRectangle(pRect, pBrush.ColorBrush, pStroke);
        }

        public void DrawElipse(Vector2 pPoint, float pRadius, Brush pBrush)
        {
            Context.DrawEllipse(new Ellipse(pPoint, pRadius, pRadius), pBrush.ColorBrush);
        }

        public void SetTransform(Transform pTransform)
        {
            Context.Transform = pTransform.Rotation;
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
            _swapChain.Present(_form.Vsync ? 1 : 0, PresentFlags.None);
        }

        public void SetFullScreen(bool pFullScreen)
        {
            _swapChain.SetFullscreenState(pFullScreen, null);
        }

        public Font CreateFont(string pFamily, float pSize, Color pColor)
        {
            return Font.Create(FactoryDWrite, Context, pFamily, pSize, pColor);
        }

        public Brush CreateBrush(Color pColor)
        {
            return Brush.Create(pColor, Context);
        }
        #endregion
    }
}