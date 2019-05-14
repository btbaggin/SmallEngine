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
        public Color Background { get; set; }

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

        StringBuilder _text = new StringBuilder();
        int _cursorPos;
        public Textbox()
        {
            Font = Game.Graphics.CreateFont("Arial", 14, Color.White);//TODO default
            Font.Alignment = Alignments.Center;
            Background = Color.Gray;
            AllowFocus = false;
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawFillRect(Bounds, pSystem.CreateBrush(Background));
            pSystem.DrawText(Text, Bounds, Font); //TODO clip and navigation around cursor
        }

        public override void Update(float pDeltaTime)
        {
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
            if(_cursorPos > 0 && Keyboard.KeyPressed(Keys.Left)) _cursorPos--;
            else if (_cursorPos < _text.Length && Keyboard.KeyPressed(Keys.Right)) _cursorPos++;

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

        public override System.Drawing.Size MeasureOverride(System.Drawing.Size pSize)
        {
            var s = Font.MeasureString(Text, pSize.Width);
            return new System.Drawing.Size(pSize.Width, (int)s.Height);
        }
    }
}
