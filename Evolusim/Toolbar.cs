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
        private float _width = 200;
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

            var left = Game.Form.Height + 25;
            var top = 50;
            _plainsButton = new ToggleButton(ResourceManager.Request<BitmapResource>("plains"),
                                       "Plains", left, top, 150, 50);
            _waterButton = new ToggleButton(ResourceManager.Request<BitmapResource>("water"),
                                       "Water", left, top + 75, 150, 50);
            _desertButton = new ToggleButton(ResourceManager.Request<BitmapResource>("desert"),
                                       "Desert", left, top + 150, 150, 50);
            _forestButton = new ToggleButton(ResourceManager.Request<BitmapResource>("forest"),
                                       "Forest", left, top + 225, 150, 50);
            _mountainButton = new ToggleButton(ResourceManager.Request<BitmapResource>("mountain"),
                                       "Mountains", left, top + 300, 150, 50);
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawFillRect(new RectangleF(Game.Form.Height, 0, _width, Game.Form.Height), _background);

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

            if (_plainsButton.IsSelected) SelectedType = Terrain.Type.Plains;
            if (_waterButton.IsSelected) SelectedType = Terrain.Type.Water;
            if (_desertButton.IsSelected) SelectedType = Terrain.Type.Desert;
            if (_forestButton.IsSelected) SelectedType = Terrain.Type.Forest;
            if (_mountainButton.IsSelected) SelectedType = Terrain.Type.Mountain;
        }
    }
}
