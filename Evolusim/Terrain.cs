using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace Evolusim
{
    class Terrain
    {
        public enum Type
        {
            Mountain,
            Water,
            Plains,
            Forest,
            Desert
        }

        int _width;
        int _height;

        Type[,] _terrain;

        BitmapResource _plains;
        BitmapResource _water;
        BitmapResource _mountain;
        BitmapResource _forest;
        BitmapResource _desert;
        BitmapResource _terrainBitmap;

        public Terrain(int pWidth, int pHeight)
        {
            _width = pWidth;
            _height = pHeight;
            _terrain = new Type[_width, _height];

            _plains = ResourceManager.Request<BitmapResource>("plains");
            _water = ResourceManager.Request<BitmapResource>("water");
            _mountain = ResourceManager.Request<BitmapResource>("mountain");
            _forest = ResourceManager.Request<BitmapResource>("forest");
            _desert = ResourceManager.Request<BitmapResource>("desert");

            RenderTerrain();
        }

        private void RenderTerrain()
        {
            BitmapResource[,] bitmaps = new BitmapResource[_width, _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    switch (_terrain[x, y])
                    {
                        case Type.Desert:
                            bitmaps[x,y] = _desert;
                            break;

                        case Type.Forest:
                            bitmaps[x,y] = _forest;
                            break;

                        case Type.Mountain:
                            bitmaps[x,y] = _mountain;
                            break;

                        case Type.Plains:
                            bitmaps[x,y] = _plains;
                            break;

                        case Type.Water:
                            bitmaps[x,y] = _water;
                            break;
                    }
                }
            }

            _terrainBitmap = ((DirectXGraphicSystem)Game.Graphics).CreateTile(bitmaps, _width, _height, 64);
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawBitmap(_terrainBitmap, 1, Vector2.Zero, new Vector2(Game.Form.Height, Game.Form.Height), Evolusim.MainCamera.Viewport);
        }

        public void SetTypeAtMouse(Type pType)
        {
            var p = InputManager.MousePosition;
            if(p.X > 0 && p.X < Game.Form.Height && p.Y > 0 && p.Y < Game.Form.Height)
            {
                var step = Game.Form.Height / (float)_height;
                int x = (int)Math.Floor(p.X / step);
                int y = (int)Math.Floor(p.Y / step);
                if(_terrain[x,y] != pType)
                {
                    _terrain[x, y] = pType;
                    RenderTerrain();
                }
            }
        }

        public Type GetType(int pX, int pY)
        {
            return _terrain[pX, pY];
        }
    }
}
