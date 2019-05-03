using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public class CollisionEventArgs : EventArgs
    {
        public bool Source { get; private set; }

        public RigidBodyComponent Collider { get; private set; }

        public CollisionEventArgs(RigidBodyComponent pCollider, bool pSource)
        {
            Collider = pCollider;
            Source = pSource;
        }
    }
}
