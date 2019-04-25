using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public interface IPhysicsBody
    {
        AxisAlignedBoundingBox AABB { get; }
    }
}
