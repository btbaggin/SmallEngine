using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class AnimationRenderComponent : RenderComponent, IUpdatable
    {
        BitmapResource _bitmap;
        int _currentFrame;
        int _maxFrames;
        Vector2 _frameSize;
        float _frameDuration;
        Action _evaluator;

        float _frameTimer;

        public int AnimationNum { get; set; }

        public AnimationRenderComponent()
        {
        }

        public void SetBitmap(string pAlias)
        {
            _bitmap = ResourceManager.Request<BitmapResource>(pAlias);
        }

        public void SetAnimation(int pMaxFrames, Vector2 pFrameSize, float pFrameDuration, Action pEvaluator)
        {
            _maxFrames = pMaxFrames - 1;
            _frameSize = pFrameSize;
            _frameDuration = pFrameDuration;
            _evaluator = pEvaluator;
        }

        protected override void DoDraw(IGraphicsSystem pSystem)
        {
            pSystem.DrawBitmap(_bitmap, 
                Opacity, 
                GameObject.ScreenPosition, 
                GameObject.Scale, 
                new System.Drawing.RectangleF(_currentFrame * _frameSize.X, AnimationNum * _frameSize.Y, _frameSize.X, _frameSize.Y));
        }

        public void Update(float pDeltaTime)
        {
            _evaluator.Invoke();
            if((_frameTimer += pDeltaTime) >= _frameDuration)
            {
                MoveNextFrame();
                _frameTimer = 0;
            }
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
