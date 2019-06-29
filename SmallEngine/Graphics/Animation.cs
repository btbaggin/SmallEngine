using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    public sealed class Animation
    {
        public BitmapResource Bitmap { get; private set; }

        public Rectangle Frame { get; private set; }

        public int FrameCount
        {
            get { return _end - _start + 1; }
        }

        public bool IsComplete
        {
            get { return _index == _end; }
        }

        readonly Size _frameSize;
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
            _frameSize = pStrip.FrameSize;
            _start = pStartIndex;
            _end = pEndIndex;
            _frameDuration = pFrameDuration;

            _columns = (int)(Bitmap.Width / _frameSize.Width);

            _index = pStartIndex;
            SetFrame();
        }

        //private Animation(SerializationInfo pInfo, StreamingContext pContext)
        //{
        //    Bitmap = ResourceManager.Request<BitmapResource>(pInfo.GetString("Bitmap"));
        //    _frameSize = (Size)pInfo.GetValue("Size", typeof(Size));
        //    _start = pInfo.GetInt32("Start");
        //    _end = pInfo.GetInt32("End");
        //    _frameDuration = pInfo.GetSingle("Duration");
        //    _columns = (int)(Bitmap.Width / _frameSize.Width);

        //    _index = _start;
        //    SetFrame();
        //}

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("Bitmap", Bitmap.Alias);
        //    info.AddValue("Size", _frameSize, typeof(Size));
        //    info.AddValue("Start", _start);
        //    info.AddValue("End", _end);
        //    info.AddValue("Duration", _frameDuration);
        //}

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

        public void SetFrame(int pFrame)
        {
            System.Diagnostics.Debug.Assert(pFrame <= _end);
            System.Diagnostics.Debug.Assert(pFrame >= 0);

            _index = _start + pFrame;
            SetFrame();
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

            Frame = new Rectangle(column * _frameSize.Width, row * _frameSize.Height, _frameSize.Width, _frameSize.Height);
        }
    }
}
