using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine
{
    public interface IDrawable
    {
        int Order { get; }
        void Draw(IGraphicsSystem pSystem);
        IDrawable GetFocusElement(Vector2 pPosition);
    }
}
