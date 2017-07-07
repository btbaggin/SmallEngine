﻿using System.Collections.Generic;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using Resource = SharpDX.Direct3D11.Resource;
using DeviceContext = SharpDX.Direct3D11.DeviceContext;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SmallEngine.Graphics
{
    class DirectXGraphicSystem : IGraphicsSystem
    {
        private SwapChain _swapChain;
        private Device _device;
        private RenderTargetView _backBufferView;
        private Dictionary<System.Drawing.Color, SolidColorBrush> _colors;

        #region Properties
        public Device Device
        {
            get { return _device; }
        }

        public RenderTargetView RenderTargetView
        {
            get { return _backBufferView; }
        }

        public SharpDX.Direct2D1.Factory Factory2D { get; private set; }

        public SharpDX.DirectWrite.Factory FactoryDWrite { get; private set; }

        public RenderTarget RenderTarget2D { get; private set; }
        #endregion

        #region Creation functions
        private GameForm _form;
        public bool Initialize(GameForm pWindow, bool pFullScreen)
        {
            _colors = new Dictionary<System.Drawing.Color, SolidColorBrush>();

            _form = pWindow;
            _form.WindowSizeChanged += Resize;
            try
            {
                // SwapChain description
                var desc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription =
                        new ModeDescription(pWindow.Width, pWindow.Height,
                                            new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    IsWindowed = !pFullScreen,
                    OutputHandle = pWindow.Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Usage.RenderTargetOutput
                };

                // Create Device and SwapChain
                SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, new[] { FeatureLevel.Level_10_0 }, desc, out _device, out _swapChain);

                // Ignore all windows events
                using (Factory factory = _swapChain.GetParent<Factory>())
                {
                    factory.MakeWindowAssociation(pWindow.Handle, WindowAssociationFlags.IgnoreAll);
                }

                // New RenderTargetView from the backbuffer
                using (var _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0))
                {
                    _backBufferView = new RenderTargetView(_device, _backBuffer);

                    Factory2D = new SharpDX.Direct2D1.Factory();
                    using (var surface = _backBuffer.QueryInterface<Surface>())
                    {
                        RenderTarget2D = new RenderTarget(Factory2D, surface,
                                                        new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
                    }
                }

                RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;

                FactoryDWrite = new SharpDX.DirectWrite.Factory();

                //SceneColorBrush = new SolidColorBrush(RenderTarget2D, Color.White);

                return true;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error creating render target: " + e.Message);
                return false;
            }
        }

        public void Resize(object sender, WindowEventArgs pE)
        {
            try
            {
                RenderTarget2D.Dispose();
                _backBufferView.Dispose();
                _swapChain.ResizeBuffers(2, pE.Size.Width, pE.Size.Height, Format.Unknown, SwapChainFlags.AllowModeSwitch);

                // New RenderTargetView from the backbuffer
                using (var _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0))
                {
                    _backBufferView = new RenderTargetView(_device, _backBuffer);

                    using (var surface = _backBuffer.QueryInterface<Surface>())
                    {
                        RenderTarget2D = new RenderTarget(Factory2D, surface,
                                                        new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
                    }
                }
                RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error creating render target: " + e.Message);
            }
        }

        public void Dispose()
        {
            _device.Dispose();
            _swapChain.Dispose();
            _backBufferView.Dispose();
            RenderTarget2D.Dispose();
            Factory2D.Dispose();
            FactoryDWrite.Dispose();
            foreach(SolidColorBrush c in _colors.Values)
            {
                c.Dispose();
            }
        }
        #endregion

        #region Overridden functions
        public void DrawText(string pText, System.Drawing.Point pPoint, Font pFont)
        {
            //TODO dont hardcode stuff... 
            RenderTarget2D.DrawText(pText, pFont.Format, new RawRectangleF(pPoint.X, pPoint.Y, 100, 100), pFont.Brush);
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

                    return new Bitmap(RenderTarget2D, size, tempStream, stride, bitmapProperties);
                }
            }
        }

        public void DefineColor(System.Drawing.Color pColor)
        {
            if(!_colors.ContainsKey(pColor))
            {
                _colors.Add(pColor, new SolidColorBrush(RenderTarget2D, new Color(pColor.R, pColor.G, pColor.B, pColor.A)));
            }
        }

        public void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Vector2 pScale)
        {
            var x = pPosition.X;
            var y = pPosition.Y;
            RenderTarget2D.DrawBitmap(pBitmap.DirectXBitmap, new RawRectangleF(x, y, x + pScale.X, y + pScale.Y), pOpacity, BitmapInterpolationMode.NearestNeighbor);
        }

        public void DrawBitmap(BitmapResource pBitmap, float pOpacity, System.Drawing.RectangleF pSourceRect, Vector2 pPosition)
        {
            var x = pPosition.X;
            var y = pPosition.Y;
            RenderTarget2D.DrawBitmap(pBitmap.DirectXBitmap, new RawRectangleF(x, y, x + pBitmap.Width, y + pBitmap.Height), pOpacity, BitmapInterpolationMode.NearestNeighbor, new RawRectangleF(pSourceRect.Left, pSourceRect.Top, pSourceRect.Right, pSourceRect.Bottom));
        }

        public void DrawPoint(Vector2 pPoint, System.Drawing.Color pColor)
        {
            System.Diagnostics.Debug.Assert(_colors.ContainsKey(pColor));
            RenderTarget2D.FillRectangle(new RawRectangleF(pPoint.X - 1, pPoint.Y - 1, pPoint.X + 1, pPoint.Y + 1), _colors[pColor]);
        }

        public void DrawLine(Vector2 pPoint1, Vector2 pPoint2, System.Drawing.Color pColor)
        {
            System.Diagnostics.Debug.Assert(_colors.ContainsKey(pColor));
            RenderTarget2D.DrawLine((RawVector2)pPoint1, (RawVector2)pPoint2, _colors[pColor]);
        }

        public void DrawRect(System.Drawing.RectangleF pRect, System.Drawing.Color pColor)
        {
            System.Diagnostics.Debug.Assert(_colors.ContainsKey(pColor));
            RenderTarget2D.FillRectangle(new RawRectangleF(pRect.Left, pRect.Top, pRect.Right, pRect.Bottom), _colors[pColor]);
        }

        public void DrawElipse(Vector2 pPoint, float pRadius, System.Drawing.Color pColor)
        {
            System.Diagnostics.Debug.Assert(_colors.ContainsKey(pColor));
            RenderTarget2D.DrawEllipse(new Ellipse((RawVector2)pPoint, pRadius, pRadius), _colors[pColor]);
        }

        public void SetTransform(float pRotation, Vector2 pCenter)
        {
            RenderTarget2D.Transform = Matrix3x2.Rotation(pRotation, new SharpDX.Vector2(pCenter.X, pCenter.Y));
        }

        public void ResetTransform()
        {
            RenderTarget2D.Transform = Matrix3x2.Identity;
        }

        public void BeginDraw()
        {
            Device.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, _form.Width, _form.Height));
            Device.ImmediateContext.OutputMerger.SetTargets(_backBufferView);
            RenderTarget2D.BeginDraw();
            RenderTarget2D.Clear(new RawColor4(255, 255, 255, 255));
        }

        public void EndDraw()
        {
            RenderTarget2D.EndDraw();
            _swapChain.Present(_form.Vsync ? 1 : 0, PresentFlags.None);
        }

        public void SetFullScreen(bool pFullScreen)
        {
            _swapChain.SetFullscreenState(pFullScreen, null);
        }

        public Font CreateFont(string pFamily, float pSize, System.Drawing.Color pColor)
        {
            return new Font(FactoryDWrite, RenderTarget2D, pFamily, pSize, pColor);
        }
        #endregion
    }
}
