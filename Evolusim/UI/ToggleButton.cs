using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;
using SmallEngine.UI;

namespace Evolusim.UI
{
    class ToggleButton : UIElement
    {
        Brush _highlightBrush;

        public bool IsSelected { get; internal set; }

        public object Data { get; private set; }

        public ToggleButton(string pAlias, string pText, object pData)
        {
            WidthPercent = .2f;
            HeightPercent = 1f;
            Orientation = ElementOrientation.Vertical;
            Data = pData;
            AddChild(new ImageElement(pAlias) { WidthPercent = 1, HeightPercent = .75f }, AnchorDirection.Top | AnchorDirection.Left, Vector2.Zero);
            AddChild(new LabelElement(pText, "Arial", 12, System.Drawing.Color.White) { WidthPercent = 1, HeightPercent = .25f }, AnchorDirection.Top | AnchorDirection.Left, Vector2.Zero);

            _highlightBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Yellow);
            SetLayout();
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);
            if(IsSelected)
            {
                pSystem.DrawRect(new System.Drawing.RectangleF(Position.X, Position.Y, Width, Height), _highlightBrush, 3);
            }
        }
    }
}
