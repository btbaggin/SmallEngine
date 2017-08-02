using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.UI;

using Evolusim.Terrain;

namespace Evolusim.UI
{
    class InspectionBar : UIElement, IMessageReceiver, IDisposable
    { 
        private const float dx = 10;

        public bool IsOpen { get; private set; }

        SmallEngine.Graphics.Brush _background;
        private Organism _organism;

        public InspectionBar() : base()
        {
            _background = Game.Graphics.CreateBrush(Color.FromArgb(150, 0, 0, 0));
            WidthPercent = .2f;
            HeightPercent = 1f;
            Position = new Vector2(-Width, 0);
            Orientation = ElementOrientation.Vertical;
            Order = 10;

            MessageBus.Register(this);
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

        private void UpdateContent()
        {
            Children.Clear();
            foreach (TraitComponent.Traits e in Enum.GetValues(typeof(TraitComponent.Traits)))
            {
                var t = _organism.GetTrait(e);
                AddChild(new AttributeElement(e.ToString(), t.Value, t.Min, t.Max), AnchorDirection.Left | AnchorDirection.Top, Vector2.Zero);
            }
            SetLayout();
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawFillRect(new RectangleF(Position.X, Position.Y, Width, Height), _background);
            base.Draw(pSystem);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

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
                case "ToolbarOpen":
                    IsOpen = true;
                    _organism = pM.GetValue<Organism>();
                    Game.ActiveCamera.Zoom = 3;
                    Game.ActiveCamera.Follow(_organism);

                    UpdateContent();
                    break;
            }
        }

        public void Dispose()
        {
            _background.Dispose();
        }
    }
}
