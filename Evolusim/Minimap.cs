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
        Vector2 _scale;
        int _resolution;
        BitmapResource _image;
        float _ratio;
        Brush _cameraOutline;

        public Minimap(Terrain pTerrain, int pSize, int pResolution)
        {
            _terrain = pTerrain;
            _scale = new Vector2(pSize);
            _resolution = pResolution;
            _ratio = _scale.X / Evolusim.WorldSize;
            _cameraOutline = Game.Graphics.CreateBrush(System.Drawing.Color.Black);
            Anchor = AnchorDirection.Top | AnchorDirection.Right;
            AnchorPoint = new Vector2(pSize);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            _terrain.BitmapData(ref _image, _resolution);

            pSystem.DrawBitmap(_image, 1, new Vector2(Game.Form.Width - _scale.X, 0), _scale);
            var x = Game.ActiveCamera.Position * _ratio;
            var w = Game.ActiveCamera.Width * _ratio;
            var h = Game.ActiveCamera.Height * _ratio;
            pSystem.DrawRect(new System.Drawing.RectangleF(Game.Form.Width - _scale.X + x.X, x.Y, w, h), _cameraOutline, 1);
        }

        public void Dispose()
        {
            _cameraOutline.Dispose();
        }
    }
}
