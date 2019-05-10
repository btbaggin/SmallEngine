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

        private Material(float pRestitution, float pDensity, float pStaticFriction, float pDynamicFriction)
        {
            Restitution = pRestitution;
            Density = pDensity;
            StaticFriction = pStaticFriction;
            DynamicFriction = pDynamicFriction;
        }

        public static Material Create(float pRestitution, float pDensity, float pStaticFriction, float pDynamicFriction)
        {
            return new Material(pRestitution, pDensity, pStaticFriction, pDynamicFriction);
        }

        public static Material CreateImmobile(float pRestitution, float pStaticFriction, float pDynamicFriction)
        {
            return new Material(pRestitution, 0f, pStaticFriction, pDynamicFriction);
        }
    }
}
