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
    }
}
