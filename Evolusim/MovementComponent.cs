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
        //TODO movement type

        public MovementComponent()
        {
        }

        public void Move(float pDeltaTime)
        {
            GameObject.Position += Vector2.Unit * 10 * pDeltaTime;
        }
    }
}
