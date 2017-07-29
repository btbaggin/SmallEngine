using System.Linq;
using SmallEngine;

namespace Evolusim
{
    class MovementComponent : DependencyComponent
    {
        public enum MovementType
        {
            Wander,
            Hungry,
            Aggressive,
            Defensive,
            Mate
        }

        private MovementType _movement;
        public MovementType Movement
        {
            get { return _movement; }
            set { _movement = value; _destinationSet = false; }
        }

        public Vector2 Speed { get; private set; }

        [ImportComponent]
        private TraitComponent _traits = null;

        int _speed;
        int _vision;
        public Vector2 _destination;
        Vegetation _food;
        Organism _mate;
        bool _destinationSet;
        public MovementComponent() : base()
        {
            Movement = MovementType.Wander;
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);

            _vision = _traits.GetTrait<int>(TraitComponent.Traits.Vision);
            _speed = _traits.GetTrait<int>(TraitComponent.Traits.Speed);
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

        private void ResolveDestination()
        {
            switch(Movement)
            {
                case MovementType.Wander:
                    break;

                case MovementType.Hungry:
                    if (_food == null) return;

                    ((Organism)GameObject).Eat(_food);
                    _food = null;
                    break;

                case MovementType.Mate:
                    if (_mate == null) return;

                    ((Organism)GameObject).Mate(_mate);
                    _mate = null;
                    break;
            }
            _destinationSet = false;
            Movement = MovementType.Wander;
        }

        private void MoveTowardsDestination(float pDeltaTime)
        {
            switch(Movement)
            {
                case MovementType.Hungry:
                    if(_food == null || _food.MarkedForDestroy) GetDestination(Terrain.Type.None);
                    break;

                case MovementType.Mate:
                    if (_mate == null || _mate.MarkedForDestroy) GetDestination(Terrain.Type.None);
                    break;

                default:
                    break;

            }

            var nextPos = Vector2.MoveTowards(GameObject.Position, _destination, _speed * pDeltaTime);
            Speed = GameObject.Position - nextPos;
            GameObject.Position = nextPos;
        }

        private void GetDestination(Terrain.Type pTerrain)
        {
            switch(Movement)
            {
                case MovementType.Wander:
                    //TODO only move to preferred terrain
                    RandomDestination();
                    break;
                case MovementType.Hungry:
                    foreach(var go in SceneManager.Current.GameObjects.Where(pGo => pGo.Tag == "Vegetation"))
                    {
                        var v = go as Vegetation;
                        if (Vector2.Distance(v.Position, GameObject.Position) < _vision * 64 && v != GameObject)
                        {
                            _food = v;
                            break;
                        }
                    }
                    if (_food != null) _destination = _food.Position;
                    else if(!_destinationSet) RandomDestination();
                    break;
                case MovementType.Defensive:
                    break;
                case MovementType.Aggressive:
                    break;
                case MovementType.Mate:
                    foreach(var go in SceneManager.Current.GameObjects.Where(pGo => pGo.Tag == "Organism"))
                    {
                        var o = go as Organism;
                        if(Vector2.Distance(o.Position, GameObject.Position) < _vision * 64 && o != GameObject)
                        {
                            _mate = o;
                            break;
                        }
                    }
                    if (_mate != null) _destination = _mate.Position;
                    else if(!_destinationSet) RandomDestination();
                    break;
            }
            _destinationSet = true;
            _destination = Vector2.Clamp(_destination, Vector2.Zero, new Vector2(Evolusim.WorldSize, Evolusim.WorldSize));
        }

        private void RandomDestination()
        {
            var u = Game.RandomFloat();
            var v = Game.RandomFloat();
            var w = (_vision * 64) * MathF.Sqrt(u);
            var t = 2 * MathF.PI * v;
            var x = w * MathF.Cos(t);
            var y = w * MathF.Sin(t);
            _destination = GameObject.Position + new Vector2(x, y);
        }
    }
}
