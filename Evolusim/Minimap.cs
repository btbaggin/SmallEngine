using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.UI;
using SmallEngine.Graphics;

namespace Evolusim
{
    class Minimap : UIElement, IDisposable
    {
        Terrain _terrain;
        int _resolution;
        BitmapResource _image;
        float _ratio;
        Brush _cameraOutline;

        public Minimap(Terrain pTerrain, int pSize, int pResolution)
        {
            Anchor = AnchorDirection.Top | AnchorDirection.Right;
            AnchorPoint = Vector2.Zero;
            Width = pSize;
            Height = pSize;
            Order = 1;
            _terrain = pTerrain;
            _resolution = pResolution;
            _ratio = (float)Width / Evolusim.WorldSize;
            _cameraOutline = Game.Graphics.CreateBrush(System.Drawing.Color.Black);
            SetLayout();
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            _terrain.BitmapData(ref _image, _resolution);

            pSystem.DrawBitmap(_image, 1, Position, new Vector2(Width, Height));
            var x = Game.ActiveCamera.Position * _ratio;
            var w = Game.ActiveCamera.Width * _ratio;
            var h = Game.ActiveCamera.Height * _ratio;
            pSystem.DrawRect(new System.Drawing.RectangleF(Position.X + x.X, Position.Y + x.Y, w, h), _cameraOutline, 1);
        }

        public void Dispose()
        {
            _cameraOutline.Dispose();
        }
    }
}
