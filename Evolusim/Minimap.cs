using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class Minimap
    {
        Terrain _terrain;
        Vector2 _scale;
        int _resolution;
        BitmapResource _image;
        float _ratio;
        Brush _cameraOutline;

        //TODO needs to be UI
        public Minimap(Terrain pTerrain, int pSize, int pResolution)
        {
            _terrain = pTerrain;
            _scale = new Vector2(pSize);
            _resolution = pResolution;
            _ratio = _scale.X / Evolusim.WorldSize;
            _cameraOutline = Game.Graphics.CreateBrush(System.Drawing.Color.Black);
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            UpdateBitmap();

            pSystem.DrawBitmap(_image, 1, new Vector2(Game.Form.Width - _scale.X, 0), _scale);
            var x = Game.ActiveCamera.Position * _ratio;
            var w = Game.ActiveCamera.Width * _ratio;
            var h = Game.ActiveCamera.Height * _ratio;
            pSystem.DrawRect(new System.Drawing.RectangleF(Game.Form.Width - _scale.X + x.X, x.Y, w, h), _cameraOutline, 1);
            _image.Dispose();
        }

        private void UpdateBitmap()
        {
            var memory = new byte[_resolution * _resolution * 4];
            var step = (int)Math.Floor((double)Terrain.Size / _resolution);
            for (int x = 0; x < _resolution; x++)
            {
                for(int y = 0; y < _resolution; y++)
                {
                    Terrain.Type t = _terrain.GetType(x * step, y * step);

                    var i = (int)(_resolution * 4 * y + x * 4);
                    var color = ColorFromType(t);
                    memory[i] = color.R;
                    memory[i + 1] = color.G;
                    memory[i + 2] = color.B;
                    memory[i + 3] = color.A;
                }
            }

            _image = ((DirectXGraphicSystem)Game.Graphics).FromByte(memory, _resolution, _resolution);
        }

        private System.Drawing.Color ColorFromType(Terrain.Type pType)
        {
            switch(pType)
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
                default:
                    throw new Exception();
            }
        }
    }
}
