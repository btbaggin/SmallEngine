using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public struct Manifold
    {
        public ColliderComponent BodyA { get; private set; }
        public ColliderComponent BodyB { get; private set; }
        public float Penetration { get; set; }
        public Vector2 Normal { get; set; }

        public Vector2[] Contacts { get; set; }

        public Manifold(ColliderComponent pBodyA, ColliderComponent pBodyB)
        {
            BodyA = pBodyA;
            BodyB = pBodyB;
            Penetration = 0;
            Normal = Vector2.Zero;
            Contacts = null;
        }

        public void Resolve()
        {
            System.Diagnostics.Debug.Assert(Contacts != null && Contacts.Length > 0);
            var rBodyA = BodyA.Body;
            var rBodyB = BodyB.Body;
            if (rBodyA == null || rBodyB == null) return;

            //Check for static objects
            if (rBodyA.Mass + rBodyB.Mass == 0)
            {
                rBodyA.Velocity = Vector2.Zero;
                rBodyB.Velocity = Vector2.Zero;
                return;
            }

            var contactCount = Contacts.Length;
             for(int i = 0; i < contactCount; i++)
            {
                //Get relative velocity between point of contacts of two objects
                Vector2 ra = Contacts[i] - BodyA.AABB.Center;
                Vector2 rb = Contacts[i] - BodyB.AABB.Center;

                Vector2 relativeVelocity = rBodyB.Velocity + Vector2.CrossProduct(rBodyB.AngularVelocity, rb) - 
                                           rBodyA.Velocity - Vector2.CrossProduct(rBodyA.AngularVelocity, ra);

                float contactVel = Vector2.DotProduct(relativeVelocity, Normal);
                if (contactVel > 0) return;

                float raCrossN = Vector2.CrossProduct(ra, Normal);
                float rbCrossN = Vector2.CrossProduct(rb, Normal);
                float invMassSum = rBodyA.InverseMass + rBodyB.InverseMass + (raCrossN * raCrossN) * 
                                   rBodyA.InverseInertia + (rbCrossN * rbCrossN) * rBodyB.InverseInertia;

                //Calculate impulse magnitude
                var e = Math.Min(BodyA.Mesh.Material.Restitution, BodyB.Mesh.Material.Restitution);
                float j = -(1 + e) * contactVel;
                j /= invMassSum;
                j /= contactCount;

                Vector2 impulse = Normal * j;
                rBodyA.ApplyImpulse(-impulse, ra);
                rBodyB.ApplyImpulse(impulse, rb);

                //Friction
                relativeVelocity = rBodyB.Velocity + Vector2.CrossProduct(rBodyB.AngularVelocity, rb) - 
                                   rBodyA.Velocity - Vector2.CrossProduct(rBodyA.AngularVelocity, ra);

                Vector2 t = relativeVelocity - (Normal * Vector2.DotProduct(relativeVelocity, Normal));
                t.Normalize();

                //Calculate friction magnitude
                float jt = -Vector2.DotProduct(relativeVelocity, t);
                jt /= invMassSum;
                jt /= contactCount;

                if (jt == 0) return;

                var sfA = BodyA.Mesh.Material.StaticFriction;
                var sfB = BodyB.Mesh.Material.StaticFriction;
                var staticFriction = Math.Sqrt(sfA * sfA + sfB * sfB);

                //If we aren't moving fast enough, using the static friction, otherwise dynamic
                Vector2 tangent;
                if (Math.Abs(jt) < j * staticFriction)
                    tangent = t * jt;
                else
                {
                    var dfA = BodyA.Mesh.Material.DynamicFriction;
                    var dfB = BodyB.Mesh.Material.DynamicFriction;
                    var dynamicFriction = MathF.Sqrt(dfA * dfA + dfB * dfB);
                    tangent = t * -j * dynamicFriction;
                }

                rBodyA.ApplyImpulse(-tangent, ra);
                rBodyB.ApplyImpulse(tangent, rb);
            }
        }

        public void CorrectPositions()
        {
            RigidBodyComponent A = BodyA.Body;
            RigidBodyComponent B = BodyB.Body;

            if (A == null || B == null) return;
            if (A.Mass + B.Mass == 0) return;

            //Prevent objects from sinking. Allow things to be slightly overlapping
            const float percent = 0.2f;
            const float slop = 0.01f;
            Vector2 correction = Math.Max(Penetration - slop, 0f) / (A.InverseMass + B.InverseMass) * percent * Normal;
            A.MoveBody(-(A.InverseMass * correction));
            B.MoveBody(B.InverseMass * correction);
        }
    }
}
