﻿using System.Collections.Generic;

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
        public RenderMethods Method
        {
            get { return RenderMethods.DirectX; }
        }

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
                var defaultDevice = new Device(DriverType.Hardware, DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport);

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

                _swapChain = new SwapChain1(dxgiFactory2, Device, pForm.Handle, ref desc, null);
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

        public void DrawText(string pText, Rectangle pRect, Font pFont, bool pClip)
        {
            var opts = pClip ? DrawTextOptions.Clip : DrawTextOptions.None;
            Context.DrawText(pText, pFont.Format, pRect, pFont.Brush, opts);
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
            Context.DrawBitmap(pBitmap.DirectXBitmap, new RawRectangleF(x, y, x + pScale.Width, y + pScale.Height), pOpacity, BitmapInterpolationMode.NearestNeighbor);
        }

        public void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Size pScale, Rectangle pSourceRect)
        {
            var x = pPosition.X;
            var y = pPosition.Y;
            Context.DrawBitmap(pBitmap.DirectXBitmap,
                                      new RawRectangleF(x, y, x + pScale.Width, y + pScale.Height),
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
            Context.FillRectangle(new RawRectangleF(pPoint.X - 1, pPoint.Y - 1, pPoint.X + 1, pPoint.Y + 1), pBrush.FillColorBrush);
        }

        public void DrawLine(Vector2 pPoint1, Vector2 pPoint2, Brush pBrush)
        {
            Context.DrawLine(pPoint1, pPoint2, pBrush.FillColorBrush);
        }

        public void DrawRect(Rectangle pRect, Brush pBrush)
        {
            if(pBrush.FillColorBrush != null) Context.FillRectangle(pRect, pBrush.FillColorBrush);
            if(pBrush.OutlineColorBrush != null) Context.DrawRectangle(pRect, pBrush.OutlineColorBrush, pBrush.OutlineSize);
        }

        public void DrawElipse(Vector2 pPoint, float pRadius, Brush pBrush)
        {
            var e = new Ellipse(pPoint, pRadius, pRadius);
            if (pBrush.FillColorBrush != null) Context.FillEllipse(e, pBrush.FillColorBrush);
            if(pBrush.OutlineColorBrush != null) Context.DrawEllipse(e, pBrush.OutlineColorBrush);
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
        #endregion
    }
}