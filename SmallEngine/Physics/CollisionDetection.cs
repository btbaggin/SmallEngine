using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    static class CollisionDetection
    {
        //https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
        //https://github.com/tutsplus/ImpulseEngine/blob/master/Collision.cpp

        public static bool CircleVsCircle(ref Manifold pM)
        {
            ColliderComponent A = pM.BodyA;
            ColliderComponent B = pM.BodyB;

            Vector2 n = B.AABB.Center - A.AABB.Center;
            float aRadius = ((CircleMesh)A.Mesh).Radius;
            float bRadius = ((CircleMesh)B.Mesh).Radius;

            float radius = aRadius + bRadius;

            if (n.LengthSqrd > radius * radius) return false;

            float distance = n.Length;
            if(distance != 0)
            {
                pM.Penetration = radius - distance;
                pM.Normal = n / distance;
                pM.Contacts = new Vector2[] { pM.Normal * aRadius + A.AABB.Center };
            }
            else
            {
                pM.Penetration = aRadius;
                pM.Normal = Vector2.UnitX;
                pM.Contacts = new Vector2[] { A.Position };
            }

            return true;
        }

        public static bool CirclevsPolygon(ref Manifold pM)
        {
            ColliderComponent A = pM.BodyA;
            ColliderComponent B = pM.BodyB;
            CircleMesh c = (CircleMesh)A.Mesh;
            PolygonMesh p = (PolygonMesh)B.Mesh;

            var bMat = B.GameObject.RotationMatrix;
            Vector2 center = A.AABB.Center;
            center = bMat.Transpose() * (center - B.AABB.Center);

            float maxSeparation = float.MinValue;
            int faceNormal = 0;
            for(int i = 0; i < p.Vertices.Length; i++)
            {
                float sep = Vector2.DotProduct(p.Normals[i], center - p.Vertices[i]);
                if (sep > c.Radius) return false;

                if(sep > maxSeparation)
                {
                    maxSeparation = sep;
                    faceNormal = i;
                }
            }

            Vector2 v1 = p.Vertices[faceNormal];
            int nextFace = faceNormal + 1 < p.Vertices.Length ? faceNormal + 1 : 0;
            Vector2 v2 = p.Vertices[nextFace];

            if (maxSeparation < .00005f)
            {
                pM.Normal = -(bMat * p.Normals[faceNormal]);
                pM.Contacts = new Vector2[] { pM.Normal * c.Radius + A.AABB.Center };
                pM.Penetration = c.Radius;
                return true;
            }

            float dot1 = Vector2.DotProduct(center - v1, v2 - v1);
            float dot2 = Vector2.DotProduct(center - v2, v1 - v2);
            pM.Penetration = c.Radius - maxSeparation;

            if(dot1 <= 0)
            {
                if (Vector2.DistanceSqrd(center, v1) > c.Radius * c.Radius) return false; 

                Vector2 n = v1 - center;
                n = bMat * n;
                n.Normalize();
                pM.Normal = n;

                v1 = bMat * v1 + B.AABB.Center;
                pM.Contacts = new Vector2[] { v1 };
            }
            else if(dot2 <= 0)
            {
                if (Vector2.DistanceSqrd(center, v2) > c.Radius * c.Radius) return false;

                Vector2 n = v2 - center;
                n = bMat * n;
                n.Normalize();
                pM.Normal = n;

                v2 = bMat * v2 + B.AABB.Center;
                pM.Contacts = new Vector2[] { v2 };
            }
            else
            {
                Vector2 n = p.Normals[faceNormal];
                if (Vector2.DotProduct(center - v1, n) > c.Radius) return false;

                n = bMat * n;
                pM.Normal = -n;
                pM.Contacts = new Vector2[] { pM.Normal * c.Radius + A.AABB.Center };
            }

            return true;
        }

        public static bool PolygonvsCircle(ref Manifold pM)
        {
            pM = new Manifold(pM.BodyB, pM.BodyA);
            return CirclevsPolygon(ref pM);
        }

        public static bool PolygonvsPolygon(ref Manifold pM)
        {
            PolygonMesh A = (PolygonMesh)pM.BodyA.Mesh;
            PolygonMesh B = (PolygonMesh)pM.BodyB.Mesh;

            float penetrationA = FindAxisLeastPenetration(out int faceA, A, B);
            if (penetrationA >= 0) return false;

            float penetrationB = FindAxisLeastPenetration(out int faceB, B, A);
            if (penetrationB >= 0) return false;

            int referenceIndex;
            bool flip;

            PolygonMesh reference;
            PolygonMesh incident;

            if(penetrationA >= penetrationB * .95f + penetrationA * .01f)
            {
                reference = A;
                incident = B;
                referenceIndex = faceA;
                flip = false;
            }
            else
            {
                reference = B;
                incident = A;
                referenceIndex = faceB;
                flip = true;
            }

            FindIncidentFace(out Vector2[] incidentFace, reference, incident, referenceIndex);

            Vector2 v1 = reference.Vertices[referenceIndex];
            referenceIndex = referenceIndex + 1 == reference.Vertices.Length ? 0 : referenceIndex + 1;
            Vector2 v2 = reference.Vertices[referenceIndex];

            var rMat = reference.Body.GameObject.RotationMatrix;
            v1 = rMat * v1 + reference.Body.AABB.Center;
            v2 = rMat * v2 + reference.Body.AABB.Center;

            Vector2 sidePlaneNormal = v2 - v1;
            sidePlaneNormal.Normalize();

            Vector2 refFaceNormal = new Vector2(sidePlaneNormal.Y, -sidePlaneNormal.X);

            float refC = Vector2.DotProduct(refFaceNormal, v1);
            float negSide = -Vector2.DotProduct(sidePlaneNormal, v1);
            float posSide = Vector2.DotProduct(sidePlaneNormal, v2);

            if (Clip(-sidePlaneNormal, negSide, ref incidentFace) < 2) return false;
            if (Clip(sidePlaneNormal, posSide, ref incidentFace) < 2) return false;

            pM.Normal = flip ? -refFaceNormal : refFaceNormal;

            float separation = Vector2.DotProduct(refFaceNormal, incidentFace[0]) - refC;
            List<Vector2> contacts = new List<Vector2>();
            if (separation <= 0)
            {
                contacts.Add(incidentFace[0]);
                pM.Penetration = -separation;
            }

            separation = Vector2.DotProduct(refFaceNormal, incidentFace[1]) - refC;
            if (separation <= 0)
            {
                contacts.Add(incidentFace[1]);
                pM.Penetration += -separation;

                pM.Penetration /= contacts.Count;
            }

            pM.Contacts = contacts.ToArray();
            return pM.Contacts.Length > 0;
        }
        
        private static float FindAxisLeastPenetration(out int pIndex, PolygonMesh pA, PolygonMesh pB)
        {
            float best = float.MinValue;
            pIndex = 0;

            var aMat = pA.Body.GameObject.RotationMatrix;
            var bMat = pB.Body.GameObject.RotationMatrix;
            for(int i = 0; i < pA.Vertices.Length; i++)
            {
                Vector2 normal = pA.Normals[i];
                Vector2 orientedNormal = aMat * normal;

                Mathematics.Matrix2X2 buT = bMat.Transpose();
                normal = buT * orientedNormal;

                Vector2 support = pB.GetSupport(-normal);

                Vector2 vertex = pA.Vertices[i];
                vertex = aMat * vertex + pA.Body.AABB.Center;
                vertex -= pB.Body.AABB.Center;
                vertex = buT * vertex;

                float d = Vector2.DotProduct(normal, support - vertex);

                if(d > best)
                {
                    best = d;
                    pIndex = i;
                }
            }

            return best;
        }

        private static void FindIncidentFace(out Vector2[] pV, PolygonMesh pRef, PolygonMesh pInc, int pIndex)
        {
            Vector2 referenceNormal = pRef.Normals[pIndex];

            var iMat = pInc.Body.GameObject.RotationMatrix;
            referenceNormal = pRef.Body.GameObject.RotationMatrix * referenceNormal;
            referenceNormal = iMat.Transpose() * referenceNormal;

            int incidentFace = 0;
            float min = float.MaxValue;
            for(int i = 0; i < pInc.Vertices.Length; i++)
            {
                float dot = Vector2.DotProduct(referenceNormal, pInc.Normals[i]);
                if(dot < min)
                {
                    min = dot;
                    incidentFace = i;
                }
            }

            pV = new Vector2[2];
            pV[0] = iMat * pInc.Vertices[incidentFace] + pInc.Body.AABB.Center;

            incidentFace = incidentFace + 1 >= pInc.Vertices.Length ? 0 : incidentFace + 1;
            pV[1] = iMat * pInc.Vertices[incidentFace] + pInc.Body.AABB.Center;
        }

        private static int Clip(Vector2 n, float c, ref Vector2[] pFace)
        {
            int sp = 0;
            Vector2[] outV = new Vector2[] { pFace[0], pFace[1] };

            float d1 = Vector2.DotProduct(n, pFace[0]) - c;
            float d2 = Vector2.DotProduct(n, pFace[1]) - c;

            if (d1 <= 0) outV[sp++] = pFace[0];
            if (d2 <= 0) outV[sp++] = pFace[1];

            if(d1 * d2 < 0)
            {
                float alpha = d1 / (d1 - d2);
                outV[sp] = pFace[0] + alpha * (pFace[1] - pFace[0]);
                sp++;
            }

            pFace = outV;

            System.Diagnostics.Debug.Assert(sp != 3);
            return sp;
        }
    }
}
