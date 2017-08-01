﻿using System;
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
    class Toolbar : UIElement, IMessageReceiver, IDisposable
    { 
        private const float dx = 10;

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

            //_group = new ToggleButtonGroup(new ToggleButton("plains", "Plains", Terrain.Type.Bare) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) },
            //    new ToggleButton("water", "Water", Terrain.Type.Water) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) },
            //    new ToggleButton("desert", "Desert", Terrain.Type.SubtropicalDesert) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) },
            //    new ToggleButton("forest", "Forest", Terrain.Type.TropicalRain) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) },
            //    new ToggleButton("mountain", "Mountains", Terrain.Type.Scorched) { Orientation = ElementOrientation.Vertical, Margin = new Vector2(2, 0) });
            //AddChild(_group, AnchorDirection.Left | AnchorDirection.Top, Vector2.Zero);
            SetLayout();

            MessageBus.Register(this);
            SelectedType = Terrain.Type.None;
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

        public void SetContent(UIElement pContent)
        {
            Children.Clear();
            AddChild(pContent, AnchorDirection.Left | AnchorDirection.Top, Vector2.Zero);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawFillRect(new RectangleF(Position.X, Position.Y, Width, Height), _background);
            base.Draw(pSystem);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);
            //SelectedType = _group.GetSelectedData<Terrain.Type>();

            if(IsOpen)
            {
                Position = new Vector2(Math.Min(Position.X + dx, 0), Position.Y);
            }
            else
            {
                Position = new Vector2(Math.Max(Position.X - dx, -Width), Position.Y);
            }
        }

        public void ReceiveMessage(GameMessage pM)
        {
            switch(pM.MessageType)
            {
                case "ToolbarToggle":
                    IsOpen = !IsOpen;
                    break;

                case "ToolbarOpen":
                    IsOpen = true;
                    break;
            }
        }

        public void Dispose()
        {
            _background.Dispose();
        }
    }
}
