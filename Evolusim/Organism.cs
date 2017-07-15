using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace Evolusim
{
    class Organism : GameObject
    {
        private BitmapRenderComponent _render;
        private MovementComponent _movement;
        static Organism()
        {
            SceneManager.Define("organism", typeof(BitmapRenderComponent),
                                            typeof(MovementComponent));
        }

        public static Organism Create()
        {
            return SceneManager.Current.CreateGameObject<Organism>("organism");
        }

        public Organism()
        {
            Position = new Vector2(Game.RandomInt(0, Evolusim.WorldSize), Game.RandomInt(0, Evolusim.WorldSize));
            Scale = new Vector2(30);
        }

        public override void Initialize()
        {
            _render = (BitmapRenderComponent)GetComponent(typeof(BitmapRenderComponent));
            _render.SetBitmap("organism");

            _movement = (MovementComponent)GetComponent(typeof(MovementComponent));
        }

        public override void Update(float pDeltaTime)
        {
            _movement.Move(pDeltaTime);
        }
    }
}
