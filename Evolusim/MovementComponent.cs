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
            GameObject.Position = Vector2.MoveTowards(GameObject.Position, _destination, _traits.GetTrait<int>(TraitComponent.Traits.Speed) * pDeltaTime);

            if(Vector2.DistanceSqrd(GameObject.Position, _destination) < 25)
            {
                _destinationSet = false;
            }
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
                    break;
                case MovementType.Defensive:
                    break;
                case MovementType.Aggressive:
                    break;
            }
            _destinationSet = true;
        }
    }
}
