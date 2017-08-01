using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Audio;
using SmallEngine.Graphics;
using SmallEngine.Input;

using Evolusim.Terrain;

namespace Evolusim
{
    class Organism : GameObject
    {
        public enum Status
        {
            None,
            Hungry,
            //Tired,
            Mating
        }

        public Status OrganismStatus { get; private set; }

        private AnimationRenderComponent _render;
        private AudioComponent _audio;
        private MovementComponent _movement;
        private TraitComponent _traits;

        private TerrainType _preferredTerrain;
        private int _currentHunger;
        private int _hunger;

        private int _lifeTime;
        private int _vision;
        private Brush _visionBrush;

        private BitmapResource _heart;
        private BitmapResource _hungry;

        private int _mateDuration;
        private int _currentMate;
        private int _mateTimer;

        public int Attractive { get; private set; }

        public override int Order => 10;

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

        public static Organism CreateFrom(Organism pOrgansihm, Organism pOrganism)
        {
            var go = SceneManager.Current.CreateGameObject<Organism>("organism");
            go.Tag = "Organism";
            go.Position = pOrganism.Position;
            return go;
        }

        public Organism()
        {
            Position = new Vector2(RandomGenerator.RandomInt(0, Evolusim.WorldSize), RandomGenerator.RandomInt(0, Evolusim.WorldSize));
            Scale = new Vector2(64);
            _preferredTerrain = TerrainMap.GetTerrainTypeAt(Position);

            _visionBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Yellow);
            _heart = ResourceManager.Request<BitmapResource>("heart");
            _hungry = ResourceManager.Request<BitmapResource>("hungry");
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

            _currentMate = _mateTimer;
            _currentHunger = _hunger;
            Coroutine.Start(LifeCycleTick);
        }

        public override void Update(float pDeltaTime)
        {
            _movement.Move(pDeltaTime, _preferredTerrain);
            _render.Update(pDeltaTime);

            if(InputManager.KeyPressed(Mouse.Left) && InputManager.IsFocused(this))
            {
                MessageBus.SendMessage(new GameMessage("ToolbarOpen", null));
            }

        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            _render.Draw(pSystem);

            switch(OrganismStatus)
            {
                case Status.None:
                    break;

                case Status.Hungry:
                    var hungryScale = new Vector2(Scale.X / 2, Scale.X / 2) * Game.ActiveCamera.Zoom;
                    pSystem.DrawBitmap(_hungry, 1, ScreenPosition + new Vector2(hungryScale.X / 4, -hungryScale.Y / 2), hungryScale);
                    break;

                case Status.Mating:
                    var heartScale = new Vector2(Scale.X / 2, Scale.X / 2) * Game.ActiveCamera.Zoom;
                    pSystem.DrawBitmap(_heart, 1, ScreenPosition + new Vector2(heartScale.X / 4, -heartScale.Y / 2),  heartScale);
                    break;
            }

#if DEBUG
            pSystem.DrawElipse(ScreenPosition, _vision * 64 * Game.ActiveCamera.Zoom, _visionBrush);
            var p = Game.ActiveCamera.ToCameraSpace(_movement._destination);
            pSystem.DrawFillRect(new System.Drawing.RectangleF(p.X, p.Y, 5, 5), _visionBrush);
#endif
        }

        public void Eat(Vegetation pFood)
        {
            _movement.Stop(1f);
            _currentHunger = _hunger;
            pFood.Destroy();
            _audio.PlayImmediate();
            OrganismStatus = Status.None;
        }

        public void Mate(Organism pMate)
        {
            _movement.Stop(1f);
            Organism.CreateFrom(this, pMate);
            _currentMate = _mateTimer;
            OrganismStatus = Status.None;
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

                _currentMate -= 1;
                _currentHunger -= 1;

                //Hunger
                if(_currentHunger <= 0)
                {
                    Destroy();
                    break;
                }
                else if(_currentHunger <= 10)
                {
                    OrganismStatus = Status.Hungry;
                    goto yield;
                }

                //Mating
                if(_currentMate <= 0)
                {
                    if(OrganismStatus != Status.Mating) _mateDuration = 11;

                    _mateDuration -= 1;
                    OrganismStatus = Status.Mating;
                }
                else if(OrganismStatus == Status.Mating && _mateDuration <= 0)
                {
                    OrganismStatus = Status.None;
                    _currentMate = _mateTimer;
                }

                yield:
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
