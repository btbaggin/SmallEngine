using System;
using SmallEngine;
using SmallEngine.Input;
using SmallEngine.Graphics;
using System.Collections.Generic;

namespace Evolusim.Terrain
{
    class Vegetation : GameObject
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public bool IsDead { get { return _lifeTime <= 0; } }

        public int Food { get; private set; }

        private int _lifeTime;
        private float _lifetimeTimer;
        BitmapRenderComponent _render;
        TerrainType _terrainType;
        private int _speadCount;

        static Vegetation()
        {
            SceneManager.Define("vegetation", typeof(BitmapRenderComponent));

            //Add dead bitmaps
            using (Effect e = new Effect())
            {
                e.AddSaturation(.2f);
                e.Create();

                foreach (var s in new string[] { "v_water", "v_desert", "v_grassland", "v_shrubland", "v_temperatedeciduous" })
                {
                    var b = e.ApplyTo(ResourceManager.Request<BitmapResource>(s));
                    ResourceManager.Add(s + "_dead", b, true);
                }
            }
        }

        public static Vegetation Create(int pX, int pY)
        {
            var v = SceneManager.Current.CreateGameObject<Vegetation>("vegetation");
            v.SetXY(pX, pY);
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
                    return .01f;

                case TerrainType.Shrubland:
                    return .02f;

                case TerrainType.Grassland:
                case TerrainType.TemperateDeciduous:
                    return .01f;

                case TerrainType.Desert:
                    return .001f;

                default:
                    return -1f; //Will not create
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            Scale = new Vector2(64);
            _render = GetComponent<BitmapRenderComponent>();
        }

        private void SetXY(int pX, int pY)
        {
            X = pX;
            Y = pY;
            _terrainType = TerrainMap.GetTerrainType(pX, pY);
            Position = TerrainMap.GetPosition(new Vector2(pX, pY));
            switch (_terrainType)
            {
                case TerrainType.Water:
                    _render.SetBitmap("v_water");
                    _lifeTime = RandomGenerator.RandomInt(20, 40);
                    _speadCount = RandomGenerator.RandomInt(1, 2);
                    Food = 10;
                    break;

                case TerrainType.Grassland:
                    _lifeTime = RandomGenerator.RandomInt(20, 40);
                    _speadCount = 1;
                    _render.SetBitmap("v_grassland");
                    Food = 10;
                    break;

                case TerrainType.Shrubland:
                    _lifeTime = RandomGenerator.RandomInt(10, 20);
                    _speadCount = RandomGenerator.RandomInt(0, 3);
                    _render.SetBitmap("v_shrubland");
                    Food = 5;
                    break;

                case TerrainType.TemperateDeciduous:
                    _lifeTime = RandomGenerator.RandomInt(40, 60);
                    _speadCount = RandomGenerator.RandomInt(1, 2);
                    _render.SetBitmap("v_temperatedeciduous");
                    Food = 20;
                    break;

                case TerrainType.Desert:
                    _lifeTime = RandomGenerator.RandomInt(60, 80);
                    _speadCount = 1;
                    _render.SetBitmap("v_desert");
                    Food = 10;
                    break;

                default:
                    throw new Exception("Unsupported terrain type");
            }
        }

        private void Spread()
        {
            for(int i = 0; i < _speadCount; i++)
            {
                var dx = RandomGenerator.RandomInt(X - 1, X + 1);
                var dy = RandomGenerator.RandomInt(Y - 1, Y + 1);
                dx = (int)MathF.Clamp(dx, 0, TerrainMap.Size);
                dy = (int)MathF.Clamp(dy, 0, TerrainMap.Size);

                if (TerrainMap.GetTerrainType(dx, dy) == _terrainType)
                {
                    Create(dx, dy);
                }
            }
        }

        private IEnumerator<WaitEvent> LifeTick()
        {
            while(!MarkedForDestroy)
            {
                if (_lifeTime == 1)
                {
                    Spread();
                }
                else if (_lifeTime == 0)
                {
                    _render.SetBitmap(_render.Bitmap.Alias + "_dead");
                }
                else if (_lifeTime <= -1)
                {
                    Destroy();
                    break;
                }

                yield return new WaitForSeconds(1);
            }
        }
    }
}
