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
        Game _game;
        public Aquarium(Game pGame)
        {
            AddComponent(new BitmapRenderComponent("aquarium_background") { Order = 0 });
            Scale = new Vector2(640, 480);
            _fishes = new List<Fish>();
            Food = new List<Food>();
            _game = pGame;
        }

        public void AddFish(Fish pFish)
        {
            _fishes.Add(pFish);
        }

        public void AddFood(Food pFood)
        {
            Food.Add(pFood);
        }
    }
}
