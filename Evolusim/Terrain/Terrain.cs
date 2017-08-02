using System;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim.Terrain
{
    class TerrainMap
    {
        public const int BitmapSize = 64;
        public static int Size { get { return 513; } }
        static TerrainType[,] _terrain;
        static bool _updateBitmap;

        float _bitmapWidth;
        float _bitmapHeight;

        Vector2 _scale;

        Brush _water;
        Brush _tropicalSeasonal;
        Brush _tropicalRain;
        Brush _grassland;
        Brush _temperateDeciduous;
        Brush _temperateRain;
        Brush _desert;
        Brush _shrubland;
        Brush _taiga;
        Brush _scorched;
        Brush _bare;
        Brush _tundra;
        Brush _snow;
        Brush _ice;
        Brush _none;

        HeightMap _height;
        HeightMap _climate;

        public TerrainMap()
        {
            _bitmapWidth = Size * BitmapSize;
            _bitmapHeight = Size * BitmapSize;
            _terrain = new TerrainType[Size, Size];
            _scale = new Vector2(64);

            _water = Game.Graphics.CreateBrush(System.Drawing.Color.Blue);//done
            _desert = Game.Graphics.CreateBrush(System.Drawing.Color.SandyBrown);//done
            _tropicalSeasonal = Game.Graphics.CreateBrush(System.Drawing.Color.Thistle);
            _tropicalRain = Game.Graphics.CreateBrush(System.Drawing.Color.Teal);
            _grassland = Game.Graphics.CreateBrush(System.Drawing.Color.Green);
            _temperateDeciduous = Game.Graphics.CreateBrush(System.Drawing.Color.DarkGreen);
            _temperateRain = Game.Graphics.CreateBrush(System.Drawing.Color.ForestGreen);
            _shrubland = Game.Graphics.CreateBrush(System.Drawing.Color.DarkOliveGreen);
            _taiga = Game.Graphics.CreateBrush(System.Drawing.Color.LawnGreen);
            _scorched = Game.Graphics.CreateBrush(System.Drawing.Color.Gray);//
            _bare = Game.Graphics.CreateBrush(System.Drawing.Color.LightYellow);
            _tundra = Game.Graphics.CreateBrush(System.Drawing.Color.LightGreen);//done
            _snow = Game.Graphics.CreateBrush(System.Drawing.Color.White);//done
            _ice = Game.Graphics.CreateBrush(System.Drawing.Color.White);//done
            _none = Game.Graphics.CreateBrush(System.Drawing.Color.Black);//done

            _height = new HeightMap(true, Size);
            _climate = new HeightMap(true, Size);

            //Create terrain
            for (int x = 0; x < Size; x++)
            {
                for(int y = 0; y < Size; y++)
                {
                    _terrain[x, y] = CalculateTerrainType(x, y);
                }
            }

            _updateBitmap = true;
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            int x = (int)(Evolusim.ActiveCamera.Position.X / BitmapSize);
            int y = (int)(Evolusim.ActiveCamera.Position.Y / BitmapSize);
            float numTilesX = Game.Form.Width / (BitmapSize * Game.ActiveCamera.Zoom);
            float numTilesY = Game.Form.Height / (BitmapSize * Game.ActiveCamera.Zoom);

            //Width and height should be the same
            var startPoint = Evolusim.ActiveCamera.ToCameraSpace(new Vector2(x * BitmapSize, y * BitmapSize));

            Vector2 scale = new Vector2(BitmapSize) * Game.ActiveCamera.Zoom;//new Vector2(tileSize);
            var currentX = startPoint.X;
            var currentY = startPoint.Y;
            for(int i = x; i <= x + numTilesX; i++)
            {
                if (i >= Size) break;
                for(int j = y; j <= y + numTilesY; j++)
                {
                    if (j >= Size) break;
                    pSystem.DrawFillRect(new System.Drawing.RectangleF(currentX, currentY, scale.X + 1, scale.Y + 1), GetBrush(i, j));
                    currentY += scale.Y;
                }
                currentY = startPoint.Y;
                currentX += scale.X;
            }
        }

        public static void SetTerrainTypeAt(TerrainType pTerrainType, Vector2 pPoint)
        {
            int x = (int)Math.Floor(pPoint.X / BitmapSize);
            int y = (int)Math.Floor(pPoint.Y / BitmapSize);
            if(x >= 0 && y >= 0 && x < _terrain.GetUpperBound(0) && y < _terrain.GetUpperBound(1))
            {
                if (_terrain[x, y] != pTerrainType)
                {
                    _terrain[x, y] = pTerrainType;
                }
            }
            _updateBitmap = true;
        }

        public static TerrainType GetTerrainTypeAt(Vector2 pPosition)
        {
            int x = (int)Math.Floor(pPosition.X / BitmapSize);
            int y = (int)Math.Floor(pPosition.Y / BitmapSize);
            if(x >= 0 && y >= 0 && x < _terrain.GetUpperBound(0) && y < _terrain.GetUpperBound(1))
            {
                return GetTerrainType(x, y);
            }

            return TerrainType.None;
        }

        public static TerrainType GetTerrainType(int pX, int pY)
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
            var step = (int)Math.Floor((double)Size / pResolution);
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
                case TerrainType.Bare:
                    return _bare;
                case TerrainType.Grassland:
                    return _grassland;
                case TerrainType.Scorched:
                    return _scorched;
                case TerrainType.Shrubland:
                    return _shrubland;
                case TerrainType.Taiga:
                    return _taiga;
                case TerrainType.TemperateDeciduous:
                    return _temperateDeciduous;
                case TerrainType.Desert:
                    return _desert;
                case TerrainType.TemperateRain:
                    return _temperateRain;
                case TerrainType.TropicalRain:
                    return _tropicalRain;
                case TerrainType.TropicalSeasonal:
                    return _tropicalSeasonal;
                case TerrainType.Tundra:
                    return _tundra;
                case TerrainType.Snow:
                    return _snow;
                case TerrainType.Water:
                    return _water;
                case TerrainType.Ice:
                    return _ice;
                case TerrainType.None:
                    return _none;
                default:
                    throw new Exception();
            }
        }


        //x axis is temp; y axis is height
        private TerrainType[,] _terrainMap = { { TerrainType.Snow,          TerrainType.Snow,               TerrainType.Snow,               TerrainType.Tundra,           TerrainType.Bare,      TerrainType.Scorched },
                                        { TerrainType.Taiga,         TerrainType.Taiga,              TerrainType.Shrubland,          TerrainType.Shrubland,        TerrainType.Desert,    TerrainType.Desert },
                                        { TerrainType.TemperateRain, TerrainType.TemperateDeciduous, TerrainType.TemperateDeciduous, TerrainType.Grassland,        TerrainType.Grassland, TerrainType.Desert },
                                        { TerrainType.TropicalRain,  TerrainType.TropicalRain,       TerrainType.TropicalSeasonal,   TerrainType.TropicalSeasonal, TerrainType.Grassland, TerrainType.Desert },
                                        { TerrainType.Water,         TerrainType.Water,              TerrainType.Water,              TerrainType.Water,            TerrainType.Water,     TerrainType.Ice } };
        private TerrainType CalculateTerrainType(int x, int y)
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
