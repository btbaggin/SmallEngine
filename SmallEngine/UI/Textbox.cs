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
        public EventHandler<FocusChangedEventArgs> FocusChanged { get; set; }

        public EventHandler<EventArgs> Enter { get; set; }

        public Brush Background { get; set; }

        public Pen Cursor { get; set; }

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

        public Textbox() : this(null) { }

        public Textbox(string pName) : base(pName)
        {
            Font = Font.Create(UIManager.DefaultFontFamily, UIManager.DefaultFontSize, UIManager.DefaultFontColor, Game.Graphics);
            Font.Alignment = Alignments.Leading;

            Background = SolidColorBrush.Create(Color.Gray);
            Cursor = Pen.Create(UIManager.DefaultFontColor, 1);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawRect(Bounds, Background);

            pSystem.DrawText(Text, Bounds, Font, true); //TODO clip, DrawTextLayout?

            if(_showCursor && IsFocused)
            {
                var s = Font.MeasureString(Text.Substring(0, _cursorPos), ActualWidth);
                var x = Bounds.Left + s.Width + 1;
                pSystem.DrawLine(new Vector2(x, Bounds.Top + 1), new Vector2(x, Bounds.Bottom - 1), Cursor);
            }
        }

        public override void Update()
        {
            if(Mouse.ButtonPressed(MouseButtons.Left))
            {
                var isFocused = IsMouseOver();
                if(isFocused != IsFocused)
                {
                    IsFocused = isFocused;
                    FocusChanged?.Invoke(this, new FocusChangedEventArgs(IsFocused));
                }

                if(IsFocused && Font.HitTest(Mouse.Position - Position, Text, ActualWidth, out int i))
                { 
                        _cursorPos = i;
                }
            }

            if (!IsFocused) return;

            //Toggle cursor every half second
            _cursorTick += GameTime.UnscaledDeltaTime;
            if(_cursorTick >= .5f)
            {
                _showCursor = !_showCursor;
                _cursorTick = 0;
            }

            //Cursor navigation
            //Always show cursor when navigating
            if (_cursorPos > 0 && Keyboard.KeyPressed(Keys.Left))
            {
                _cursorPos--;
                _cursorTick = 0;
                _showCursor = true;
                HandleInputEvent(Keys.Left);
            }
            else if (_cursorPos < _text.Length && Keyboard.KeyPressed(Keys.Right))
            {
                _cursorPos++;
                _cursorTick = 0;
                _showCursor = true;
                HandleInputEvent(Keys.Right);
            }

            if(Keyboard.KeyPressed(Keys.Enter))
            {
                Enter?.Invoke(this, new EventArgs());
                HandleInputEvent(Keys.Enter);
            }

            //Text deletion
            if (Keyboard.KeyPressed(Keys.Backspace) && _cursorPos > 0)
            {
                _text = _text.Remove(_cursorPos - 1, 1);
                _cursorPos--;
                HandleInputEvent(Keys.Backspace);
            }
            if (Keyboard.KeyPressed(Keys.Delete) && _cursorPos < _text.Length)
            {
                _text = _text.Remove(_cursorPos, 1);
                HandleInputEvent(Keys.Delete);
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
                    HandleInputEvent(k);
                }
            }
        }

        public override Size MeasureOverride(Size pSize)
        {
            var s = Font.MeasureString(Text, pSize.Width);
            return new Size(pSize.Width, s.Height);
        }

        public override void Dispose()
        {
            Background.Dispose();
            Cursor.Dispose();
            Font.Dispose();
            base.Dispose();
        }
    }
}
