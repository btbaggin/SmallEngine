using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

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
            Scale = new Vector2(64);
            _preferredTerrain = Terrain.GetTypeAt(Position); 
        }

        public override void Initialize()
        {
            _render = GetComponent<AnimationRenderComponent>();
            _render.SetBitmap("organism");
            _render.SetAnimation(4, new Vector2(16, 32), .5f, AnimationEval);

            _movement = GetComponent<MovementComponent>();

            _traits = GetComponent<TraitComponent>();
            _hunger = _traits.GetTrait<int>(TraitComponent.Traits.Hunger);
            _currentHunger = _hunger;
            Coroutine.Start(UseHunger);
        }

        public override void Update(float pDeltaTime)
        {
            _movement.Move(pDeltaTime, _preferredTerrain);
            _render.Update(pDeltaTime);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            _render.Draw(pSystem);
        }

        public void Eat(Vegetation pFood)
        {
            _currentHunger = _hunger;
            pFood.Destroy();
            _render.SetBitmap("organism");
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
                    _render.SetBitmap("organism_hungry");
                }
                yield return new WaitForSeconds(1);
            }
        }

        private void AnimationEval()
        {
            if (Math.Abs(_movement.Speed.X) - Math.Abs(_movement.Speed.Y) > 0)
            {
                _render.AnimationNum = _movement.Speed.X > 0 ? 3 : 1;
            }
            else
            {
                _render.AnimationNum = _movement.Speed.Y > 0 ? 2 : 0;
            }
        }
    }
}
