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
            Restitution = pRestitution;
            Density = pDensity;
            StaticFriction = pStaticFriction;
            DynamicFriction = pDynamicFriction;
        }
    }
}
