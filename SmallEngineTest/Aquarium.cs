using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace SmallEngineTest
{
    class Aquarium : GameObject
    {
        public Aquarium()
        {
            AddComponent(new BitmapRenderComponent("aquarium_background") { Order = 0 });
            Scale = new Vector2(640, 480);
        }
    }
}
