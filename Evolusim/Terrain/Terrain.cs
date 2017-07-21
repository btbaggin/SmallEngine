using System;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class Terrain : IDrawable
    {
        public enum Type
        {
            None,
            Mountain,
            Water,
            Plains,
            Forest,
            Desert,
            Ice,
            Snow
        }

        public const int BitmapSize = 64;
        public static int Size { get { return 513; } }
        static Type[,] _terrain;
        static bool _updateBitmap;

        float _bitmapWidth;
        float _bitmapHeight;

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
            _bitmapWidth = Size * BitmapSize;
            _bitmapHeight = Size * BitmapSize;
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
            _updateBitmap = true;
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            int x = (int)(Evolusim.ActiveCamera.Position.X / BitmapSize);
            int y = (int)(Evolusim.ActiveCamera.Position.Y / BitmapSize);
            float numTilesX = Evolusim.ActiveCamera.Width / BitmapSize;
            float numTilesY = Evolusim.ActiveCamera.Height / BitmapSize;

            //Width and height should be the same
            var tileSize = (Game.Form.Width / numTilesX);
            var startPoint = Evolusim.ActiveCamera.ToCameraSpace(new Vector2(x * BitmapSize, y * BitmapSize));

            Vector2 scale = new Vector2(tileSize);
            var currentX = startPoint.X;
            var currentY = startPoint.Y;
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

        public static void SetTypeAt(Type pType, Vector2 pPoint)
        {
            int x = (int)Math.Floor(pPoint.X / BitmapSize);
            int y = (int)Math.Floor(pPoint.Y / BitmapSize);
            if(x >= 0 && y >= 0 && x < _terrain.GetUpperBound(0) && y < _terrain.GetUpperBound(1))
            {
                if (_terrain[x, y] != pType)
                {
                    _terrain[x, y] = pType;
                }
            }
            _updateBitmap = true;
        }

        public static Type GetTypeAt(Vector2 pPosition)
        {
            int x = (int)Math.Floor(pPosition.X / BitmapSize);
            int y = (int)Math.Floor(pPosition.Y / BitmapSize);
            if(x >= 0 && y >= 0 && x < _terrain.GetUpperBound(0) && y < _terrain.GetUpperBound(1))
            {
                return GetType(x, y);
            }

            return Type.None;
        }

        public static Type GetType(int pX, int pY)
        {
            return _terrain[pX, pY];
        }

        public static Vector2 GetTile(Vector2 pPosition)
        {
            return new Vector2((float)Math.Floor(pPosition.X / BitmapSize), (float)Math.Floor(pPosition.Y / BitmapSize));
        }

        public static Vector2 GetPosition(Vector2 pTile)
        {
            return pTile * BitmapSize;
        }

        internal void BitmapData(ref BitmapResource pResource, int pResolution)
        {
            if (!_updateBitmap)
                return;

            var memory = new byte[pResolution * pResolution * 4];
            var step = (int)Math.Floor((double)Terrain.Size / pResolution);
            for (int x = 0; x < pResolution; x++)
            {
                for (int y = 0; y < pResolution; y++)
                {
                    Terrain.Type t = GetType(x * step, y * step);

                    var i = (int)(pResolution * 4 * y + x * 4);
                    var color = ColorFromType(t);
                    memory[i] = color.R;
                    memory[i + 1] = color.G;
                    memory[i + 2] = color.B;
                    memory[i + 3] = color.A;
                }
            }

            if (pResource != null) pResource.Dispose();
            pResource = ((DirectXGraphicSystem)Game.Graphics).FromByte(memory, pResolution, pResolution);
            _updateBitmap = false;
        }

        private System.Drawing.Color ColorFromType(Terrain.Type pType)
        {
            switch (pType)
            {
                case Terrain.Type.Desert:
                    return System.Drawing.Color.Brown;
                case Terrain.Type.Forest:
                    return System.Drawing.Color.DarkGreen;
                case Terrain.Type.Ice:
                    return System.Drawing.Color.White;
                case Terrain.Type.Mountain:
                    return System.Drawing.Color.Gray;
                case Terrain.Type.Plains:
                    return System.Drawing.Color.Green;
                case Terrain.Type.Snow:
                    return System.Drawing.Color.Snow;
                case Terrain.Type.Water:
                    return System.Drawing.Color.Blue;
                case Terrain.Type.None:
                    return System.Drawing.Color.Black;
                default:
                    throw new Exception();
            }
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
