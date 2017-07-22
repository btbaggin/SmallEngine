using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class VegetationMap : IDrawable, IUpdatable
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

        static VegetationType[,] _vegetation;

        const int SpreadSize = 2;
        const int GrowthTime = 10;
        const int DeadClearTime = 1;
        float _growthTimer;
        float _deadClearTimer;
        bool _clearDead;
        BitmapResource _plant;
        BitmapResource _plantDead;

        public VegetationMap()
        {
            _vegetation = new VegetationType[Terrain.Size, Terrain.Size];
            _plant = ResourceManager.Request<BitmapResource>("plant");
            _plantDead = ResourceManager.Request<BitmapResource>("plant_dead");
            GenerateVegetation();
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            float numTilesX = Evolusim.ActiveCamera.Width / Terrain.BitmapSize;

            //Width and height should be the same
            var tileSize = (int)(Game.Form.Width / numTilesX);

            var scale = new Vector2(tileSize);
            for (int x = 0; x < Terrain.Size; x++)
            {
                for(int y = 0; y < Terrain.Size; y++)
                {
                    switch(_vegetation[x, y])
                    {
                        case VegetationType.Berry:
                            pSystem.DrawBitmap(_plant, 1, Game.ActiveCamera.ToCameraSpace(Terrain.GetPosition(new Vector2(x, y))), scale);
                            break;

                        case VegetationType.Cactus:
                            pSystem.DrawBitmap(_plant, 1, Game.ActiveCamera.ToCameraSpace(Terrain.GetPosition(new Vector2(x, y))), scale);
                            break;

                        case VegetationType.Lily:
                            pSystem.DrawBitmap(_plant, 1, Game.ActiveCamera.ToCameraSpace(Terrain.GetPosition(new Vector2(x, y))), scale);
                            break;

                        case VegetationType.Thing:
                            pSystem.DrawBitmap(_plant, 1, Game.ActiveCamera.ToCameraSpace(Terrain.GetPosition(new Vector2(x, y))), scale);
                            break;

                        case VegetationType.Wheat:
                            pSystem.DrawBitmap(_plant, 1, Game.ActiveCamera.ToCameraSpace(Terrain.GetPosition(new Vector2(x, y))), scale);
                            break;

                        case VegetationType.Dead:
                            pSystem.DrawBitmap(_plantDead, 1, Game.ActiveCamera.ToCameraSpace(Terrain.GetPosition(new Vector2(x, y))), scale);
                            break;
                    }
                }
            }
        }

        public void Update(float pDeltaTime)
        {
            if ((_growthTimer += pDeltaTime) >= GrowthTime)
            {
                _growthTimer = 0;
                Spread();
                _clearDead = true;
            }
            else if (_clearDead && (_deadClearTimer += pDeltaTime) >= DeadClearTime)
            {
                for(int x = 0; x < Terrain.Size; x++)
                {
                    for(int y = 0; y < Terrain.Size; y++)
                    {
                        if(_vegetation[x, y] == VegetationType.Dead)
                        {
                            _vegetation[x, y] = VegetationType.None;
                        }
                    }
                }
                _deadClearTimer = 0;
                _clearDead = false;
            }
        }

        public static Vector2 GetNearestFood(Vector2 pPosition)
        {
            var xy = Terrain.GetTile(pPosition);
            for(int x = (int)xy.X; x < Terrain.Size; x++)
            {
                for (int y = (int)xy.Y; y < Terrain.Size; y++)
                {
                    if(_vegetation[x, y] != VegetationType.None &&
                        _vegetation[x, y] != VegetationType.Dead)
                    {
                        return Terrain.GetPosition(new Vector2(x, y));
                    }
                }
            }
            return pPosition;
        }

        public static bool Eat(Vector2 pPosition)
        {
            var xy = Terrain.GetTile(pPosition);
            int x = (int)xy.X;
            int y = (int)xy.Y;
            var success = x < Terrain.Size &&
                          y < Terrain.Size &&
                          _vegetation[x, y] != VegetationType.None &&
                          _vegetation[x, y] != VegetationType.Dead;
            if(success) _vegetation[x, y] = VegetationType.None;
            return success;
        }

        private void Spread()
        {
            for(int x = 0; x < Terrain.Size; x++)
            {
                for(int y = 0; y < Terrain.Size; y++)
                {
                    if (_vegetation[x, y] == VegetationType.Dead)
                    {
                        _vegetation[x, y] = VegetationType.None;
                    }
                    else if (_vegetation[x, y] != VegetationType.None)
                    {
                        //TODO should they all spread?
                        var dx = Game.RandomInt(Math.Max(0, x - SpreadSize), Math.Min(Terrain.Size, x + SpreadSize));
                        var dy = Game.RandomInt(Math.Max(0, y - SpreadSize), Math.Min(Terrain.Size, y + SpreadSize));

                        _vegetation[dx, dy] = _vegetation[x, y];
                        _vegetation[x, y] = VegetationType.Dead;
                    }
                }
            }
        }

        private void GenerateVegetation()
        {
            for(int x = 0; x < Terrain.Size; x++)
            {
                for(int y = 0; y < Terrain.Size; y++)
                {
                    if(Game.RandomFloat() < .01f)
                    {
                        _vegetation[x, y] = GetVegetationType(x, y);
                    }
                }
            }
        }

        private VegetationType GetVegetationType(int pX, int pY)
        {
            switch(Terrain.GetType(pX,pY))
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
    }
}
