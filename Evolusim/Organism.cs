using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Audio;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace Evolusim
{
    class Organism : GameObject
    {
        private AnimationRenderComponent _render;
        private AudioComponent _audio;
        private MovementComponent _movement;
        private TraitComponent _traits;

        private Terrain.Type _preferredTerrain;
        private int _currentHunger;
        private int _hunger;

        private int _lifeTime;
        private int _vision;
        private Brush _visionBrush;

        private BitmapResource _heart;

        private int _currentMate;
        private int _mateTimer;
        private bool _isMating;

        public int Attractive { get; private set; }

        static Organism()
        {
            SceneManager.Define("organism", typeof(AnimationRenderComponent),
                                            typeof(AudioComponent),
                                            typeof(TraitComponent),
                                            typeof(MovementComponent));
        }

        public static Organism Create()
        {
            var go = SceneManager.Current.CreateGameObject<Organism>("organism");
            go.Tag = "Organism";
            return go;
        }

        public Organism()
        {
            Position = new Vector2(Game.RandomInt(0, Evolusim.WorldSize), Game.RandomInt(0, Evolusim.WorldSize));
            Scale = new Vector2(64);
            _preferredTerrain = Terrain.GetTypeAt(Position);
            _visionBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Yellow);
            _heart = ResourceManager.Request<BitmapResource>("heart");
        }

        public override void Initialize()
        {
            _render = GetComponent<AnimationRenderComponent>();
            _render.SetBitmap("organism");
            _render.SetAnimation(4, new Vector2(16, 32), .5f, AnimationEval);

            _audio = GetComponent<AudioComponent>();
            _audio.SetAudio("nom");

            _movement = GetComponent<MovementComponent>();

            _traits = GetComponent<TraitComponent>();
            _hunger = _traits.GetTrait<int>(TraitComponent.Traits.Hunger);

            _lifeTime = _traits.GetTrait<int>(TraitComponent.Traits.Lifetime);
            _vision = _traits.GetTrait<int>(TraitComponent.Traits.Vision);

            Attractive = _traits.GetTrait<int>(TraitComponent.Traits.Attractive);
            var mateRate = _traits.GetTrait<int>(TraitComponent.Traits.MateRate);
            if (mateRate == 0) _mateTimer = _lifeTime;
            else _mateTimer = _lifeTime / mateRate;


            _currentHunger = _hunger;
            Coroutine.Start(LifeCycleTick);
        }

        public override void Update(float pDeltaTime)
        {
            _movement.Move(pDeltaTime, _preferredTerrain);
            _render.Update(pDeltaTime);

            if(InputManager.KeyPressed(Mouse.Left) && IsMouseOver())
            {
                MessageBus.SendMessage(new GameMessage("ToolbarOpen", null));
            }

        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            _render.Draw(pSystem);

            if(_isMating)
            {
                var heartScale = new Vector2(Scale.X / 2, Scale.X / 2) * Game.ActiveCamera.Zoom;
                pSystem.DrawBitmap(_heart, 1, ScreenPosition + new Vector2(heartScale.X / 4, 0),  heartScale);
            }
#if DEBUG
            pSystem.DrawElipse(ScreenPosition, _vision * 64 * Game.ActiveCamera.Zoom, _visionBrush);
            var p = Game.ActiveCamera.ToCameraSpace(_movement._destination);
            pSystem.DrawFillRect(new System.Drawing.RectangleF(p.X, p.Y, 5, 5), _visionBrush);
#endif
        }

        public void Eat(Vegetation pFood)
        {
            _currentHunger = _hunger;
            pFood.Destroy();
            _render.SetBitmap("organism");
            _audio.PlayImmediate();
        }

        public void Mate(Organism pMate)
        {
            Organism.Create();
            System.Diagnostics.Debug.WriteLine("Baby making time!!!");
            _currentMate = _mateTimer;
            _isMating = false;
        }

        private IEnumerator<WaitEvent> LifeCycleTick()
        {
            while(!MarkedForDestroy)
            {
                //Lifetime
                if((_lifeTime -= 1) <= 0)
                {
                    Destroy();
                    break;
                }

                //Hunger
                _currentHunger -= 1;
                if(_currentHunger <= 0)
                {
                    Destroy();
                    break;
                }
                else if(_currentHunger <= 10 && !_isMating)
                {
                    _movement.Movement = MovementComponent.MovementType.Hungry;
                    _render.SetBitmap("organism_hungry");
                }

                //Mating
                _currentMate -= 1;
                if(!_isMating && _currentMate <= 0)
                {
                    _isMating = true;
                    _currentMate = 10;
                    _movement.Movement = MovementComponent.MovementType.Mate;
                }
                else if(_isMating && _currentMate <= 0)
                {
                    _movement.Movement = MovementComponent.MovementType.Wander;
                    _currentMate = _mateTimer;
                    _isMating = false;
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
