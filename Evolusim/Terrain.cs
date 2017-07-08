using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SmallEngine;
using SmallEngine.Graphics;

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
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            float xStep = Game.Form.Width / _width;
            float yStep = Game.Form.Height / _height;
            for(int x = 0; x < _width; x++)
            {
                for(int y = 0; y < _height; y++)
                {
                    switch(_terrain[x,y])
                    {
                        case Type.Desert:
                            pSystem.DrawBitmap(_desert, 1, new Vector2(xStep * x, yStep * y), new Vector2(xStep, yStep));
                            break;

                        case Type.Forest:
                            pSystem.DrawBitmap(_forest, 1, new Vector2(xStep * x, yStep * y), new Vector2(xStep, yStep));
                            break;

                        case Type.Mountain:
                            pSystem.DrawBitmap(_mountain, 1, new Vector2(xStep * x, yStep * y), new Vector2(xStep, yStep));
                            break;

                        case Type.Plains:
                            pSystem.DrawBitmap(_plains, 1, new Vector2(xStep * x, yStep * y), new Vector2(xStep, yStep));
                            break;

                        case Type.Water:
                            pSystem.DrawBitmap(_water, 1, new Vector2(xStep * x, yStep * y), new Vector2(xStep, yStep));
                            break;
                    }
                }
            }
        }
    }
}
