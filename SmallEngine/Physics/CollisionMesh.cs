using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.Physics
{
    public enum Shapes
    {
        Circle,
        Polygon
    }

    public abstract class CollisionMesh
    {
        internal ColliderComponent Body { get; set; }

        public Shapes Shape { get; protected set; }

        public Material Material { get; set; }

        protected CollisionMesh(Shapes pShape, Material pMaterial)
        {
            Shape = pShape;
            Material = pMaterial;
        }

        public abstract AxisAlignedBoundingBox CalculateAABB(Vector2 pPosition);

        public abstract void CalculateMass(out float pMass, out float pInertia);

        public abstract bool Contains(Vector2 pPoint);
    }

    #region CircleMesh
    public class CircleMesh : CollisionMesh
    {
        public float Radius { get; set; }
        public CircleMesh(float pRadius, Material pMaterial) : base(Shapes.Circle, pMaterial)
        {
            Radius = pRadius;
        }

        public override AxisAlignedBoundingBox CalculateAABB(Vector2 pPosition)
        {
            return new AxisAlignedBoundingBox(pPosition, pPosition + Radius * 2);
        }

        public override void CalculateMass(out float pMass, out float pInertia)
        {
            pMass = Material.Density * MathF.PI * Radius * Radius;
            pInertia = pMass * Radius * Radius;
        }

        public override bool Contains(Vector2 pPoint)
        {
            return Vector2.DistanceSqrd(pPoint, Vector2.Zero) < Radius * Radius;
        }
    }
    #endregion

    #region SquareMesh
    public class SquareMesh : PolygonMesh
    {
        public SquareMesh(Size pSize, Material pMaterial) : this(Vector2.Zero, pSize, pMaterial) { }

        public SquareMesh(Vector2 pPosition, Size pSize, Material pMaterial) : base(new Vector2[] { pPosition,
                                                                                 new Vector2(pSize.Width, 0) + pPosition,
                                                                                 new Vector2(pSize.Width, pSize.Height) + pPosition,
                                                                                 new Vector2(0, pSize.Height) + pPosition}, pMaterial)
        {
        }
    }
    #endregion

    #region PolygonMesh
    public class PolygonMesh : CollisionMesh
    {
        public Vector2[] Verticies { get; private set; }
        public Vector2[] Normals { get; private set; }

        Vector2 _min, _max;
        public PolygonMesh(Vector2[] pVerticies, Material pMaterial) : base(Shapes.Polygon, pMaterial)
        {
            _min = new Vector2(float.MaxValue, float.MaxValue);
            _max = new Vector2(-float.MaxValue, -float.MaxValue);
            SetVerticies(pVerticies);
        }

        private void SetVerticies(Vector2[] pVerticies)
        {
            int actualVertexCount = 0;
            int vertexCount = pVerticies.Length;
            System.Diagnostics.Debug.Assert(vertexCount > 2);

            //Find the right most point on the hull
            int rightMost = 0;
            float highestX = pVerticies[0].X;
            for(int i = 1; i < vertexCount; i++)
            {
                float x = pVerticies[i].X;
                if(x > highestX)
                {
                    highestX = x;
                    rightMost = i;
                }
                else if(x == highestX && pVerticies[i].Y < pVerticies[rightMost].Y)
                {
                    rightMost = i;
                }
            }

            int[] hull = new int[64];
            int outCount = 0;
            int indexHull = rightMost;

            while(true)
            {
                hull[outCount] = indexHull;

                // Search for next index that wraps around the hull
                // by computing cross products to find the most counter-clockwise
                // vertex in the set, given the previous hull index
                int nextHullIndex = 0;
                for(int i = 1; i < vertexCount; i++)
                {
                    // Skip if same coordinate as we need three unique
                    // points in the set to perform a cross product
                    if (nextHullIndex == indexHull)
                    {
                        nextHullIndex = i;
                        continue;
                    }

                    // Cross every set of three unique verticies
                    // Record each counter clockwise third vertex and add
                    // to the output hull
                    Vector2 e1 = pVerticies[nextHullIndex] - pVerticies[hull[outCount]];
                    Vector2 e2 = pVerticies[i] - pVerticies[hull[outCount]];
                    float c = Vector2.CrossProduct(e1, e2);
                    if (c < 0) nextHullIndex = i;

                    // Cross product is zero then e vectors are on same line
                    // therefor want to record vertex farthest along that line
                    if (c == 0 && e2.LengthSqrd > e1.LengthSqrd) nextHullIndex = i;
                }

                outCount++;
                indexHull = nextHullIndex;

                if(nextHullIndex == rightMost)
                {
                    actualVertexCount = outCount;
                    break;
                }
            }

            // Copy verticies into shape's verticies
            Verticies = new Vector2[actualVertexCount];
            for(int i = 0; i < actualVertexCount; i++)
            {
                Verticies[i] = pVerticies[hull[i]];
                var v = Verticies[i];
                if (v.X < _min.X) _min.X = v.X;
                if (v.Y < _min.Y) _min.Y = v.Y;
                if (v.X > _max.X) _max.X = v.X;
                if (v.Y > _max.Y) _max.Y = v.Y;
            }

            // Compute face normals
            Normals = new Vector2[actualVertexCount];
            for(int i = 0; i < actualVertexCount; i++)
            {
                int i2 = i + 1 < actualVertexCount ? i + 1 : 0;
                Vector2 face = Verticies[i2] - Verticies[i];

                System.Diagnostics.Debug.Assert(face.LengthSqrd > .0005f);

                Normals[i] = Vector2.Normalize(new Vector2(face.Y, -face.X));
            }
        }

        public Vector2 GetSupport(Vector2 pDirection)
        {
            float best = float.MinValue;
            Vector2 bestVert = default;

            for (int i = 0; i < Verticies.Length; i++)
            {
                Vector2 v = Verticies[i];
                float proj = Vector2.DotProduct(v, pDirection);

                if(proj > best)
                {
                    bestVert = v;
                    best = proj;
                }
            }

            return bestVert;
        }

        public override AxisAlignedBoundingBox CalculateAABB(Vector2 pPosition)
        {
            return new AxisAlignedBoundingBox(pPosition + _min, pPosition + _max);
        }

        public override bool Contains(Vector2 pPoint)
        {
            bool result = false;
            var j = Verticies.Length - 1;
            for(int i = 0; i < Verticies.Length; i++)
            {
                //Count number of sides that intersect with Y coordinate
                if(Verticies[i].Y < pPoint.Y && Verticies[j].Y >= pPoint.Y || Verticies[j].Y < pPoint.Y && Verticies[i].Y >= pPoint.Y)
                {
                    //Check if they are polygon is to left of point
                    if(Verticies[i].X + (pPoint.Y - Verticies[i].Y) / (Verticies[j].Y - Verticies[i].Y) * (Verticies[j].X - Verticies[i].X) < pPoint.X)
                    {
                        //An odd amount means point is inside polygon
                        result = !result;
                    }
                }
                j = i;
            }

            return result;
        }

        public override void CalculateMass(out float pMass, out float pInertia)
        {
            Vector2 centroid = new Vector2();
            float area = 0.0f;
            float I = 0.0f;
            const float k_inv3 = 1.0f / 3.0f;

            for (int i1 = 0; i1 < Verticies.Length; ++i1)
            {
                // Triangle vertices, third vertex implied as (0, 0)
                Vector2 p1 = Verticies[i1];
                int i2 = i1 + 1 < Verticies.Length ? i1 + 1 : 0;
                Vector2 p2 = Verticies[i2];

                float D = Vector2.CrossProduct(p1, p2);
                float triangleArea = 0.5f * D;

                area += triangleArea;

                // Use area to weight the centroid average, not just vertex position
                centroid += triangleArea * k_inv3 * (p1 + p2);

                float intx2 = p1.X * p1.X + p2.X * p1.X + p2.X * p2.X;
                float inty2 = p1.Y * p1.Y + p2.Y * p1.Y + p2.Y * p2.Y;
                I += (0.25f * k_inv3 * D) * (intx2 + inty2);
            }

            centroid *= 1.0f / area;

            // Translate verticies to centroid (make the centroid (0, 0)
            // for the polygon in model space)
            // Not really necessary, but I like doing this anyway
            for (int i = 0; i < Verticies.Length; ++i)
                Verticies[i] -= centroid;

            pMass = Material.Density * area;
            pInertia = I * Material.Density;
        }
    }
    #endregion
}
