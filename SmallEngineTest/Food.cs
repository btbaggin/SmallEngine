using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace SmallEngineTest
{
    class Food : GameObject
    {
        public Food()
        {
            AddComponent(new BitmapRenderComponent("food"));
            Position = SmallEngine.Input.InputManager.MousePosition;
            Scale = new Vector2(10, 10);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            Position += Vector2.UnitY * pDeltaTime * 20;

            if(Position.Y > 480)
            {
                Dispose(); //TODO dispose
            }
        }
    }
}
