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
        public int Health { get; set; }

        private AnimationRenderComponent _render;
        private AudioComponent _audio;
        private MovementComponent _movement;
        private TraitComponent _traits;
        private StatusComponent _status;

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

        private float _lifeTimeTick;
        private int _currentLifetime;
        private int _lifeTime;
        private int _totalHealth;

        private int _vision;
        private Brush _visionBrush;

        public static Organism SelectedOrganism { get; set; }

        public override int Order => 10;

        #region Constructors
        static Organism()
        {
            SceneManager.Define("organism", typeof(AnimationRenderComponent),
                                            typeof(AudioComponent),
                                            typeof(TraitComponent),
                                            typeof(MovementComponent),
                                            typeof(StatusComponent),
                                            typeof(ToolbarComponent));
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
            Scale = new Vector2(TerrainMap.BitmapSize);
            _preferredTerrain = TerrainMap.GetTerrainTypeAt(Position);

            _visionBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Yellow);
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
            _status = GetComponent<StatusComponent>();

            _traits = GetComponent<TraitComponent>();
            _hunger = (int)_traits.GetTrait(TraitComponent.Traits.Hunger).Value;

            _lifeTime = (int)_traits.GetTrait(TraitComponent.Traits.Lifetime).Value;
            _totalHealth = (int)_traits.GetTrait(TraitComponent.Traits.Health).Value;

            _vision = (int)_traits.GetTrait(TraitComponent.Traits.Vision).Value;
            _stamina = (int)_traits.GetTrait(TraitComponent.Traits.Stamina).Value;

            var mateRate = (int)_traits.GetTrait(TraitComponent.Traits.MateRate).Value;
            if (mateRate == 0) _mate = _lifeTime;
            else _mate = _lifeTime / mateRate;

            Health = _totalHealth;
            _currentMate = _mate;
            _currentHunger = _hunger;
            _currentStamina = _stamina;
            _currentLifetime = _lifeTime;
        }

        public override void Update(float pDeltaTime)
        {
            //TODO move all update/draw to components



            if ((_lifeTimeTick += pDeltaTime) >= 1)
            {
                _lifeTimeTick = 0;

                //Lifetime
                if ((_currentLifetime -= 1) <= 0)
                {
                    Destroy();
                    return;
                }

                //If we are sleeping, just regen stamina, nothing else
                if (_status.HasStatus(StatusComponent.Status.Sleeping))
                {
                    _staminaPercent += .1f;
                    if (_staminaPercent >= 1f)
                    {
                        _currentStamina = _stamina + 1; //Add one because we are going to subtract 1
                        _status.RemoveStatus(StatusComponent.Status.Sleeping);
                    }
                    else return;
                }

                _currentMate -= 1;
                _currentHunger -= 1;
                _currentStamina -= 1;

                _hungerPercent = (float)_currentHunger / _hunger;
                _staminaPercent = (float)_currentStamina / _stamina;
                _matePercent = (float)_currentStamina / _mate;

                //Hard sleep check
                if (_staminaPercent <= 0)
                {
                    //Go to sleep no matter what if we have no stamina
                    _status.AddStatus(StatusComponent.Status.Sleeping);
                    return;
                }

                //*** Hunger
                if (_hungerPercent <= 0)
                {
                    Destroy();
                    return;
                }
                else if (_hungerPercent <= .25f)
                {
                    //Start seeking food no matter what
                    _status.OverrideStatus(StatusComponent.Status.Hungry);
                }
                else if (_hungerPercent <= .5)
                {
                    //Start seeking food...
                    _status.AddStatus(StatusComponent.Status.Hungry);
                }

                //*** Stamina
                if (_currentStamina <= .3)
                {
                    TrySleep();
                    return;
                }

                //*** Mating
                if (_currentMate <= .4f)
                {
                    _status.AddStatus(StatusComponent.Status.Mating);
                }
            }
        }

        public void Eat(Vegetation pFood)
        {
            _movement.Stop(1f);
            _currentHunger = Math.Min(_hunger, _currentHunger + pFood.Food);
            pFood.Destroy();
            _audio.PlayImmediate();
            _status.RemoveStatus(StatusComponent.Status.Hungry);
        }

        public void Mate(Organism pMate)
        {
            _movement.Stop(1f);
            CreateFrom(this, pMate);
            _currentMate = _mate;
            _status.RemoveStatus(StatusComponent.Status.Mating);
        }

        public void TrySleep()
        {
            //TODO maybe look around for stuff
            if (_status.HasStatus(StatusComponent.Status.Hungry) && _hungerPercent > .4f)
            {
                _status.AddStatus(StatusComponent.Status.Sleeping);
            }
        }

        public void MoveTo(Vector2 pPosition)
        {
            if (_status.HasStatus(StatusComponent.Status.Sleeping)) return;
            _movement.MoveTo(pPosition);
        }

        public IEnumerable<Tuple<string, float>> GetStats()
        {
            yield return Tuple.Create("Hunger", _hungerPercent);
            yield return Tuple.Create("Stamina", _staminaPercent);
            yield return Tuple.Create("Mate", _matePercent);
            yield return Tuple.Create("Lifetime", (float)_currentLifetime / _lifeTime);
            yield return Tuple.Create("Health", (float)Health / _totalHealth);
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
