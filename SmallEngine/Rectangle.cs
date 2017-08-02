using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public struct Rectangle
    {
        #region Properties
        public float X
        {
            get { return Location.X; }
            set { _location.X = value; }
        }

        public float Y
        {
            get { return Location.Y; }
            set { _location.Y = value; }
        }

        public float Width
        {
            get { return Size.X; }
            set { _size.X = value; }
        }

        public float Height
        {
            get { return Size.Y; }
            set { _size.Y = value; }
        }

        public float Left
        {
            get { return X; }
            set { X = value; }
        }

        public float Top
        {
            get { return Location.Y; }
            set { Y = value; }
        }
        
        public float Right
        {
            get { return X + Width; }
            set { X = value - Width; }
        }

        public float Bottom
        {
            get { return Y + Height; }
            set { Y = value - Height; }
        }

        private Vector2 _location;
        public Vector2 Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private Vector2 _size;
        public Vector2 Size
        {
            get { return _size; }
            set { _size = value; }
        }
        #endregion

        #region Constructors
        public Rectangle(Vector2 pLocation, Vector2 pSize)
        {
            _location = pLocation;
            _size = pSize;
        }

        public Rectangle(Vector2 pLocation, float pWidth, float pHeight)
        {
            _location = pLocation;
            _size = new Vector2(pWidth, pHeight);
        }

        public Rectangle(float pX, float pY, float pWdith, float pHeight)
        {
            _location = new Vector2(pX, pY);
            _size = new Vector2(pWdith, pHeight);
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

        public void Grow(Vector2 pAmount)
        {
            _size += pAmount;
        }

        public void Grow(float pX, float pY)
        {
            _size.X += pX;
            _size.Y += pY;
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

        public static implicit operator SharpDX.Mathematics.Interop.RawRectangleF?(Rectangle pRect)
        {
            return new SharpDX.Mathematics.Interop.RawRectangleF(pRect.Left, pRect.Top, pRect.Right, pRect.Bottom);
        }
        #endregion
    }
}
