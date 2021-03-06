﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.UI;
using SmallEngine.Input;

namespace Evolusim.UI
{
    class ToggleButtonGroup : UIElement
    {
        public ToggleButtonGroup(params ToggleButton[] pButtons)
        {
            Height = 50;
            WidthPercent = 1f;
            foreach(var b in pButtons)
            {
                AddChild(b, AnchorDirection.Left, Vector2.Zero);
            }
            Orientation = ElementOrientation.Horizontal;
            SetLayout();
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);
            foreach(var c in Children)
            {
                if (InputManager.KeyPressed(Mouse.Left) && InputManager.IsFocused(c))
                {
                    SetAllOff();
                    ((ToggleButton)c).IsSelected = true;
                }
            }
        }

        public void SetAllOff()
        {
            foreach(UIElement child in Children)
            {
                if(child is ToggleButton b) b.IsSelected = false;
            }
        }

        public T GetSelectedData<T>()
        {
            foreach(var b in Children)
            {
                if(((ToggleButton)b).IsSelected)
                {
                    return (T)((ToggleButton)b).Data;
                }
            }

            return default(T);
        }
    }
}
