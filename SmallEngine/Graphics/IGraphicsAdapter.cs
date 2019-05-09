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
        bool Initialize(GameForm pForm, bool pFullScreen);
        void BeginDraw();
        void EndDraw();

        void DrawText(string pText, Rectangle pRect, Font pFont);

        void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Vector2 pScale);
        void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Vector2 pScale, Rectangle pSourceRect);
        void DrawImage(BitmapResource pBitmap, Effect pEffect, Vector2 pPosition);

        void DrawLine(Vector2 pPoint1, Vector2 pPoint2, Brush pBrush);
        void DrawFillRect(Rectangle pRect, Brush pBrush);
        void DrawRect(Rectangle pRect, Brush pBrush, float pStroke);
        void DrawElipse(Vector2 pPoint, float pRadius, Brush pBrush);
        void SetFullScreen(bool pFullScreen);
        void SetTransform(Transform pTransform);
        void ResetTransform();

        Font CreateFont(string pFamily, float pSize, Color pColor);
        Brush CreateBrush(Color pColor);

    }
}
