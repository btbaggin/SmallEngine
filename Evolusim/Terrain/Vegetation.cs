using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class Vegetation : GameObject
    {
        public enum VegetationType
        {
            None,
            Berry,
            Wheat,
            Cactus,
            Thing,
            Lily,
            Dead
        }

        private const int SpreadSize = 2;

        public VegetationType Type { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public bool IsDead { get { return _lifeTime <= 0; } }
        private int _lifeTime;
        private float _lifetimeTimer;

        BitmapRenderComponent _render;

        static Vegetation()
        {
            SceneManager.Define("vegetation", typeof(BitmapRenderComponent));
        }

        public static Vegetation Create(int pX, int pY)
        {
            var v = SceneManager.Current.CreateGameObject<Vegetation>("vegetation");
            v.X = pX; v.Y = pY;
            v.Position = Terrain.GetPosition(new Vector2(pX, pY));
            v.Tag = "Vegetation";
            return v;
        }

        public override void Initialize()
        {
            base.Initialize();
            Type = GetVegetationType();
            _lifeTime = Game.RandomInt(20, 40);
            Scale = new Vector2(64);

            _render = GetComponent<BitmapRenderComponent>();
            switch(Type)
            {
                case VegetationType.Cactus:
                    _render.SetBitmap("cactus");
                    break;

                default:
                    _render.SetBitmapFromGroup("plants");
                    break;
            }
        }

        private VegetationType GetVegetationType()
        {
            switch (Terrain.GetType((int)Position.X, (int)Position.Y))
            {
                case Terrain.Type.Forest:
                    return VegetationType.Berry;

                case Terrain.Type.Plains:
                    return VegetationType.Wheat;

                case Terrain.Type.Mountain:
                    return VegetationType.Thing;

                case Terrain.Type.Desert:
                    return VegetationType.Cactus;

                case Terrain.Type.Water:
                    return VegetationType.Lily;
            }
            return VegetationType.None;
        }

        private void Spread()
        {
            var dx = Game.RandomInt(X - SpreadSize, X + SpreadSize);
            var dy = Game.RandomInt(Y - SpreadSize, Y + SpreadSize);
            dx = (int)MathF.Clamp(dx, 0, Terrain.Size);
            dy = (int)MathF.Clamp(dy, 0, Terrain.Size);

            if (dx < Terrain.Size && dy < Terrain.Size) Vegetation.Create(dx, dy);
        }

        public override void Update(float pDeltaTime)
        {
            if ((_lifetimeTimer += pDeltaTime) >= 1)
            {
                _lifeTime -= 1;
                _lifetimeTimer = 0;
            }
            else return;

            if(_lifeTime == 1)
            {
                Spread();
            }
            else if(_lifeTime == 0)
            {
                _render.SetBitmap("plant_dead");
            }
            else if(_lifeTime <= -1)
            {
                Destroy();
            }
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            _render.Draw(pSystem);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
