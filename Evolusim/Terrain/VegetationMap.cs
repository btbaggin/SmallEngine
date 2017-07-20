using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class VegetationMap
    {
        public enum VegetationType
        {
            None,
            Berry,
            Wheat,
            Cactus,
            Thing,
            Lily
        }

        Terrain.Type[,] _terrain;
        VegetationType[,] _vegetation;
        BitmapResource _plant;

        public VegetationMap(Terrain.Type[,] pTerrain)
        {
            _terrain = pTerrain;
            _vegetation = new VegetationType[Terrain.Size, Terrain.Size];
            _plant = ResourceManager.Request<BitmapResource>("plant");
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
                            pSystem.DrawBitmap(_plant, 1, Terrain.GetPosition(new Vector2(x, y)), new Vector2(10));
                            break;
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
            switch(_terrain[pX,pY])
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
