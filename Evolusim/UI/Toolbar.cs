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
    class Toolbar : UIElement, IDisposable
    { 
        private const float dx = 5;

        public bool IsOpen { get; private set; }

        SmallEngine.Graphics.Brush _background;

        ToggleButtonGroup _group;

        public Terrain.Type SelectedType { get; private set; }

        public Toolbar() : base()
        {
            _background = Game.Graphics.CreateBrush(Color.FromArgb(150, 0, 0, 0));
            WidthPercent = .2f;
            HeightPercent = 1f;
            Position = new Vector2(-Width, 0);
            Orientation = ElementOrientation.Vertical;
            Order = 10;

            _group = new ToggleButtonGroup();
            var anchor = AnchorDirection.Left | AnchorDirection.Top;
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("plains"), "Plains", Terrain.Type.Plains, _group), anchor, Vector2.Zero);
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("water"), "Water", Terrain.Type.Water, _group), anchor, Vector2.Zero);
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("desert"), "Desert", Terrain.Type.Desert, _group), anchor, Vector2.Zero);
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("forest"), "Forest", Terrain.Type.Forest, _group), anchor, Vector2.Zero);
            AddChild(new ToggleButton(ResourceManager.Request<BitmapResource>("mountain"), "Mountains", Terrain.Type.Mountain, _group), anchor, Vector2.Zero);
            SetLayout();
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
            SelectedType = _group.GetSelectedData<Terrain.Type>();

            if(IsOpen)
            {
                Position = new Vector2(Position.X + dx, Position.Y);
                if(Position.X >= 0)
                {
                    Position = new Vector2(0, Position.Y);
                }
            }
            else
            {
                Position = new Vector2(Position.X - dx, Position.Y);
                if (Position.X <= -Width)
                {
                    Position = new Vector2(-Width, Position.Y);
                }
            }
        }

        public void Dispose()
        {
            _background.Dispose();
        }
    }
}
