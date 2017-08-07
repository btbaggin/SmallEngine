using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class StatusComponent : RenderComponent
    {
        [Flags]
        public enum Status
        {
            None,
            Hungry,
            Sleeping,
            Mating
        }

        private Status _currentStatus;
        private BitmapResource _sleep;
        private BitmapResource _heart;
        private BitmapResource _hungry;

        public StatusComponent()
        {
            _heart = ResourceManager.Request<BitmapResource>("heart");
            _hungry = ResourceManager.Request<BitmapResource>("hungry");
            _sleep = ResourceManager.Request<BitmapResource>("sleep");
        }

        protected override void DoDraw(IGraphicsSystem pSystem)
        {
            var scale = new Vector2(GameObject.Scale.X / 2, GameObject.Scale.Y / 2) * Game.ActiveCamera.Zoom;
            var pos = GameObject.ScreenPosition + new Vector2(scale.X / 2, -scale.Y / 2);
            if (_currentStatus.HasFlag(Status.Sleeping))
            {
                pSystem.DrawBitmap(_sleep, 1, pos, scale);
            }
            else if (_currentStatus.HasFlag(Status.Hungry))
            {
                pSystem.DrawBitmap(_hungry, 1, pos, scale);
            }
            else if (_currentStatus.HasFlag(Status.Mating))
            {
                pSystem.DrawBitmap(_heart, 1, pos, scale);
            }
        }

        public void AddStatus(Status pStatus)
        {
            _currentStatus |= pStatus;
        }

        public void RemoveStatus(Status pStatus)
        {
            _currentStatus &= ~pStatus;
        }

        public void OverrideStatus(Status pStatus)
        {
            _currentStatus = pStatus;
        }

        public bool HasStatus(Status pStatus)
        {
            return _currentStatus.HasFlag(pStatus);
        }

        protected override void DoDraw(IGraphicsSystem pSystem, Effect pEffect)
        {
            throw new NotImplementedException();
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);
        }
    }
}
