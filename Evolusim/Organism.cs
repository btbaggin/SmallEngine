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
            r = new Random();
            SceneManager.Define("organism", typeof(BitmapRenderComponent),
                                            typeof(MovementComponent));
        }

        static Random r;
        public static Organism Create()
        {
            return SceneManager.Current.CreateGameObject<Organism>("organism");
        }

        public Organism()
        {
            Position = new Vector2(r.Next(0, 1000), r.Next(0, 1000));
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
