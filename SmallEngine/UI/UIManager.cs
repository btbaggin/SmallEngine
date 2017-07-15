﻿using System;
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
        private static List<UIElement> _elements;

        static UIManager()
        {
            _elements = new List<UI.UIElement>();
        }

        public static void Register(UIElement pElement)
        {
            _elements.Add(pElement);
        }

        public static void Unregister(UIElement pElement)
        {
            _elements.Remove(pElement);
        }

        internal void Draw(IGraphicsSystem pSystem)
        {
            foreach (var e in _elements)
            {
                if (e.Visible)
                {
                    e.Draw(pSystem);
                }
            }
        }

        internal void Update(float pDeltaTime)
        {
            foreach (var e in _elements)
            {
                if (e.Visible && e.Enabled)
                {
                    e.Update(pDeltaTime);
                }
            }
        }
    }
}