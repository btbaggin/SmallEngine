using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public class CollisionEventArgs : EventArgs
    {
        public ColliderComponent Collider { get; private set; }

        public Manifold Collision { get; private set; }

        public CollisionEventArgs(ColliderComponent pCollider, Manifold pCollision)
        {
            Collider = pCollider;
            Collision = pCollision;
        }

        public bool IsSource()
        {
            return Collision.BodyA == Collider;
        }
    }

    public class CollisionBeginEventArgs : EventArgs
    {
        public ColliderComponent Collider { get; private set; }

        public Manifold Collision { get; private set; }

        public bool Cancel { get; set; }

        public CollisionBeginEventArgs(ColliderComponent pCollider, Manifold pCollision)
        {
            Collider = pCollider;
            Collision = pCollision;
        }
    }
}
