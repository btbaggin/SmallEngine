using System;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.UI;
using SmallEngine.Messages;

namespace Evolusim.UI
{
    class InspectionBar : UIElement, IMessageReceiver, IDisposable
    { 
        private const float dx = 10;

        public bool IsOpen { get; private set; }

        readonly Brush _background;
        private Organism _organism;

        public InspectionBar() : base()
        {
            _background = Game.Graphics.CreateBrush(System.Drawing.Color.FromArgb(150, 0, 0, 0));
            WidthPercent = .2f;
            HeightPercent = 1f;
            Position = new Vector2(-Width, 0);
            Orientation = ElementOrientation.Vertical;
            Order = 10;

            Game.Messages.Register(this);
        }

        public void Toggle()
        {
            Visible = true;
            IsOpen = !IsOpen;
        }

        public void Hide()
        {
            Visible = false;
        }

        private void UpdateContent()
        {
            Children.Clear();
            foreach (var t in _organism.GetStats())
            {
                AddChild(new AttributeElement(t.Item1, t.Item2), AnchorDirection.Left | AnchorDirection.Top, Vector2.Zero);
            }
            SetLayout();
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawFillRect(new Rectangle(Position, Width, Height), _background);
            base.Draw(pSystem);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if(_organism != null)
            {
                var l = _organism.GetStats();
                foreach (var c in Children)
                {
                    var a = ((AttributeElement)c);
                    foreach (var s in l)
                    {
                        if (s.Item1 == a.Attribute)
                        {
                            a.UpdateValue(s.Item2);
                        }
                    }
                }
            }
            
            if(IsOpen)
            {
                Position = new Vector2(Math.Min(Position.X + dx, 0), Position.Y);
            }
            else
            {
                Position = new Vector2(Math.Max(Position.X - dx, -Width), Position.Y);
            }
        }

        public void ReceiveMessage(IMessage pMessage)
        {
            switch(pMessage.Type)
            {
                case "ToolbarOpen":
                    IsOpen = true;
                    _organism = pMessage.GetData<Organism>();
                    Organism.SelectedOrganism = _organism;
                    Game.ActiveCamera.Zoom = 1;
                    Game.ActiveCamera.Follow(_organism);

                    UpdateContent();
                    break;

                case "ToolbarClose":
                    IsOpen = false;
                    _organism = null;
                    Organism.SelectedOrganism = null;
                    Game.ActiveCamera.StopFollow();
                    break;
            }
        }

        public void Dispose()
        {
            _background.Dispose();
        }
    }
}
