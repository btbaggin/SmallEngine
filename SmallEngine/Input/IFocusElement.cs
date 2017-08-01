using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Input
{
    public interface IFocusElement
    {
        IFocusElement GetFocusedElement(Vector2 pPoint);
    }
}
