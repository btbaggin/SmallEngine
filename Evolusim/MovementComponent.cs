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

        public void Move(float pDeltaTime, TerrainType pTerrain)
        {
            if (!_destinationSet) GetDestination(pTerrain);

            if(_stopped)
            {
                _stopped = (_stopDuration -= pDeltaTime) > 0;
            }

            //TODO use pathing
            MoveTowardsDestination(pDeltaTime);

            if(GameObject.Position ==  _destination) ResolveDestination();
        }

        public void MoveTo(Vector2 pPosition)
        {
            _override = true;
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

        private void GetDestination(TerrainType pTerrain)
        {
            if(!_override)
            {
                switch (_gameObject.GetMovementType())
                {
                    case Organism.Status.None:
                        //TODO only move to preferred terrain
                        RandomDestination();
                        break;

                    case Organism.Status.Hungry:
                        _food = (Vegetation)SceneManager.Current.GameObjects.NearestWithinDistance(GameObject, _vision * 64, "Vegetation");
                        if (_food != null) _destination = _food.Position;
                        else if (!_destinationSet) RandomDestination();
                        break;

                    case Organism.Status.Mating:
                        _mate = (Organism)SceneManager.Current.GameObjects.NearestWithinDistance(GameObject, _vision * 64, "Organism");
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
                switch (_gameObject.GetMovementType())
                {
                    case Organism.Status.Hungry:
                        if (_food == null || _food.IsDead) GetDestination(TerrainType.None);
                        break;

                    case Organism.Status.Mating:
                        if (_mate == null || _mate.MarkedForDestroy) GetDestination(TerrainType.None);
                        else _destination = _mate.Position;
                        break;

                    case Organism.Status.Sleeping:
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
                switch (_gameObject.GetMovementType())
                {
                    case Organism.Status.None:
                        break;

                    case Organism.Status.Hungry:
                        if (_food == null)
                        {
                            _destinationSet = false;
                            return;
                        }

                        _gameObject.Eat(_food);
                        _food = null;
                        break;

                    case Organism.Status.Mating:
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
            var u = RandomGenerator.RandomFloat();
            var v = RandomGenerator.RandomFloat();
            var w = (_vision * 64) * MathF.Sqrt(u);
            var t = 2 * MathF.PI * v;
            var x = w * MathF.Cos(t);
            var y = w * MathF.Sin(t);
            _destination = GameObject.Position + new Vector2(x, y);
        }
    }
}
