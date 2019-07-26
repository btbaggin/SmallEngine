using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    [Serializable]
    public class Material
    {
        public float Restitution { get; set; }
        public float Density { get; set; }
        public float StaticFriction { get; set; }
        public float DynamicFriction { get; set; }

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
