using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public class Material
    {
        public float Restitution { get; private set; }

        public float Density { get; private set; }

        public float StaticFriction { get; private set; }

        public float DynamicFriction { get; private set; }

        public Material(float pRestitution, float pDensity, float pStaticFriction, float pDynamicFriction)
        {
            System.Diagnostics.Debug.Assert(pStaticFriction >= pDynamicFriction, "Static friction must be greater than dynamic friction");
            Restitution = pRestitution;
            Density = pDensity;
            StaticFriction = pStaticFriction;
            DynamicFriction = pDynamicFriction;
        }
    }
}
