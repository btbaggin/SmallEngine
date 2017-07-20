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

            _group = new ToggleButtonGroup(new ToggleButton("plains", "Plains", Terrain.Type.Plains) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) },
                new ToggleButton("water", "Water", Terrain.Type.Water) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) },
                new ToggleButton("desert", "Desert", Terrain.Type.Desert) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) },
                new ToggleButton("forest", "Forest", Terrain.Type.Forest) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) },
                new ToggleButton("mountain", "Mountains", Terrain.Type.Mountain) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) });
            AddChild(_group, AnchorDirection.Left | AnchorDirection.Top, Vector2.Zero);
            SetLayout();
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }

        public void Hide()
        {
            IsOpen = false;
            Position = new Vector2(-Width, Position.Y);
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
