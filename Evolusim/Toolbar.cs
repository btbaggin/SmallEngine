using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SmallEngine;
using SmallEngine.Graphics;
using Evolusim.UI;

namespace Evolusim
{
    class Toolbar
    { 
        private float _width;
        private const float dx = 5;

        public bool IsOpen { get; private set; }

        SmallEngine.Graphics.Brush _background;

        ToggleButton _plainsButton;
        ToggleButton _waterButton;
        ToggleButton _desertButton;
        ToggleButton _forestButton;
        ToggleButton _mountainButton;

        public Terrain.Type SelectedType { get; private set; }

        public Toolbar()
        {
            _background = Game.Graphics.CreateBrush(System.Drawing.Color.Black);

            _plainsButton = new ToggleButton(ResourceManager.Request<BitmapResource>("plains"), "Plains", 150, 50);
            _waterButton = new ToggleButton(ResourceManager.Request<BitmapResource>("water"), "Water", 150, 50);
            _desertButton = new ToggleButton(ResourceManager.Request<BitmapResource>("desert"), "Desert", 150, 50);
            _forestButton = new ToggleButton(ResourceManager.Request<BitmapResource>("forest"), "Forest", 150, 50);
            _mountainButton = new ToggleButton(ResourceManager.Request<BitmapResource>("mountain"), "Mountains", 150, 50);
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawFillRect(new RectangleF(Game.Form.Width - _width, 0, _width, Game.Form.Height), _background);

            _plainsButton.Draw(pSystem);
            _waterButton.Draw(pSystem);
            _desertButton.Draw(pSystem);
            _forestButton.Draw(pSystem);
            _mountainButton.Draw(pSystem);
        }

        public void Update(float pDeltaTime)
        {
            _plainsButton.Update(pDeltaTime);
            _waterButton.Update(pDeltaTime);
            _desertButton.Update(pDeltaTime);
            _forestButton.Update(pDeltaTime);
            _mountainButton.Update(pDeltaTime);

            var left = Game.Form.Width - _width;
            _plainsButton.Position = new Vector2(left + 25, 50);
            _waterButton.Position = new Vector2(left + 25, 125);
            _desertButton.Position = new Vector2(left + 25, 200);
            _forestButton.Position = new Vector2(left + 25, 275);
            _mountainButton.Position = new Vector2(left + 25, 350);

            if (_plainsButton.IsSelected) SelectedType = Terrain.Type.Plains;
            if (_waterButton.IsSelected) SelectedType = Terrain.Type.Water;
            if (_desertButton.IsSelected) SelectedType = Terrain.Type.Desert;
            if (_forestButton.IsSelected) SelectedType = Terrain.Type.Forest;
            if (_mountainButton.IsSelected) SelectedType = Terrain.Type.Mountain;

            if(IsOpen)
            {
                _width += dx;
                if(_width >= Game.Form.Width / 5)
                {
                    _width = Game.Form.Width / 5;
                }
            }
            else
            {
                _width -= dx;
                if (_width <= 0)
                {
                    _width = 0;
                }
            }
        }
    }
}
