using SharpDX;
using System;

namespace SmallEngine.Graphics
{
    public enum RenderMethods
    {
        DirectX,
        OpenGL
    }

    public interface IGraphicsAdapter : IDisposable
    {
        RenderMethods Method { get; }
        
        bool Initialize(GameForm pForm, bool pFullScreen);
        void Resize(object sender, WindowEventArgs pE);
        void BeginDraw();
        void EndDraw();

        void DrawText(string pText, Rectangle pRect, Font pFont);
        void DrawText(string pText, Rectangle pRect, Font pFont, bool pClip);

        void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Size pScale);

        BitmapResource TileBitmap(BitmapResource pBitmap, int pTileWidth, int pTileHeight, int pXCount, int pYCount);

        void DrawLine(Vector2 pPoint1, Vector2 pPoint2, Pen pBrush);
        void DrawRect(Rectangle pRect, Brush pBrush);
        void DrawElipse(Vector2 pPoint, float pRadius, Brush pBrush);
        void SetFullScreen(bool pFullScreen);
        void SetTransform(Transform pTransform);
        void ResetTransform();
    }

    public static class GraphicsAdapterFactory
    {
        public static IGraphicsAdapter Create(RenderMethods pMethod)
        {
            switch(pMethod)
            {
                case RenderMethods.DirectX:
                    return new DirectXAdapter();

                case RenderMethods.OpenGL:
                    throw new NotImplementedException();

                default:
                    throw new UnknownEnumException(typeof(RenderMethods), pMethod);
            }
        }
    }
}
