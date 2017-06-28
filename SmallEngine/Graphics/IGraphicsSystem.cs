using System;
using System.Drawing;

namespace SmallEngine.Graphics
{
    public interface IGraphicsSystem : IDisposable
    {
        bool Initialize(GameForm pForm, bool pFullScreen);
        void BeginDraw();
        void EndDraw();
        void DrawBitmap(BitmapResource pBitmap, float pOpacity, Vector2 pPosition, Vector2 pScale);
        void DrawBitmap(BitmapResource pBitmap, float pOpacity, RectangleF pSource, Vector2 pPosition);
        void DrawPoint(Vector2 pPoint, Color pColor);
        void DrawLine(Vector2 pPoint1, Vector2 pPoint2, Color pColor);
        void DrawRect(RectangleF pRect, Color pColor);
        void DrawElipse(Vector2 pPoint, float pRadius, Color pColor);
        void SetFullScreen(bool pFullScreen);
        void DefineColor(Color pColor);
        void SetTransform(float pRotation, Vector2 pCenter);
        void ResetTransform();
        //TODO effects?
    }
}
