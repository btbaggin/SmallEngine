using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class UIManager
    {
        readonly List<UIElement> _elements = new List<UIElement>();
        readonly Dictionary<string, UIElement> _namedElements = new Dictionary<string, UIElement>();
        bool _measureInvalid = true;

        public static string DefaultFontFamily { get; set; } = "Arial";
        public static int DefaultFontSize { get; set; } = 14;
        public static Color DefaultFontColor { get; set; } = Color.Black;

        public void Register(UIElement pElement)
        {
            _elements.AddOrdered(pElement);
        }

        internal void AddNamedElement(UIElement pElement)
        {
            System.Diagnostics.Debug.Assert(pElement.Name != null);

            _namedElements.Add(pElement.Name, pElement);
        }

        public UIElement GetElement(string pName)
        {
            if(_namedElements.ContainsKey(pName))
            {
                return _namedElements[pName];
            }

            return null;
        }

        #region Internal Methods
        internal void UpdateAndDraw(IGraphicsAdapter pSystem)
        {
            //We update and draw at the same time because GameObjects will often manipulate UI elements
            //So if we arrange before those updates can happen it can cause a frame of misplaced items
            var bounds = new Rectangle(0, 0, Game.Form.Width, Game.Form.Height);
            var size = new Size(Game.Form.Width, Game.Form.Height);
            pSystem.ResetTransform();

            foreach (var e in _elements)
            {
                if (_measureInvalid) e.Measure(size);
                e.Arrange(bounds);
                e.UpdateInternal();

                if (e.Visible == Visibility.Visible)
                {
                    e.DrawInternal(pSystem);
                }
            }
            _measureInvalid = false;
        }

        internal void InvalidateMeasure()
        {
            _measureInvalid = true;
        }

        internal void DisposeElements()
        {
            foreach(var c in _elements)
            {
                c.Dispose();
            }
        }
        #endregion
    }
}