using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.UI
{
    public class FocusChangedEventArgs : EventArgs
    {
        public bool Focused { get; private set; }

        public FocusChangedEventArgs(bool pFocused)
        {
            Focused = pFocused;
        }
    }
}
