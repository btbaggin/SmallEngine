﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class Label : UIElement
    {
        private Font _font;
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                InvalidateMeasure();
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _fixed?.Dispose();
                _fixed = null;
                InvalidateMeasure();
            }
        }
        FixedText _fixed;

        public Label(string pText) : this(null, pText) { }

        public Label(string pName, string pText) : base(pName)
        {
            _font = Font.Create(UIManager.DefaultFontFamily, UIManager.DefaultFontSize, UIManager.DefaultFontColor, Game.Graphics);
            Font.Alignment = Alignments.Center;
            _text = pText;
            Enabled = false;
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawText(Text, Bounds, Font);
        }

        public override void Update() { }

        public override Size MeasureOverride(Size pSize)
        {
            if (_fixed == null) _fixed = _font.FixText(Text, pSize.Width);
            return _fixed.GetSize();
        }

        public override void Dispose()
        {
            _font.Dispose();
            base.Dispose();
        }
    }
}
