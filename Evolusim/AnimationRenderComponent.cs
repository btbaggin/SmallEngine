using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class AnimationRenderComponent  : BitmapRenderComponent
    {
        int _currentFrame;
        int _maxFrames;
        Vector2 _frameSize;
        float _frameDuration;
        Action<AnimationRenderComponent> _evaluator;

        float _frameTimer;

        public int AnimationNum { get; set; }

        public AnimationRenderComponent()
        {
        }

        public void SetAnimation(int pMaxFrames, Vector2 pFrameSize, float pFrameDuration, Action<AnimationRenderComponent> pEvaluator)
        {
            _maxFrames = pMaxFrames - 1;
            _frameSize = pFrameSize;
            _frameDuration = pFrameDuration;
            _evaluator = pEvaluator;
        }

        protected override void DoDraw(IGraphicsSystem pSystem)
        {
            pSystem.DrawBitmap(Bitmap, 
                               Opacity, 
                               GameObject.ScreenPosition, 
                               GameObject.Scale * Game.ActiveCamera.Zoom, 
                               new Rectangle(_currentFrame * _frameSize.X, AnimationNum * _frameSize.Y, _frameSize.X, _frameSize.Y));
        }

        protected override void DoDraw(IGraphicsSystem pSystem, Effect pEffect)
        {
            throw new NotImplementedException();
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);
            SceneManager.Current.AddUpdatable(this);
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            SceneManager.Current.RemoveUpdatable(this);//TODO i don't like this. Use weak references?
        }

        public override void Update(float pDeltaTime)
        {
            _evaluator.Invoke(this);
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
    }
}
