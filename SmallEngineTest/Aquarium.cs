using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Input;

namespace SmallEngineTest
{
    class Aquarium : GameObject
    {
        List<Fish> _fishes;
        public List<Food> Food { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            Scale = new Vector2(640, 480);
            Food = new List<Food>();
            _fishes = new List<Fish>();
        }

        public void AddFish(Fish pFish)
        {
            pFish._aquarium = this;
            _fishes.Add(pFish);
        }

        public void AddFood(Food pFood)
        {
            Food.Add(pFood);
        }
    }
}
