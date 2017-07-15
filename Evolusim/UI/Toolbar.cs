using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.UI;

namespace Evolusim.UI
{
    class Toolbar : StackPanel
    { 
        private const float dx = 5;

        public bool IsOpen { get; private set; }

        SmallEngine.Graphics.Brush _background;

        //ToggleButton _plainsButton;
        //ToggleButton _waterButton;
        //ToggleButton _desertButton;
        //ToggleButton _forestButton;
        //ToggleButton _mountainButton;

        public Terrain.Type SelectedType { get; private set; }

        public Toolbar() : base(Orientation.Vertical)
        {
            _background = Game.Graphics.CreateBrush(Color.FromArgb(150, 0, 0, 0));
            WidthPercent = .2f;
            HeightPercent = 1f;
            Position = new Vector2(Game.Form.Width, 0);

            var group = new ToggleButtonGroup();
            var anchor = AnchorDirection.Left | AnchorDirection.Top;
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("plains"), "Plains", group), anchor, Vector2.Zero);
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("water"), "Water", group), anchor, Vector2.Zero);
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("desert"), "Desert", group), anchor, Vector2.Zero);
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("forest"), "Forest", group), anchor, Vector2.Zero);
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("mountain"), "Mountains", group), anchor, Vector2.Zero);
            Measure(new Size(Width, Height), Position);
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawFillRect(new RectangleF(Position.X, Position.Y, Width, Height), _background);
            base.Draw(pSystem);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);
            //if (_plainsButton.IsSelected) SelectedType = Terrain.Type.Plains;
            //if (_waterButton.IsSelected) SelectedType = Terrain.Type.Water;
            //if (_desertButton.IsSelected) SelectedType = Terrain.Type.Desert;
            //if (_forestButton.IsSelected) SelectedType = Terrain.Type.Forest;
            //if (_mountainButton.IsSelected) SelectedType = Terrain.Type.Mountain;

            if(IsOpen)
            {
                Position = new Vector2(Position.X - dx, 0);
                if(Position.X <= Game.Form.Width - Game.Form.Width / 5)
                {
                    Position = new Vector2(Game.Form.Width - Game.Form.Width / 5, 0);
                }
            }
            else
            {
                Position = new Vector2(Position.X + dx, 0);
                if (Position.X >= Game.Form.Width)
                {
                    Position = new Vector2(Game.Form.Width, 0);
                }
            }
        }
    }
}
