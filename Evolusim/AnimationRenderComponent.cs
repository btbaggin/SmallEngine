using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class AnimationRenderComponent : RenderComponent
    {
        BitmapResource _bitmap;
        int _currentFrame;
        int _maxFrames;
        Vector2 _frameSize;

        public int AnimationNum { get; set; }

        public AnimationRenderComponent()
        {
        }

        public void SetBitmap(string pAlias, int pMaxFrames, Vector2 pFrameSize)
        {
            _bitmap = ResourceManager.Request<BitmapResource>(pAlias);
            _maxFrames = pMaxFrames - 1;
            _frameSize = pFrameSize;
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawBitmap(_bitmap, Opacity, Evolusim.ActiveCamera.ToCameraSpace(GameObject.Position), GameObject.Scale, new System.Drawing.RectangleF(_currentFrame * _frameSize.X,
                                                                                                                                                     AnimationNum * _frameSize.Y,
                                                                                                                                                     _frameSize.X, _frameSize.Y));
        }

        public void MoveNextFrame()
        {
            if (_currentFrame == _maxFrames) _currentFrame = 0;
            else _currentFrame++;
        }

        public void MovePreviousFrame()
        {
            if (_currentFrame == 0) _currentFrame = _maxFrames;
            else _currentFrame--;
        }

        public override void Dispose()
        {
            ResourceManager.Dispose<BitmapResource>(_bitmap.Alias);
        }
    }
}
