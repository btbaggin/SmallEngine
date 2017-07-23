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
            Defensive
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

        int _distance;
        Vector2 _destination;
        Vegetation _food;
        bool _destinationSet;
        public MovementComponent() : base()
        {
            Movement = MovementType.Wander;
            _distance = 5;
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
                    Movement = MovementType.Wander;
                    ((Organism)GameObject).Eat(_food);
                    break;
            }
            _destinationSet = false;
        }

        private void MoveTowardsDestination(float pDeltaTime)
        {
            switch(Movement)
            {
                case MovementType.Hungry:
                    if(_food.MarkedForDestroy)
                    {
                        GetDestination(Terrain.Type.None);
                    }
                    break;

                default:
                    break;

            }

            var nextPos = Vector2.MoveTowards(GameObject.Position, _destination, _traits.GetTrait<int>(TraitComponent.Traits.Speed) * pDeltaTime);
            Speed = GameObject.Position - nextPos;
            GameObject.Position = nextPos;
        }

        private void GetDestination(Terrain.Type pTerrain)
        {
            switch(Movement)
            {
                case MovementType.Wander:
                    //TODO only move to preferred terrain
                    var t = Terrain.GetTile(GameObject.Position);
                    var p = new Vector2(t.X + Game.RandomInt(-_distance, _distance), t.Y + Game.RandomInt(-_distance, _distance));
                    _destination = Terrain.GetPosition(p);
                    break;
                case MovementType.Hungry:
                    _food = Vegetation.FindNearestVegetation(GameObject.Position);
                    _destination = Terrain.GetPosition(new Vector2(_food.X, _food.Y));
                    break;
                case MovementType.Defensive:
                    break;
                case MovementType.Aggressive:
                    break;
            }
            _destinationSet = true;
            _destination = Vector2.Clamp(_destination, Vector2.Zero, new Vector2(Evolusim.WorldSize, Evolusim.WorldSize));
        }
    }
}
