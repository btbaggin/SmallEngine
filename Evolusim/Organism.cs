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
        [Flags]
        public enum Status
        {
            None,
            Hungry,
            Sleeping,
            Mating
        }

        public Status OrganismStatus { get; private set; }

        private AnimationRenderComponent _render;
        private AudioComponent _audio;
        private MovementComponent _movement;
        private TraitComponent _traits;

        private TerrainType _preferredTerrain;

        private int _hunger;
        private int _currentHunger;
        private float _hungerPercent;

        private int _stamina;
        private int _currentStamina;
        private float _staminaPercent;

        private int _mate;
        private int _currentMate;
        private float _matePercent;

        private int _lifeTime;
        private int _vision;
        private Brush _visionBrush;

        private BitmapResource _sleep;
        private BitmapResource _heart;
        private BitmapResource _hungry;

        public int Attractive { get; private set; }

        public static Organism SelectedOrganism { get; set; }

        public override int Order => 10;

        #region Constructors
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
            _sleep = ResourceManager.Request<BitmapResource>("sleep");
        }
        #endregion

        public override void Initialize()
        {
            _render = GetComponent<AnimationRenderComponent>();
            _render.SetBitmap("organism");
            _render.SetAnimation(4, new Vector2(16, 32), .5f, AnimationEval);

            _audio = GetComponent<AudioComponent>();
            _audio.SetAudio("nom");

            _movement = GetComponent<MovementComponent>();

            _traits = GetComponent<TraitComponent>();
            _hunger = (int)_traits.GetTrait(TraitComponent.Traits.Hunger).Value;

            _lifeTime = (int)_traits.GetTrait(TraitComponent.Traits.Lifetime).Value;
            _vision = (int)_traits.GetTrait(TraitComponent.Traits.Vision).Value;
            _stamina = (int)_traits.GetTrait(TraitComponent.Traits.Stamina).Value;

            Attractive = (int)_traits.GetTrait(TraitComponent.Traits.Attractive).Value;
            var mateRate = (int)_traits.GetTrait(TraitComponent.Traits.MateRate).Value;
            if (mateRate == 0) _mate = _lifeTime;
            else _mate = _lifeTime / mateRate;

            _currentMate = _mate;
            _currentHunger = _hunger;
            _currentStamina = _stamina;
            Coroutine.Start(LifeCycleTick);
        }

        public override void Update(float pDeltaTime)
        {
            _movement.Move(pDeltaTime, _preferredTerrain);
            _render.Update(pDeltaTime);

            if(InputManager.KeyPressed(Mouse.Left) && InputManager.IsFocused(this))
            {
                MessageBus.SendMessage(new GameMessage("ToolbarOpen", this));
            }
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            _render.Draw(pSystem);

            var scale = new Vector2(Scale.X / 2, Scale.X / 2) * Game.ActiveCamera.Zoom;
            var pos = ScreenPosition + new Vector2(scale.X / 2, -scale.Y / 2);
            if(OrganismStatus.HasFlag(Status.Sleeping))
            {
                pSystem.DrawBitmap(_sleep, 1, pos, scale);
            }
            else if(OrganismStatus.HasFlag(Status.Hungry))
            {
                pSystem.DrawBitmap(_sleep, 1, pos, scale);
            }
            else if(OrganismStatus.HasFlag(Status.Mating))
            {
                pSystem.DrawBitmap(_sleep, 1, pos, scale);
            }

#if DEBUG
            pSystem.DrawElipse(ScreenPosition, _vision * 64 * Game.ActiveCamera.Zoom, _visionBrush);
            var p = Game.ActiveCamera.ToCameraSpace(_movement._destination);
            pSystem.DrawFillRect(new Rectangle(p, 5, 5), _visionBrush);
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
            _currentMate = _mate;
            OrganismStatus = Status.None;
        }

        public void TrySleep()
        {
            //TODO maybe look around for stuff
            if(OrganismStatus.HasFlag(Status.Hungry) && _hungerPercent > .4f)
            {
                OrganismStatus |= Status.Sleeping;
            }
        }

        public void MoveTo(Vector2 pPosition)
        {
            if (OrganismStatus.HasFlag(Status.Sleeping)) return;
            _movement.MoveTo(pPosition);
        }

        public Traits.Trait GetTrait(TraitComponent.Traits pTrait)
        {
            return _traits.GetTrait(pTrait);
        }

        public Status GetMovementType()
        {
            if (OrganismStatus.HasFlag(Status.Sleeping)) return Status.Sleeping;
            if (OrganismStatus.HasFlag(Status.Hungry)) return Status.Hungry;
            if (OrganismStatus.HasFlag(Status.Mating)) return Status.Mating;
            return Status.None;
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

                //If we are sleeping, just regen stamina, nothing else
                if (OrganismStatus.HasFlag(Status.Sleeping))
                {
                    _staminaPercent += .1f;
                    if (_staminaPercent >= 1f)
                    {
                        _currentStamina = _stamina + 1; //Add one because we are going to subtract 1
                        OrganismStatus &= ~Status.Sleeping;
                    }
                    else
                    {
                        goto yield;
                    }
                }

                _currentMate -= 1;
                _currentHunger -= 1;
                _currentStamina -= 1;

                _hungerPercent = _currentHunger / _hunger;
                _staminaPercent = _currentStamina / _stamina;
                _matePercent = _currentStamina / _mate;

                //Hard sleep check
                if (_staminaPercent <= 0)
                {
                    //Go to sleep no matter what if we have no stamina
                    OrganismStatus |= Status.Sleeping;
                    goto yield;
                }

                //*** Hunger
                if (_hungerPercent <= 0)
                { 
                    Destroy();
                    break;
                }
                else if(_hungerPercent <= .25f)
                {
                    //Start seeking food no matter what
                    OrganismStatus = Status.Hungry;
                    goto yield;
                }
                else if(_hungerPercent <= .75)
                {
                    //Start seeking food...
                    OrganismStatus |= Status.Hungry;
                    goto yield;
                }

                //*** Stamina
                if(_currentStamina <= .3)
                {
                    TrySleep();
                    goto yield;
                }

                //*** Mating
                if(_currentMate <= .4f)
                {
                    OrganismStatus |= Status.Mating;
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
