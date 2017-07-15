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
            Desert,
            Ice,
            Snow
        }

        public static int Size { get { return 513; } }

        float _bitmapWidth;
        float _bitmapHeight;

        Type[,] _terrain;

        BitmapResource _plains;
        BitmapResource _water;
        BitmapResource _mountain;
        BitmapResource _forest;
        BitmapResource _desert;
        BitmapResource _ice;
        BitmapResource _snow;
        HeightMap _height;
        HeightMap _climate;

        public Terrain()
        {
            _bitmapWidth = Size * 64;
            _bitmapHeight = Size * 64;
            _terrain = new Type[Size, Size];

            _plains = ResourceManager.Request<BitmapResource>("plains");
            _water = ResourceManager.Request<BitmapResource>("water");
            _mountain = ResourceManager.Request<BitmapResource>("mountain");
            _forest = ResourceManager.Request<BitmapResource>("forest");
            _desert = ResourceManager.Request<BitmapResource>("desert");
            _snow = ResourceManager.Request<BitmapResource>("snow");
            _ice = ResourceManager.Request<BitmapResource>("ice");

            _height = new HeightMap(false, Size);
            _climate = new HeightMap(true, Size);

            for (int x = 0; x < Size; x++)
            {
                for(int y = 0; y < Size; y++)
                {
                    _terrain[x, y] = CalculateType(x, y);
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
            var tileSize = (int)(Game.Form.Width / numTilesX);
            var startPoint = Evolusim.ActiveCamera.ToCameraSpace(new Vector2(x * 64, y * 64));

            Vector2 scale = new Vector2(tileSize, tileSize);
            var currentX = (int)startPoint.X;
            var currentY = (int)startPoint.Y;
            for(int i = x; i <= x + numTilesX + 2; i++)
            {
                if (i >= Size) break;
                for(int j = y; j <= y + numTilesY + 2; j++)
                {
                    if (j >= Size) break;
                    pSystem.DrawBitmap(GetBitmap(i, j), 1, new Vector2(currentX, currentY), scale);
                    currentY += tileSize;
                }
                currentY = (int)startPoint.Y;
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

        private BitmapResource GetBitmap(int x, int y)
        {
            switch (_terrain[x, y])
            {
                case Type.Desert:
                    return _desert;
                case Type.Forest:
                    return _forest;
                case Type.Mountain:
                    return _mountain;
                case Type.Plains:
                    return _plains;
                case Type.Water:
                    return _water;
                case Type.Snow:
                    return _snow;
                case Type.Ice:
                    return _ice;
            }

            return null;
        }
        //x axis is temp; y axis is height
        private Type[,] _terrainMap = { { Type.Water,    Type.Water,    Type.Water,  Type.Ice,    Type.Ice },
                                        { Type.Plains,   Type.Plains,   Type.Plains, Type.Plains, Type.Plains },
                                        { Type.Forest,   Type.Forest,   Type.Forest, Type.Forest, Type.Forest },
                                        { Type.Desert,   Type.Desert,   Type.Desert, Type.Desert, Type.Desert },
                                        { Type.Mountain, Type.Mountain, Type.Snow,   Type.Snow,   Type.Snow } };
        private Type CalculateType(int x, int y)
        {
            var h = _height.Query(x, y);
            var c = _climate.Query(x, y);

            int i = 0;
            if (h <  -.4) { i = 0; }
            else if(h < 0) { i = 1; }
            else if(h < .3) { i = 2; }
            else if(h < .6) { i = 3; }
            else { i = 4; }

            int j = 0;
            if (c < -.4) { j = 0; }
            else if (c < 0) { j = 1; }
            else if (c < .3) { j = 2; }
            else if (c < .6) { j = 3; }
            else { j = 4; }

            return _terrainMap[i, j];
        }
    }
}
