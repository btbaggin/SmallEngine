using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace SmallEngine.UI
{
    public class Textbox : UIElement
    {
        public Color Background
        {
            get { return _backgroundBrush.FillColor; }
            set { _backgroundBrush.FillColor = value; }
        }

        public Color Foreground
        {
            get { return _foregroundBrush.FillColor; }
            set { _foregroundBrush.FillColor = value; }
        }

        public Font Font { get; set; }

        public string Text
        {
            get { return _text.ToString(); }
            set
            {
                _text.Clear();
                _text.Append(value);
            }
        }

        public bool IsFocused { get; private set; }

        StringBuilder _text = new StringBuilder();
        int _cursorPos;
        float _cursorTick;
        bool _showCursor;
        readonly Brush _backgroundBrush, _foregroundBrush;

        public Textbox() : this(null) { }

        public Textbox(string pName) : base(pName)
        {
            Font = Font.Create(UIManager.DefaultFontFamily, UIManager.DefaultFontSize, UIManager.DefaultFontColor, Game.Graphics);
            Font.Alignment = Alignments.Leading;

            _backgroundBrush = Brush.CreateFillBrush(Color.Gray, Game.Graphics);
            _foregroundBrush = Brush.CreateFillBrush(UIManager.DefaultFontColor, Game.Graphics);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawRect(Bounds, _backgroundBrush);

            pSystem.DrawText(Text, Bounds, Font, true); //TODO clip and navigation around cursor

            if(_showCursor && IsFocused)
            {
                var s = Font.MeasureString(Text.Substring(0, _cursorPos), ActualWidth);
                var x = Bounds.Left + s.Width + 1;
                pSystem.DrawLine(new Vector2(x, Bounds.Top + 1), new Vector2(x, Bounds.Bottom - 1), _foregroundBrush);
            }
        }

        public override void Update()
        {
            //TODO caret navigation using mouse
            if(Mouse.ButtonPressed(MouseButtons.Left))
            {
                IsFocused = IsMouseOver();
            }

            if (!IsFocused) return;

            //Toggle cursor every half second
            _cursorTick += GameTime.UnscaledDeltaTime;
            if(_cursorTick >= .5f)
            {
                _showCursor = !_showCursor;
                _cursorTick = 0;
            }

            //Text input
            var keys = Enum.GetValues(typeof(Keys));
            for(int i = 0; i < keys.Length; i++)
            {
                var k = (Keys)keys.GetValue(i);
                if (Keyboard.KeyPressed(k))
                {
                    var text = Keyboard.ToUnicode(k);
                    if(text != "\b")
                    {
                        _text = _text.Insert(_cursorPos, text);
                        _cursorPos += text.Length;
                    }
                }
            }

            //Cursor navigation
            //Always show cursor when navigating
            if (_cursorPos > 0 && Keyboard.KeyPressed(Keys.Left))
            {
                _cursorPos--;
                _cursorTick = 0;
                _showCursor = true;
            }
            else if (_cursorPos < _text.Length && Keyboard.KeyPressed(Keys.Right))
            {
                _cursorPos++;
                _cursorTick = 0;
                _showCursor = true;
            }

            //Text deletion
            if(Keyboard.KeyPressed(Keys.Backspace) && _cursorPos > 0)
            {
                _text = _text.Remove(_cursorPos - 1, 1);
                _cursorPos--;
            }
            if(Keyboard.KeyPressed(Keys.Delete) && _cursorPos < _text.Length)
            {
                _text = _text.Remove(_cursorPos, 1);
            }
        }

        public override Size MeasureOverride(Size pSize)
        {
            var s = Font.MeasureString(Text, pSize.Width);
            return new Size(pSize.Width, s.Height);
        }

        public override void Dispose()
        {
            _backgroundBrush.Dispose();
            _foregroundBrush.Dispose();
            Font.Dispose();
            base.Dispose();
        }
    }
}
