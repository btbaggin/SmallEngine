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
            float step = Game.Form.Height / (float)_height;
            for(int x = 0; x < _width; x++)
            {
                for(int y = 0; y < _height; y++)
                {
                    switch(_terrain[x,y])
                    {
                        case Type.Desert:
                            pSystem.DrawBitmap(_desert, 1, new Vector2(step * x, step * y), new Vector2(step, step));
                            break;

                        case Type.Forest:
                            pSystem.DrawBitmap(_forest, 1, new Vector2(step * x, step * y), new Vector2(step, step));
                            break;

                        case Type.Mountain:
                            pSystem.DrawBitmap(_mountain, 1, new Vector2(step * x, step * y), new Vector2(step, step));
                            break;

                        case Type.Plains:
                            pSystem.DrawBitmap(_plains, 1, new Vector2(step * x, step * y), new Vector2(step, step));
                            break;

                        case Type.Water:
                            pSystem.DrawBitmap(_water, 1, new Vector2(step * x, step * y), new Vector2(step, step));
                            break;
                    }
                }
            }
        }

        public void SetTypeAtMouse(Type pType)
        {
            var p = InputManager.MousePosition;
            if(p.X > 0 && p.X < Game.Form.Height && p.Y > 0 && p.Y < Game.Form.Height)
            {
                var step = Game.Form.Height / (float)_height;
                int x = (int)Math.Floor(p.X / step);
                int y = (int)Math.Floor(p.Y / step);
                _terrain[x, y] = pType;
            }

        }

        public Type GetType(int pX, int pY)
        {
            return _terrain[pX, pY];
        }
    }
}
