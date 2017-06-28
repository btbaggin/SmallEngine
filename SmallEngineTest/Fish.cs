using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Audio;

namespace SmallEngineTest
{
    class Fish : GameObject
    {
        float _speed;
        Random r;
        Vector2 _destination;
        public Fish()
        {
            AddComponent(new BitmapRenderComponent("fish") { Order = 1 });
            r = new Random();
            Coroutine.Start(GenerateBubbles);
            var s = r.Next(20, 100);
            Scale = new Vector2(s, s);
            _speed = r.Next(5, 30);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if(_destination == Vector2.Zero)
            {
                _destination = new Vector2(r.Next(0, 640), r.Next(0, 480));
            }
            Position += Vector2.Lerp(_destination, Position, .1f) * pDeltaTime * _speed;
            if (Math.Abs(Position.X - _destination.X) < 5 && Math.Abs(Position.Y - _destination.Y) < 5)
            {
                _destination = Vector2.Zero;
            }
        }

        private IEnumerator<WaitEvent> GenerateBubbles()
        {
            while(true)
            {
                //TODO somethingh something paused?
                ResourceManager.RequestFromGroup<AudioResource>("bubbles").Play(.4f);
                yield return new WaitForSeconds((float)r.NextDouble() * 10f);
            }
            
        }
    }
}
