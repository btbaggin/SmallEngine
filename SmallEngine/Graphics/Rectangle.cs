using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    [Serializable]
    public struct Rectangle
    {
        #region Properties
        public float X
        {
            get { return Location.X; }
        }

        public float Y
        {
            get { return Location.Y; }
        }

        public float Width
        {
            get { return Size.Width; }
        }

        public float Height
        {
            get { return Size.Height; }
        }

        public float Left
        {
            get { return X; }
        }

        public float Top
        {
            get { return Location.Y; }
        }
        
        public float Right
        {
            get { return X + Width; }
        }

        public float Bottom
        {
            get { return Y + Height; }
        }

        public Vector2 Location;

        public Size Size;
        #endregion

        #region Constructors
        public Rectangle(Vector2 pLocation, Size pSize)
        {
            Location = pLocation;
            Size = pSize;
        }

        public Rectangle(Vector2 pLocation, float pWidth, float pHeight)
        {
            Location = pLocation;
            Size = new Size(pWidth, pHeight);
        }

        public Rectangle(float pX, float pY, float pWdith, float pHeight)
        {
            Location = new Vector2(pX, pY);
            Size = new Size(pWdith, pHeight);
        }
        #endregion

        public bool Contains(float pX, float pY)
        {
            return Left <= pX && Top <= pY && Right >= pX && Bottom >= pY;
        }

        public bool Contains(Vector2 pPoint)
        {
            return Left <= pPoint.X && Top <= pPoint.Y && Right >= pPoint.X && Bottom >= pPoint.Y;
        }

        public bool Contains(Rectangle pRect)
        {
            return Left <= pRect.Left && Top <= pRect.Top && Right >= pRect.Right && Bottom >= pRect.Bottom;
        }

        public bool IntersectsWith(Rectangle pRect)
        {
            return Contains(pRect.Location) || Contains(pRect.Right, pRect.Bottom);
        }

        public Rectangle Grow(Vector2 pAmount)
        {
            return Grow(pAmount.X, pAmount.Y);
        }

        public Rectangle Grow(float pX, float pY)
        {
            return new Rectangle(Location, Size.Width + pX, Size.Height + pY);
        }

        #region Operators
        public static Rectangle operator +(Rectangle pR, Vector2 pV)
        {
            return new Rectangle(pR.X + pV.X, pR.Y + pV.Y, pR.Width, pR.Height);
        }

        public static Rectangle operator +(Vector2 pV, Rectangle pR)
        {
            return new Rectangle(pR.X + pV.X, pR.Y + pV.Y, pR.Width, pR.Height);
        }

        public static Rectangle operator *(Rectangle pR, Vector2 pV)
        {
            return new Rectangle(pR.X * pV.X, pR.Y * pV.Y, pR.Width, pR.Height);
        }

        public static Rectangle operator *(Vector2 pV, Rectangle pR)
        {
            return new Rectangle(pR.X * pV.X, pR.Y * pV.Y, pR.Width, pR.Height);
        }

        public static Rectangle operator /(Rectangle pR, Vector2 pV)
        {
            return new Rectangle(pR.X / pV.X, pR.Y / pV.Y, pR.Width, pR.Height);
        }

        public static Rectangle operator /(Vector2 pV, Rectangle pR)
        {
            return new Rectangle(pR.X / pV.X, pR.Y / pV.Y, pR.Width, pR.Height);
        }

        public static explicit operator System.Drawing.RectangleF(Rectangle pRect)
        {
            return new System.Drawing.RectangleF(pRect.X, pRect.Y, pRect.Width, pRect.Height);
        }

        public static implicit operator SharpDX.Mathematics.Interop.RawRectangleF(Rectangle pRect)
        {
            return new SharpDX.Mathematics.Interop.RawRectangleF(pRect.Left, pRect.Top, pRect.Right, pRect.Bottom);
        }

        public static implicit operator SharpDX.Mathematics.Interop.RawRectangleF?(Rectangle? pRect)
        {
            if (!pRect.HasValue) return null;
            return new SharpDX.Mathematics.Interop.RawRectangleF(pRect.Value.Left, pRect.Value.Top, pRect.Value.Right, pRect.Value.Bottom);
        }

        public static implicit operator SharpDX.Mathematics.Interop.RawRectangle (Rectangle pRect)
        {
            return new SharpDX.Mathematics.Interop.RawRectangle((int)pRect.Left, (int)pRect.Top, (int)pRect.Right, (int)pRect.Bottom);
        }
        #endregion
    }
}
