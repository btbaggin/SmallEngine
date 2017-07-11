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
        BitmapResource[,] _bitmaps;

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
            _bitmapWidth = _width * 64;
            _bitmapHeight = _height * 64;
            _terrain = new Type[_width, _height];
            _bitmaps = new BitmapResource[_width, _height];

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

            InitializeBitmaps();
            GenerateBitmap();
        }

        private void InitializeBitmaps()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    switch (_terrain[x, y])
                    {
                        case Type.Desert:
                            _bitmaps[x,y] = _desert;
                            break;

                        case Type.Forest:
                            _bitmaps[x,y] = _forest;
                            break;

                        case Type.Mountain:
                            _bitmaps[x,y] = _mountain;
                            break;

                        case Type.Plains:
                            _bitmaps[x,y] = _plains;
                            break;

                        case Type.Water:
                            _bitmaps[x,y] = _water;
                            break;
                    }
                }
            }
        }

        private void GenerateBitmap()
        {
            //TODO this leaks
            _terrainBitmap = Game.Graphics.CreateTile(_bitmaps, _width, _height, 64);
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawBitmap(_terrainBitmap, 1, Vector2.Zero, new Vector2(Game.Form.Width, Game.Form.Height), Evolusim.MainCamera.Viewport);
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
                    switch (pType)
                    {
                        case Type.Desert:
                            _bitmaps[x, y] = _desert;
                            break;

                        case Type.Forest:
                            _bitmaps[x, y] = _forest;
                            break;

                        case Type.Mountain:
                            _bitmaps[x, y] = _mountain;
                            break;

                        case Type.Plains:
                            _bitmaps[x, y] = _plains;
                            break;

                        case Type.Water:
                            _bitmaps[x, y] = _water;
                            break;
                    }
                    GenerateBitmap();
                }
            }
        }

        public Type GetType(int pX, int pY)
        {
            return _terrain[pX, pY];
        }
    }
}
