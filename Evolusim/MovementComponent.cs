using System.Linq;
using SmallEngine;

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

        Organism _gameObject;

        public MovementComponent() : base()
        {
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);

            _vision = _traits.GetTrait<int>(TraitComponent.Traits.Vision);
            _speed = _traits.GetTrait<int>(TraitComponent.Traits.Speed);
            _gameObject = (Organism)GameObject;
        }

        public void Move(float pDeltaTime, Terrain.Type pTerrain)
        {
            if (!_destinationSet)
            {
                GetDestination(pTerrain);
            }

            //TODO use pathing
            MoveTowardsDestination(pDeltaTime);

            if(GameObject.Position ==  _destination)
            {
                ResolveDestination();
            }
        }

        private void GetDestination(Terrain.Type pTerrain)
        {
            switch (_gameObject.OrganismStatus)
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

        private void MoveTowardsDestination(float pDeltaTime)
        {
            switch(_gameObject.OrganismStatus)
            {
                case Organism.Status.Hungry:
                    if(_food == null || _food.MarkedForDestroy) GetDestination(Terrain.Type.None);
                    break;

                case Organism.Status.Mating:
                    if (_mate == null || _mate.MarkedForDestroy) GetDestination(Terrain.Type.None);
                    else _destination = _mate.Position;
                    break;

                default:
                    break;
            }

            var nextPos = Vector2.MoveTowards(GameObject.Position, _destination, _speed * pDeltaTime);
            Speed = GameObject.Position - nextPos;
            GameObject.Position = nextPos;
        }

        private void ResolveDestination()
        {
            switch (_gameObject.OrganismStatus)
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
            _destinationSet = false;
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
