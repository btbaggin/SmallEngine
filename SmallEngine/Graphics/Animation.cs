using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    public class Animation
    {
        public BitmapResource Bitmap { get; private set; }

        public Rectangle Frame { get; private set; }

        readonly Size _frame;
        readonly int _start;
        readonly int _end;
        readonly int _columns;
        readonly float _frameDuration;
        float _frameTimer;
        int _index;

        public Animation(SpriteStrip pStrip, int pStartIndex, int pEndIndex) : this(pStrip, pStartIndex, pEndIndex, 0) { }

        public Animation(SpriteStrip pStrip, int pStartIndex, int pEndIndex, float pFrameDuration)
        {
            Bitmap = pStrip.Bitmap;
            _frame = pStrip.FrameSize;
            _start = pStartIndex;
            _end = pEndIndex;
            _frameDuration = pFrameDuration;

            _columns = (int)(Bitmap.Width / _frame.Width);

            _index = pStartIndex;
            SetFrame();
        }

        public void Update(float pDeltaTime)
        {
            _frameTimer += pDeltaTime;
            if(_frameDuration > 0 && _frameTimer >= _frameDuration)
            {
                _frameTimer -= _frameDuration;
                MoveNext();
            }
            

            SetFrame();
        }

        public void MoveNext()
        {
            _index++;
            if (_index > _end) _index = _start;
        }

        public void Reset()
        {
            _index = _start;
        }

        private void SetFrame()
        {
            int row = 0;
            int column = 0;
            if (_index > 0)
            {
                row = _index / _columns;
                column = _index % _columns; 
            }

            Frame = new Rectangle(column * _frame.Width, row * _frame.Height, _frame.Width, _frame.Height);
        }
    }
}
