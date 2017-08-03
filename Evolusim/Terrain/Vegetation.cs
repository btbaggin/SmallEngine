using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim.Terrain
{
    class Vegetation : GameObject
    {
        private const int SpreadSize = 2;

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
            v.SetXY(pX, pY);
            v.Position = TerrainMap.GetPosition(new Vector2(pX, pY));
            v.Tag = "Vegetation";
            return v;
        }

        public static void Populate()
        {
            for (int x = 0; x < TerrainMap.Size; x++)
            {
                for (int y = 0; y < TerrainMap.Size; y++)
                {
                    if (RandomGenerator.RandomFloat() < GetPercent(TerrainMap.GetTerrainType(x, y)))
                    {
                        Create(x, y);
                    }
                }
            }
        }

        private static float GetPercent(TerrainType pType)
        {
            switch (pType)
            {
                case TerrainType.Water:
                    return .05f;

                case TerrainType.Grassland:
                case TerrainType.Shrubland:
                case TerrainType.TemperateDeciduous:
                    return .01f;

                case TerrainType.Desert:
                    return .003f;

                default:
                    return -1f; //Will not create
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _lifeTime = RandomGenerator.RandomInt(20, 40);
            Scale = new Vector2(64);
            _render = GetComponent<BitmapRenderComponent>();
        }

        private void SetXY(int pX, int pY)
        {
            switch (TerrainMap.GetTerrainType(pX, pY))
            {
                case TerrainType.Water:
                    _render.SetBitmap("v_water");
                    break;

                case TerrainType.Grassland:
                    _render.SetBitmap("v_grassland");
                    break;

                case TerrainType.Shrubland:
                    _render.SetBitmap("v_shrubland");
                    break;

                case TerrainType.TemperateDeciduous:
                    _render.SetBitmap("v_temperatedeciduous");
                    break;

                case TerrainType.Desert:
                    _render.SetBitmap("v_desert");
                    break;

                default:
                    throw new Exception("Unsupported terrain type");
            }
        }

        private void Spread()
        {
            var dx = RandomGenerator.RandomInt(X - SpreadSize, X + SpreadSize);
            var dy = RandomGenerator.RandomInt(Y - SpreadSize, Y + SpreadSize);
            dx = (int)MathF.Clamp(dx, 0, TerrainMap.Size);
            dy = (int)MathF.Clamp(dy, 0, TerrainMap.Size);

            if (dx < TerrainMap.Size && dy < TerrainMap.Size) Vegetation.Create(dx, dy); //TODO this breaks
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
            else if(_lifeTime <= -1)
            {
                Destroy();
            }
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            if(IsDead)
            {
                ((DirectXGraphicSystem)Game.Graphics).SetEffect(_render.Bitmap, ScreenPosition);
            }
            else
            {
                _render.Draw(pSystem);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
