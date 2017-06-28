using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public class Material
    {
        public Material() { }

        public float Restitution { get; set; } = 0.0f;

        public float StaticFriction { get; set; } = 0.6f;

        public float KineticFriction { get; set; } = 0.3f;
    }
}
