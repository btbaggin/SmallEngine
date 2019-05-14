using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using System.Drawing;

namespace SmallEngine.UI
{
    public class UIManager
    {
        internal static List<UIElement> Elements { get; private set; } = new List<UIElement>();
        private static bool _measureInvalid = true;

        public static void Register(UIElement pElement)
        {
            var i = Elements.BinarySearch(pElement);
            if (i == -1) Elements.Add(pElement);
            else Elements.Insert(i, pElement);
        }

        public static void Unregister(UIElement pElement)
        {
            //TODO what to do with this?
            Elements.Remove(pElement);
        }

        public static UIElement GetFocusElement()
        {
            UIElement focus = null;
            foreach(var e in Elements)
            {
                focus = e.GetFocusElement(Input.Mouse.Position);
                if (focus != null) return focus;
            }

            return null;
        }

        internal void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.ResetTransform();
            foreach (var e in Elements)
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
            foreach (var e in Elements)
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
    }
}