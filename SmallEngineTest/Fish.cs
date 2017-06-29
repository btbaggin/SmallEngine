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
        HungerComponent _hunger;
        BitmapRenderComponent _render;
        Aquarium _aquarium;
        public Fish(Aquarium pAquarium)
        {
            _render = new BitmapRenderComponent("fish") { Order = 1 };
            AddComponent(_render);
            _hunger = new HungerComponent(20, 7);
            AddComponent(_hunger);
            r = new Random();
            Coroutine.Start(GenerateBubbles);
            var s = r.Next(20, 100);
            Scale = new Vector2(s, s);
            _speed = r.Next(25, 100);
            _aquarium = pAquarium;
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if(_hunger.SearchingForFood)
            {
                _destination = NearestFood();
                _render.Bitmap = ResourceManager.Request<BitmapResource>("fish_hungry");

            }
            else if(_destination == Vector2.Zero)
            {
                _destination = new Vector2(r.Next(0, 640), r.Next(0, 480));
                _render.Bitmap = ResourceManager.Request<BitmapResource>("fish");
            }

            var newPosition = Vector2.Normalize(Vector2.Lerp(Position, _destination, .1f)) * pDeltaTime * _speed;
            Position += newPosition;

            if (Vector2.DistanceSqrd(Position, _destination) < 81)
            {
                if(_hunger.SearchingForFood)
                {
                    _hunger.Eat();
                }
                _destination = Vector2.Zero;
            }
        }

        private Vector2 NearestFood()
        {
            float minDistance = float.MaxValue;
            Vector2 minFood = Vector2.Zero;
            foreach(var f in _aquarium.Food)
            {
                var distance = Vector2.DistanceSqrd(f.Position, Position);
                if (distance < minDistance)
                {
                    minFood = f.Position;
                    minDistance = distance;
                }
            }
            return minFood;
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
