using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public static class PhysicsHelper
    {
        public static Rectangle WorldBounds { get; set; } = new Rectangle(0, 0, Int16.MaxValue, Int16.MaxValue);

        public static Vector2 Gravity { get; set; }

        public static RigidBodyComponent HitTest(Vector2 pPoint)
        {
            return _physics.HitTest(pPoint);
        }

        #region Internal methods
        static PhysicsSystem _physics;
        internal static void Create()
        {
            _physics = new PhysicsSystem();
        }

        internal static void Update(float pDeltaTime)
        {
            _physics.Update(pDeltaTime);
        }

        internal static void CreateQuadTree()
        {
            _physics.CreateQuadTree();
        }
        #endregion
    }
}
