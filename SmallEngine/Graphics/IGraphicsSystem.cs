using System;
using System.Drawing;

namespace SmallEngine.Graphics
{
    public interface IGraphicsSystem : IDisposable
    {
        bool Initialize(GameForm pForm, bool pFullScreen);
        void BeginDraw();
        void EndDraw();

        void DrawText(string pText, Rectangle pRect, Font pFont);

        void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Vector2 pScale);
        void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Vector2 pScale, Rectangle pSourceRect);
        void DrawImage(Effect pEffect);

        void DrawLine(Vector2 pPoint1, Vector2 pPoint2, Brush pBrush);
        void DrawFillRect(Rectangle pRect, Brush pBrush);
        void DrawRect(Rectangle pRect, Brush pBrush, float pStroke);
        void DrawElipse(Vector2 pPoint, float pRadius, Brush pBrush);
        void SetFullScreen(bool pFullScreen);
        void SetTransform(float pRotation, Vector2 pCenter);
        void ResetTransform();

        Font CreateFont(string pFamily, float pSize, Color pColor);
        Brush CreateBrush(Color pColor);

    }
}
