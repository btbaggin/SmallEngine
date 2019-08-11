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
        enum MeasureStatus
        {
            Invalid,
            PendingInvalid,
            Valid
        }

        readonly List<UIElement> _elements = new List<UIElement>();
        readonly Dictionary<string, UIElement> _namedElements = new Dictionary<string, UIElement>();
        MeasureStatus _measureStatus = MeasureStatus.Invalid;

        public static Font DefaultFont { get; set; } = Font.Create("Arial", 14, Color.White);

        public void Add(UIElement pElement, Scene pScene)
        {
            AddChildElements(pElement, pScene);
            _elements.AddOrdered(pElement);
        }

        public void Remove(UIElement pElement)
        {
            RemoveChildElements(pElement);
            _elements.Remove(pElement);
        }

        private void AddChildElements(UIElement pElement, Scene pScene)
        {
            pElement.ContainingScene = pScene;
            pElement.AddedToLayout = true;
            if(pElement.Name != null) _namedElements.Add(pElement.Name, pElement);
            foreach(var c in pElement.Children)
            {
                AddChildElements(c, pScene);
            }
        }

        private void RemoveChildElements(UIElement pElement)
        {
            if (pElement.Name != null) _namedElements.Remove(pElement.Name);
            pElement.AddedToLayout = false;
            foreach(var c in pElement.Children)
            {
                RemoveChildElements(c);
            }
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
        internal void Update()
        {
            var size = new Size(Game.Form.Width, Game.Form.Height);

            Input.InputState.SwapUIStates();
            foreach (var e in _elements)
            {
                if (_measureStatus == MeasureStatus.Invalid) e.Measure(size);
                e.UpdateInternal();
            }

            if (_measureStatus == MeasureStatus.PendingInvalid) _measureStatus = MeasureStatus.Invalid;
            else _measureStatus = MeasureStatus.Valid;
            Input.InputState.SwapUIStates();
        }

        internal void Draw(IGraphicsAdapter pSystem)
        {
            //We update and draw at the same time because GameObjects will often manipulate UI elements
            //So if we arrange before those updates can happen it can cause a frame of misplaced items
            var bounds = new Rectangle(0, 0, Game.Form.Width, Game.Form.Height);
            pSystem.ResetTransform();

            foreach (var e in _elements)
            {
                e.Arrange(bounds);

                if (e.Visible == Visibility.Visible) //TODO have separate array for visible elements
                {
                    e.DrawInternal(pSystem);
                }
            }
        }

        internal void InvalidateMeasure()
        {
            _measureStatus = MeasureStatus.PendingInvalid;
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