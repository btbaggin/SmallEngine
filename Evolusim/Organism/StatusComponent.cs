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
            None = 0,
            Hungry = 1,
            Sleeping = 2,
            Mating = 4,
            Scared = 8
        }

        [ImportComponent]
        private TraitComponent _traits = null;

        private Status _currentStatus;
        readonly BitmapResource _sleep;
        readonly BitmapResource _heart;
        readonly BitmapResource _hungry;
        readonly BitmapResource _scared;

        private int _hunger;
        private int _currentHunger;
        private float _hungerPercent;

        private int _stamina;
        private int _currentStamina;
        private float _staminaPercent;

        private int _mate;
        private int _currentMate;
        private float _matePercent;

        private int _currentLifetime;
        private int _lifeTime;
        private int _totalHealth;

        private Timer _t = new Timer(1);

        public StatusComponent()
        {
            _heart = ResourceManager.Request<BitmapResource>("heart");
            _hungry = ResourceManager.Request<BitmapResource>("hungry");
            _sleep = ResourceManager.Request<BitmapResource>("sleep");
            _scared = ResourceManager.Request<BitmapResource>("scared");
            Visible = false;
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);

            _traits = (TraitComponent)pGameObject.GetComponent(typeof(TraitComponent));//TODO

            _hunger = (int)_traits.GetTrait(TraitComponent.Traits.Hunger).Value;

            _lifeTime = (int)_traits.GetTrait(TraitComponent.Traits.Lifetime).Value;
            _totalHealth = (int)_traits.GetTrait(TraitComponent.Traits.Health).Value;

            _stamina = (int)_traits.GetTrait(TraitComponent.Traits.Stamina).Value;

            var mateRate = (int)_traits.GetTrait(TraitComponent.Traits.MateRate).Value;
            if (mateRate == 0) _mate = _lifeTime;
            else _mate = _lifeTime / mateRate;

            _currentMate = _mate;
            _currentHunger = _hunger;
            _currentStamina = _stamina;
            _currentLifetime = _lifeTime;
        }

        protected override void DoDraw(IGraphicsAdapter pSystem)
        {
            //TODO make a custom component that handles rendering this?
            var scale = new Vector2(GameObject.Scale.X / 2, GameObject.Scale.Y / 2) * Game.ActiveCamera.Zoom;
            var pos = GameObject.ScreenPosition + new Vector2(scale.X / 2, -scale.Y / 2);
            if(_currentStatus.HasFlag(Status.Scared))
            {
                pSystem.DrawBitmap(_scared, 1, pos, scale);
            }
            else if (_currentStatus.HasFlag(Status.Sleeping))
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

        public override void Update(float pDeltaTime)
        {
            if(_t.Tick())
            {
                //Lifetime
                if ((_currentLifetime -= 1) <= 0)
                {
                    GameObject.Destroy();
                    return;
                }

                //If we are sleeping, just regen stamina, nothing else
                if (HasStatus(Status.Sleeping))
                {
                    _staminaPercent += .1f;
                    if (_staminaPercent >= 1f)
                    {
                        _currentStamina = _stamina + 1; //Add one because we are going to subtract 1
                        RemoveStatus(Status.Sleeping);
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
                    AddStatus(Status.Sleeping);
                    return;
                }

                //*** Hunger
                if (_hungerPercent <= 0)
                {
                    GameObject.Destroy();
                    return;
                }
                else if (_hungerPercent <= .25f)
                {
                    //Start seeking food no matter what
                    OverrideStatus(Status.Hungry);
                }
                else if (_hungerPercent <= .5)
                {
                    //Start seeking food...
                    AddStatus(Status.Hungry);
                }

                //*** Stamina
                if (_staminaPercent <= .3)
                {
                    TrySleep();
                    return;
                }

                //*** Mating
                if (_matePercent <= .4f)
                {
                    AddStatus(Status.Mating);
                }
            }
        }

        public void TrySleep()
        {
            //TODO maybe look around for stuff
            if (HasStatus(Status.Hungry) && _hungerPercent > .4f)
            {
                AddStatus(Status.Sleeping);
            }
        }

        public IEnumerable<Tuple<string, float>> GetStats()
        {
            yield return Tuple.Create("Hunger", _hungerPercent);
            yield return Tuple.Create("Stamina", _staminaPercent);
            yield return Tuple.Create("Mate", _matePercent);
            yield return Tuple.Create("Lifetime", (float)_currentLifetime / _lifeTime);
        }

        public void AddStatus(Status pStatus)
        {
            _currentStatus |= pStatus;
            Visible = (_currentStatus != Status.None);
        }

        public void RemoveStatus(Status pStatus)
        {
            _currentStatus &= ~pStatus;
            Visible = (_currentStatus != Status.None);
        }

        public void OverrideStatus(Status pStatus)
        {
            _currentStatus = pStatus;
            Visible = (_currentStatus != Status.None);
        }

        public bool HasStatus(Status pStatus)
        {
            return _currentStatus.HasFlag(pStatus);
        }    

        public void Eat(Terrain.Vegetation pFood)
        {
            _currentHunger = Math.Min(_hunger, _currentHunger + pFood.Food);
            RemoveStatus(Status.Hungry);
        }

        public void Mate(Organism pMate)
        {
            _currentMate = _mate;
            RemoveStatus(Status.Mating);
        }

        protected override void DoDraw(IGraphicsAdapter pSystem, Effect pEffect)
        {
            throw new NotImplementedException();
        }
    }
}
