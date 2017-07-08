using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolusim.UI
{
    class ToggleButtonGroup
    {
        List<ToggleButton> _buttons;

        public ToggleButtonGroup(params ToggleButton[] pButtons)
        {
            _buttons = new List<ToggleButton>();
        }

        internal void AddToGroup(ToggleButton pButton)
        {
            _buttons.Add(pButton);
        }

        public void SetAllOff()
        {
            foreach(ToggleButton b in _buttons)
            {
                b.IsSelected = false;
            }
        }
    }
}
