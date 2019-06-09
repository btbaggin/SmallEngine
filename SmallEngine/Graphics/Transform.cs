using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace SmallEngine.Graphics
{
    public class Transform
    {
        public static Transform Identity { get; } = new Transform(SharpDX.Matrix3x2.Identity);

        public SharpDX.Matrix3x2 Matrix { get; private set; }

        public Transform(SharpDX.Matrix3x2 pMatrix)
        {
            Matrix = pMatrix;
        }

        public static Transform Create(IGameObject pGameObject)
        {
            var center = new Vector2(pGameObject.Position.X + pGameObject.Scale.Width / 2, pGameObject.Position.Y + pGameObject.Scale.Height / 2);
            SharpDX.Matrix3x2.Rotation(pGameObject.Rotation, new SharpDX.Vector2(center.X, center.Y), out SharpDX.Matrix3x2 m);
            m *= pGameObject.TransformMatrix;
            return new Transform(m);
        }

        public static Transform CreateBasic(IGameObject pGameObject)
        {
            var center = new Vector2(pGameObject.Position.X + pGameObject.Scale.Width / 2, pGameObject.Position.Y + pGameObject.Scale.Height / 2);
            SharpDX.Matrix3x2.Rotation(pGameObject.Rotation, new SharpDX.Vector2(center.X, center.Y), out SharpDX.Matrix3x2 m);
            return new Transform(m);
        }
    }
}
