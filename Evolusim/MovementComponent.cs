using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace Evolusim
{
    class MovementComponent : Component
    {
        public enum MovementType
        {
            Wander,
            Hungry,
            Aggressive,
            Defensive
        }

        public MovementType Movement { get; set; }

        Random r;
        Vector2 _destination;
        bool _destinationSet;
        public MovementComponent()
        {
            Movement = MovementType.Wander;
            r = new Random();
        }

        public void Move(float pDeltaTime)
        {
            if (!_destinationSet)
            {
                GetDestination();
            }

            //TODO use pathing
            GameObject.Position += Vector2.Unit * 100 * pDeltaTime;//Vector2.MoveTowards(GameObject.Position, _destination, .05f) * pDeltaTime;//100 * pDeltaTime);

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
                    _destination = new Vector2(r.Next(0, Evolusim.WorldSize), r.Next(0, Evolusim.WorldSize));
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
