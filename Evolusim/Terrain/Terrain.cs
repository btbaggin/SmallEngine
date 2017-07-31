using System;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class Terrain
    {
        public enum Type
        {
            //None,
            //Mountain,
            //Water,
            //Plains,
            //Forest,
            //Desert,
            //Ice,
            //Snow
            None,
            Snow,
            Tundra,
            Bare,
            Scorched,
            Taiga,
            Shrubland,
            TemperateDesert,
            TemperateRain,
            TemperateDeciduous,
            Grassland,
            TropicalRain,
            TropicalSeasonal,
            SubtropicalDesert,
            Water
        }

        public const int BitmapSize = 64;
        public static int Size { get { return 513; } }
        static Type[,] _terrain;
        static bool _updateBitmap;

        float _bitmapWidth;
        float _bitmapHeight;

        Vector2 _scale;

        //BitmapResource _plains;
        //BitmapResource _water;
        //BitmapResource _mountain;
        //BitmapResource _forest;
        //BitmapResource _desert;
        //BitmapResource _ice;
        //BitmapResource _snow;
        Brush _water;
        Brush _subtropicalDesert;
        Brush _tropicalSeasonal;
        Brush _tropicalRain;
        Brush _grassland;
        Brush _temperateDeciduous;
        Brush _temperateRain;
        Brush _tempearteDesert;
        Brush _shrubland;
        Brush _taiga;
        Brush _scorched;
        Brush _bare;
        Brush _tundra;
        Brush _snow;
        Brush _none;

        HeightMap _height;
        HeightMap _climate;

        public Terrain()
        {
            _bitmapWidth = Size * BitmapSize;
            _bitmapHeight = Size * BitmapSize;
            _terrain = new Type[Size, Size];
            _scale = new Vector2(64);

            //_plains = ResourceManager.Request<BitmapResource>("plains");
            //_water = ResourceManager.Request<BitmapResource>("water");
            //_mountain = ResourceManager.Request<BitmapResource>("mountain");
            //_forest = ResourceManager.Request<BitmapResource>("forest");
            //_desert = ResourceManager.Request<BitmapResource>("desert");
            //_snow = ResourceManager.Request<BitmapResource>("snow");
            //_ice = ResourceManager.Request<BitmapResource>("ice");

            _water = Game.Graphics.CreateBrush(System.Drawing.Color.Blue);//
            _subtropicalDesert = Game.Graphics.CreateBrush(System.Drawing.Color.Brown);//
            _tropicalSeasonal = Game.Graphics.CreateBrush(System.Drawing.Color.Thistle);
            _tropicalRain = Game.Graphics.CreateBrush(System.Drawing.Color.Teal);
            _grassland = Game.Graphics.CreateBrush(System.Drawing.Color.Green);
            _temperateDeciduous = Game.Graphics.CreateBrush(System.Drawing.Color.DarkGreen);
            _temperateRain = Game.Graphics.CreateBrush(System.Drawing.Color.ForestGreen);
            _tempearteDesert = Game.Graphics.CreateBrush(System.Drawing.Color.SandyBrown);
            _shrubland = Game.Graphics.CreateBrush(System.Drawing.Color.LightSeaGreen);
            _taiga = Game.Graphics.CreateBrush(System.Drawing.Color.LawnGreen);
            _scorched = Game.Graphics.CreateBrush(System.Drawing.Color.Gray);//
            _bare = Game.Graphics.CreateBrush(System.Drawing.Color.LightYellow);
            _tundra = Game.Graphics.CreateBrush(System.Drawing.Color.LightGreen);//
            _snow = Game.Graphics.CreateBrush(System.Drawing.Color.White);//
            _none = Game.Graphics.CreateBrush(System.Drawing.Color.Black);

            _height = new HeightMap(true, Size);
            _climate = new HeightMap(true, Size);

            //Create terrain
            for (int x = 0; x < Size; x++)
            {
                for(int y = 0; y < Size; y++)
                {
                    _terrain[x, y] = CalculateType(x, y);
                }
            }

            //Create vegetation
            for (int x = 0; x < Terrain.Size; x++)
            {
                for (int y = 0; y < Terrain.Size; y++)
                {
                    if (RandomGenerator.RandomFloat() < .01f)
                    {
                        Vegetation.Create(x, y);
                    }
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
                    var s = _scale * Game.ActiveCamera.Zoom;
                    pSystem.DrawFillRect(new System.Drawing.RectangleF(currentX, currentY, s.X, s.Y), GetBrush(i, j));//, 1, new Vector2(currentX, currentY), _scale * Game.ActiveCamera.Zoom);
                    currentY += tileSize;
                }
                currentY = startPoint.Y;
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
                    var i = (int)(pResolution * 4 * y + x * 4);
                    var color = GetBrush(x * step, y * step).Color;
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

        private Brush GetBrush(int pX, int pY)
        {
            switch(_terrain[pX, pY])
            {
                case Type.Bare:
                    return _bare;
                case Type.Grassland:
                    return _grassland;
                case Type.Scorched:
                    return _scorched;
                case Type.Shrubland:
                    return _shrubland;
                case Type.SubtropicalDesert:
                    return _subtropicalDesert;
                case Type.Taiga:
                    return _taiga;
                case Type.TemperateDeciduous:
                    return _temperateDeciduous;
                case Type.TemperateDesert:
                    return _tempearteDesert;
                case Type.TemperateRain:
                    return _temperateRain;
                case Type.TropicalRain:
                    return _tropicalRain;
                case Type.TropicalSeasonal:
                    return _tropicalSeasonal;
                case Type.Tundra:
                    return _tundra;
                case Type.Snow:
                    return _snow;
                case Type.Water:
                    return _water;
                case Type.None:
                    return _none;
                default:
                    throw new Exception();
            }
        }


        //x axis is temp; y axis is height
        private Type[,] _terrainMap = { { Type.Snow,          Type.Snow,               Type.Snow,               Type.Tundra,           Type.Bare,            Type.Scorched},
                                        { Type.Taiga,         Type.Taiga,              Type.Shrubland,          Type.Shrubland,        Type.TemperateDesert, Type.TemperateDesert },
                                        { Type.TemperateRain, Type.TemperateDeciduous, Type.TemperateDeciduous, Type.Grassland,        Type.Grassland,       Type.TemperateDesert },
                                        { Type.TropicalRain,  Type.TropicalRain,       Type.TropicalSeasonal,   Type.TropicalSeasonal, Type.Grassland,       Type.SubtropicalDesert},
                                        { Type.Water,         Type.Water,              Type.Water,              Type.Water,            Type.Water,           Type.Water } };
        private Type CalculateType(int x, int y)
        {
            var h = _height.Query(x, y);
            var c = _climate.Query(x, y);

            //0 - 4
            int i = 0;
            if (h <  -.6) { i = 0; }
            else if(h < -.2) { i = 1; }
            else if(h < .2) { i = 2; }
            else if(h < .6) { i = 3; }
            else { i = 4; }

            //0 - 5
            int j = 0;
            if (c < -.7) { j = 0; }
            else if (c < -.3) { j = 1; }
            else if (c < .0) { j = 2; }
            else if (c < .3) { j = 3; }
            else if (c < .7) { j = 4; }
            else { j = 5; }

            return _terrainMap[i, j];
        }
    }
}
