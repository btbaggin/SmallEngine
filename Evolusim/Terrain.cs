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

        float _bitmapWidth;
        float _bitmapHeight;

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
            _bitmapWidth = _width * 64;
            _bitmapHeight = _height * 64;
            _terrain = new Type[_width, _height];

            _plains = ResourceManager.Request<BitmapResource>("plains");
            _water = ResourceManager.Request<BitmapResource>("water");
            _mountain = ResourceManager.Request<BitmapResource>("mountain");
            _forest = ResourceManager.Request<BitmapResource>("forest");
            _desert = ResourceManager.Request<BitmapResource>("desert");

            var n = new DiamondSquareNoise(_width);
            var h = n.Generate(0, 20, .4f);

            float minValue = 0;
            float maxValue = 0;
            for(int x = 0; x < _width; x++)
            {
                for(int y = 0; y < _width; y++)
                {
                    if (h[x, y] > maxValue) maxValue = h[x, y];
                    if (h[x, y] < minValue) minValue = h[x, y];
                }
            }

            for(int x = 0; x < _width; x++)
            {
                for(int y = 0; y < _height; y++)
                {
                    var hh = (h[x, y] + Math.Abs(minValue)) / maxValue;
                    if(hh < .45)
                    {
                        _terrain[x, y] = Type.Water;
                    }
                    else if(hh < .5)
                    {
                        _terrain[x, y] = Type.Plains;
                    }
                    else if(hh < .7)
                    {
                        _terrain[x, y] = Type.Forest;
                    }
                    else if(hh < .9)
                    {
                        _terrain[x, y] = Type.Desert;
                    }
                    else
                    {
                        _terrain[x, y] = Type.Mountain;
                    }
                }
            }
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            int x = (int)(Evolusim.ActiveCamera.Position.X / 64);
            int y = (int)(Evolusim.ActiveCamera.Position.Y / 64);
            float numTilesX = Evolusim.ActiveCamera.Width / 64f;
            float numTilesY = Evolusim.ActiveCamera.Height / 64f;

            //Width and height should be the same
            float tileSize = Game.Form.Width / numTilesX;
            var startPoint = Evolusim.ActiveCamera.ToCameraSpace(new Vector2(x * 64, y * 64));

            Vector2 scale = new Vector2(tileSize, tileSize);
            var currentX = startPoint.X;
            var currentY = startPoint.Y;
            for(int i = x; i <= x + numTilesX + 2; i++)
            {
                if (i >= _width) break;
                for(int j = y; j <= y + numTilesY + 2; j++)
                {
                    if (j >= _height) break;
                    switch(_terrain[i, j])
                    {
                        case Type.Desert:
                            pSystem.DrawBitmap(_desert, 1, new Vector2(currentX, currentY), scale);
                            break;
                        case Type.Forest:
                            pSystem.DrawBitmap(_forest, 1, new Vector2(currentX, currentY), scale);
                            break;
                        case Type.Mountain:
                            pSystem.DrawBitmap(_mountain, 1, new Vector2(currentX, currentY), scale);
                            break;
                        case Type.Plains:
                            pSystem.DrawBitmap(_plains, 1, new Vector2(currentX, currentY), scale);
                            break;
                        case Type.Water:
                            pSystem.DrawBitmap(_water, 1, new Vector2(currentX, currentY), scale);
                            break;
                    }
                    currentY += tileSize;
                }
                currentY = startPoint.Y;
                currentX += tileSize;
            }
        }

        public void SetTypeAt(Type pType, Vector2 pPoint)
        {
            int x = (int)Math.Floor(pPoint.X / 64);
            int y = (int)Math.Floor(pPoint.Y / 64);
            if(x >= 0 && y >= 0 && x < _terrain.GetUpperBound(0) && y < _terrain.GetUpperBound(1))
            {
                if (_terrain[x, y] != pType)
                {
                    _terrain[x, y] = pType;
                }
            }
        }

        public Type GetType(int pX, int pY)
        {
            return _terrain[pX, pY];
        }
    }
}
