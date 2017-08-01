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
        internal static List<UIElement> Elements { get; private set; }

        static UIManager()
        {
            Elements = new List<UI.UIElement>();
        }

        public static void Register(UIElement pElement)
        {
            Elements.Add(pElement);
        }

        public static void Unregister(UIElement pElement)
        {
            Elements.Remove(pElement);
        }

        internal void Draw(IGraphicsSystem pSystem)
        {
            //TODO speed
            foreach (var e in Elements.OrderBy((pE) => pE.Order))
            {
                if (e.Visible)
                {
                    e.Draw(pSystem);
                }
            }
        }

        internal void Update(float pDeltaTime)
        {
            foreach (var e in Elements)
            {
                if (e.Visible && e.Enabled)
                {
                    e.Update(pDeltaTime);
                }
            }
        }

        internal void Resize()
        {
            foreach(var e in Elements)
            {
                e.Measure(new SizeF(Game.Form.Width, Game.Form.Height), 0, 0);
            }
        }
    }
}