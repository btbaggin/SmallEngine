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
        static List<UIElement> _elements = new List<UIElement>();
        static Dictionary<string, UIElement> _namedElements = new Dictionary<string, UIElement>();
        static bool _measureInvalid = true;

        public static string DefaultFontFamily { get; set; } = "Arial";
        public static int DefaultFontSize { get; set; } = 14;
        public static Color DefaultFontColor { get; set; } = Color.Black;

        public static void Register(UIElement pElement)
        {
            _elements.AddOrdered(pElement);
        }

        internal static void AddNamedElement(UIElement pElement)
        {
            System.Diagnostics.Debug.Assert(pElement.Name != null);

            _namedElements.Add(pElement.Name, pElement);
        }

        public static void Unregister(UIElement pElement)
        {
            //TODO what to do with this?
            _elements.Remove(pElement);
        }

        public static UIElement GetElement(string pName)
        {
            if(_namedElements.ContainsKey(pName))
            {
                return _namedElements[pName];
            }

            return null;
        }

        public static T GetElement<T>(string pName) where T : UIElement
        {
            if(_namedElements.ContainsKey(pName))
            {
                return (T)_namedElements[pName];
            }

            return null;
        }

        #region Internal Methods
        internal void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.ResetTransform();
            foreach (var e in _elements)
            {
                if (e.Visible == Visibility.Visible)
                {
                    e.DrawInternal(pSystem);
                }
            }
        }

        internal void Update(float pDeltaTime)
        {
            var bounds = new Rectangle(0, 0, Game.Form.Width, Game.Form.Height);
            var size = new Size(Game.Form.Width, Game.Form.Height);
            foreach (var e in _elements)
            {
                if(_measureInvalid) e.Measure(size);
                e.Arrange(bounds);
                e.UpdateInternal(pDeltaTime);
            }
            _measureInvalid = false;
        }

        internal static void InvalidateMeasure()
        {
            _measureInvalid = true;
        }
        #endregion
    }
}