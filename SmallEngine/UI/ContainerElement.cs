using SmallEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.UI
{
    public abstract class ContainerElement : UIElement
    {
        public Brush Background { get; set; }

        public HorizontalAlignments HorizontalContentAlignment { get; set; }

        public VerticalAlignments VerticalContentAlignment { get; set; }

        protected ContainerElement(string pName) : base(pName) { }

        public void AddElement(UIElement pElement)
        {
            using (var tb = Debug.TimedBlock.Start())
            {
                AddChildElements(pElement, ContainingScene);
                base.AddChild(pElement);
                if (AddedToLayout) ContainingScene.InvalidateUI();
            }
        }

        public void InsertElement(int pIndex, UIElement pElement)
        {
            using (var tb = Debug.TimedBlock.Start())
            {
                AddChildElements(pElement, ContainingScene);
                base.InsertChild(pIndex, pElement);
                if (AddedToLayout) ContainingScene.InvalidateUI();
            }
        }

        private void AddChildElements(UIElement pElement, Scene pScene)
        {
            pElement.ContainingScene = pScene;
            foreach (var c in pElement.Children)
            {
                AddChildElements(c, pScene);
            }
        }

        public void ClearElements()
        {
            foreach (var c in Children)
            {
                ContainingScene.RemoveUIElement(c);
                c.Dispose();
            }
            Children.Clear();
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            if (Background != null) pSystem.DrawRect(Bounds, Background);
        }
    }
}
