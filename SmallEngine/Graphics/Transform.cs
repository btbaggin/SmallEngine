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
        public static Transform Identity { get; } = new Transform(Matrix3x2.Identity);

        public Matrix3x2 Rotation { get; private set; }

        public Transform(Matrix3x2 pRotation)
        {
            Rotation = pRotation;
        }

        public static Transform Create(IGameObject pGameObject)
        {
            var center = new Vector2(pGameObject.Position.X + pGameObject.Scale.Width / 2, pGameObject.Position.Y + pGameObject.Scale.Height / 2);
            Matrix3x2.Rotation(pGameObject.RotationMatrix.Rotation, new SharpDX.Vector2(center.X, center.Y), out Matrix3x2 m);
            return new Transform(m);
        }
    }
}
