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

        public MovementType Movement { get; set; }

        public Vector2 Speed { get; private set; }

        [ImportComponent]
        private TraitComponent _traits = null;

        int _distance;
        Vector2 _destination;
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
            var nextPos = Vector2.MoveTowards(GameObject.Position, _destination, _traits.GetTrait<int>(TraitComponent.Traits.Speed) * pDeltaTime);
            Speed = GameObject.Position - nextPos;
            GameObject.Position = nextPos;

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
                    if(VegetationMap.Eat(GameObject.Position))
                    {
                        Movement = MovementType.Wander;
                        ((Organism)GameObject).Eat();
                    }
                    break;
            }
            _destinationSet = false;
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
                    _destination = VegetationMap.GetNearestFood(GameObject.Position);
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
