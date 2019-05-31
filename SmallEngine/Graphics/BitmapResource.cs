using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    public class BitmapResource : Resource
    {
        SharpDX.Direct2D1.Bitmap _bitmap;
        public SharpDX.Direct2D1.Bitmap DirectXBitmap
        {
            get { return _bitmap; }
            internal set
            {
                _bitmap = value;
                Width = (int)_bitmap.Size.Width;
                Height = (int)_bitmap.Size.Height;
            }
        }

        /// <summary>
        /// Width of the bitmap
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the bitmap
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// A source rectangle to draw from the underlying bitmap
        /// </summary>
        public Rectangle? Source { get; private set; }

        /// <summary>
        /// Creates a new BitmapResource from a subsection of the current bitmap
        /// </summary>
        /// <param name="pRectangle">Area to create from the current bitmap</param>
        /// <returns></returns>
        public BitmapResource CreateSubBitmap(Rectangle pRectangle)
        {
            return new BitmapResource()
            {
                Width = (int)pRectangle.Width,
                Height = (int)pRectangle.Height,
                _bitmap = DirectXBitmap,
                Source = pRectangle
            };
        }

        internal override void Create()
        {
            if(Game.RenderMethod == RenderMethods.DirectX)
            {
                var dx = (DirectXAdapter)Game.Graphics;
                DirectXBitmap = dx.LoadBitmap(Path);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal override Task CreateAsync()
        {
            return Task.Run(() => Create());
        }

        protected override void DisposeResource()
        {
            DirectXBitmap.Dispose();
        }
    }
}
