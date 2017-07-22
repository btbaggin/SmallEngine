using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace Evolusim
{
    class Organism : GameObject
    {
        private AnimationRenderComponent _render;
        private MovementComponent _movement;
        private TraitComponent _traits;

        private Terrain.Type _preferredTerrain;
        private int _currentHunger;
        private int _hunger;

        static Organism()
        {
            SceneManager.Define("organism", typeof(AnimationRenderComponent),
                                            typeof(TraitComponent),
                                            typeof(MovementComponent));
        }

        public static Organism Create()
        {
            return SceneManager.Current.CreateGameObject<Organism>("organism");
        }

        public Organism()
        {
            Position = new Vector2(Game.RandomInt(0, Evolusim.WorldSize), Game.RandomInt(0, Evolusim.WorldSize));
            Scale = new Vector2(30);
            _preferredTerrain = Terrain.GetTypeAt(Position); 
        }

        public override void Initialize()
        {
            _render = GetComponent<AnimationRenderComponent>();
            _render.SetBitmap("organism", 4, new Vector2(16, 32));

            _movement = GetComponent<MovementComponent>();

            _traits = GetComponent<TraitComponent>();
            _hunger = _traits.GetTrait<int>(TraitComponent.Traits.Hunger);
            _currentHunger = _hunger;
            Coroutine.Start(UseHunger);
        }

        private float _animationTimer;
        public override void Update(float pDeltaTime)
        {
            _movement.Move(pDeltaTime, _preferredTerrain);
            if (Math.Abs(_movement.Speed.X) - Math.Abs(_movement.Speed.Y) > 0)
            {
                if(_movement.Speed.X > 0)
                {
                    _render.AnimationNum = 3;
                }
                else
                {
                    _render.AnimationNum = 1;
                }
            }
            else
            {
                if(_movement.Speed.Y > 0)
                {
                    _render.AnimationNum = 2;
                }
                else
                {
                    _render.AnimationNum = 0;
                }
            }

            if ((_animationTimer += pDeltaTime) >= .5)
            {
                _render.MoveNextFrame();
                _animationTimer = 0;
            }

            
        }

        public void Eat()
        {
            _currentHunger = _hunger;
        }

        private IEnumerator<WaitEvent> UseHunger()
        {
            while(!MarkedForDestroy)
            {
                _currentHunger -= 1;
                if(_currentHunger <= 0)
                {
                    Destroy();
                }
                else if(_currentHunger <= 10)
                {
                    _movement.Movement = MovementComponent.MovementType.Hungry;
                }
                yield return new WaitForSeconds(1);
            }
        }
    }
}
