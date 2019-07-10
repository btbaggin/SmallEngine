using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.Physics
{
    public static class PhysicsHelper
    {
        public static Rectangle WorldBounds { get; set; } = new Rectangle(0, 0, Int16.MaxValue, Int16.MaxValue);

        public static Vector2 Gravity { get; set; } = new Vector2(0, 250);

        public static ColliderComponent HitTest(Vector2 pPoint)
        {
            return _physics.HitTest(pPoint);
        }

        #region Internal methods
        static PhysicsSystem _physics;
        internal static void Create()
        {
            _physics = new PhysicsSystem();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static void Update()
        {
            _physics.Process();
        }

        internal static void CreateQuadTree()
        {
            _physics.CreateQuadTree();
        }
        #endregion
    }
}
