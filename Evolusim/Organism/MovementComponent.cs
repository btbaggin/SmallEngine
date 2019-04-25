using System.Linq;
using SmallEngine;

using Evolusim.Terrain;

namespace Evolusim
{
    class MovementComponent : DependencyComponent
    {
        public Vector2 Speed { get; private set; }

        [ImportComponent]
        private TraitComponent _traits = null;

        [ImportComponent]
        private StatusComponent _status = null;

        int _speed;
        int _vision;
        public Vector2 _destination;
        Vegetation _food;
        Organism _mate;
        bool _destinationSet;
        bool _stopped;
        float _stopDuration;
        bool _override;

        Organism _gameObject;

        public MovementComponent() : base()
        {
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);

            _vision = (int)_traits.GetTrait(TraitComponent.Traits.Vision).Value;
            _speed = (int)_traits.GetTrait(TraitComponent.Traits.Speed).Value;
            _gameObject = (Organism)GameObject;
        }

        public override void Update(float pDeltaTime)
        {
            if (!_destinationSet) GetDestination();

            if(_stopped)
            {
                float v = _stopDuration -= pDeltaTime;
                _stopped = v > 0;
            }

            //TODO use pathing
            MoveTowardsDestination(pDeltaTime);

            if(GameObject.Position ==  _destination) ResolveDestination();
        }

        public void MoveTo(Vector2 pPosition, bool pOverride)
        {
            if (_status.HasStatus(StatusComponent.Status.Sleeping))
            {
                if (pOverride) _status.RemoveStatus(StatusComponent.Status.Sleeping);
                else return;
            }

            _override = pOverride;
            _destination = pPosition;
            _destinationSet = true;
            _mate = null;
            _food = null;
        }

        public void Stop(float pDuration)
        {
            _stopDuration = pDuration;
            _stopped = true;
        }

        private void GetDestination()
        {
            if(!_override)
            {
                switch (GetMovementType())
                {
                    case StatusComponent.Status.None:
                        //TODO only move to preferred terrain
                        RandomDestination();
                        break;

                    case StatusComponent.Status.Scared:
                        //Shouldn't need to do anything
                        break;

                    case StatusComponent.Status.Hungry:
                        _food = (Vegetation)Scene.Current.GameObjects.NearestWithinDistance(GameObject, _vision * 64, "Vegetation");
                        if (_food != null) _destination = _food.Position;
                        else if (!_destinationSet) RandomDestination();
                        break;

                    case StatusComponent.Status.Mating:
                        _mate = (Organism)Scene.Current.GameObjects.NearestWithinDistance(GameObject, _vision * 64, "Organism");
                        if (_mate != null) _destination = _mate.Position;
                        else if (!_destinationSet) RandomDestination();
                        break;
                }
                _destinationSet = true;
                _destination = Vector2.Clamp(_destination, Vector2.Zero, new Vector2(Evolusim.WorldSize, Evolusim.WorldSize));
            }
        }

        private void MoveTowardsDestination(float pDeltaTime)
        {
            if (_stopped) return;

            if(!_override)
            {
                switch (GetMovementType())
                {
                    case StatusComponent.Status.Hungry:
                        if (_food == null || _food.IsDead) GetDestination();
                        break;

                    case StatusComponent.Status.Mating:
                        if (_mate == null || _mate.MarkedForDestroy) GetDestination();
                        else _destination = _mate.Position;
                        break;

                    case StatusComponent.Status.Sleeping:
                        return;

                    default:
                        break;
                }
            }

            var nextPos = Vector2.MoveTowards(GameObject.Position, _destination, _speed * pDeltaTime);
            Speed = GameObject.Position - nextPos;
            GameObject.Position = nextPos;
        }

        private void ResolveDestination()
        {
            if(!_override)
            {
                switch (GetMovementType())
                {
                    case StatusComponent.Status.None:
                    case StatusComponent.Status.Scared:
                        break;

                    case StatusComponent.Status.Hungry:
                        if (_food == null)
                        {
                            _destinationSet = false;
                            return;
                        }

                        _gameObject.Eat(_food);
                        _food = null;
                        break;

                    case StatusComponent.Status.Mating:
                        if (_mate == null)
                        {
                            _destinationSet = false;
                            return;
                        }

                        _gameObject.Mate(_mate);
                        _mate = null;
                        break;
                }
            }

            _destinationSet = false;
            _override = false;
        }

        private void RandomDestination()
        {
            var u = Generator.Random.NextFloat();
            var v = Generator.Random.NextFloat();
            var w = (_vision * 64) * MathF.Sqrt(u);
            var t = 2 * MathF.PI * v;
            var x = w * MathF.Cos(t);
            var y = w * MathF.Sin(t);
            _destination = GameObject.Position + new Vector2(x, y);
        }

        private StatusComponent.Status GetMovementType()
        {
            if (_status.HasStatus(StatusComponent.Status.Scared)) return StatusComponent.Status.Scared;
            if (_status.HasStatus(StatusComponent.Status.Sleeping)) return StatusComponent.Status.Sleeping;
            if (_status.HasStatus(StatusComponent.Status.Hungry)) return StatusComponent.Status.Hungry;
            if (_status.HasStatus(StatusComponent.Status.Mating)) return StatusComponent.Status.Mating;
            return StatusComponent.Status.None;
        }

    }
}
