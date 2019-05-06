using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public static class PhysicsParameters
    {
        public static Rectangle WorldBounds { get; set; } = new Rectangle(0, 0, Int16.MaxValue, Int16.MaxValue);

        public static Vector2 Gravity { get; set; }
    }
}
