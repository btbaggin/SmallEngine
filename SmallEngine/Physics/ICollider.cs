using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SmallEngine.Physics
{
    public enum Shape
    {
        Square,
        Circle
    }

    public interface ICollider
    {
        Shape ColliderShape { get; }
        RectangleF Bounds { get; set; }

        bool IsColliding(ICollider pCollider);
        void Resolve(ICollider pCollider);
    }
}
