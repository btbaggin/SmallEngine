using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        Vector2 _destination;
        bool _destinationSet;
        public MovementComponent() : base()
        {
            Movement = MovementType.Wander;
        }

        public void Move(float pDeltaTime)
        {
            if (!_destinationSet)
            {
                GetDestination();
            }

            //TODO use pathing
            GameObject.Position = Vector2.MoveTowards(GameObject.Position, _destination, _traits.GetTrait<int>(TraitComponent.Traits.Speed) * pDeltaTime);

            if(Vector2.DistanceSqrd(GameObject.Position, _destination) < 25)
            {
                _destinationSet = false;
            }
        }

        private void GetDestination()
        {
            switch(Movement)
            {
                case MovementType.Wander:
                    _destination = new Vector2(Game.RandomInt(0, Evolusim.WorldSize), Game.RandomInt(0, Evolusim.WorldSize));
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
