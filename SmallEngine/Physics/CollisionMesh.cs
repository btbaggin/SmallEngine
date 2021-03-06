﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

    [Serializable]
    public abstract class CollisionMesh : ISerializable
    {
        internal ColliderComponent Body { get; set; }

        public Shapes Shape { get; protected set; }

        public Material Material { get; set; }

        public float Mass { get; protected set; }

        public float Inertia { get; protected set; }

        protected CollisionMesh(Shapes pShape, Material pMaterial)
        {
            Shape = pShape;
            Material = pMaterial;
        }

        protected CollisionMesh(SerializationInfo pInfo, StreamingContext pContext)
        {
            Shape = (Shapes)pInfo.GetInt32("Shape");
            Material = (Material)pInfo.GetValue("Material", typeof(Material));
            Mass = pInfo.GetSingle("Mass");
            Inertia = pInfo.GetSingle("Inertia");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Shape", Shape);
            info.AddValue("Material", Material, typeof(Material));
            info.AddValue("Mass", Mass);
            info.AddValue("Inertia", Inertia);
        }

        public abstract AxisAlignedBoundingBox CalculateAABB(Vector2 pPosition);

        public abstract void CalculateMass();

        public abstract bool Contains(Vector2 pPoint);
    }

    #region CircleMesh
    [Serializable]
    public sealed class CircleMesh : CollisionMesh
    {
        float _radius;
        public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                CalculateMass();
            }
        }

        public CircleMesh(float pRadius, Material pMaterial) : base(Shapes.Circle, pMaterial)
        {
            Radius = pRadius;
        }

        private CircleMesh(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
            Radius = pInfo.GetSingle("Radius");
            CalculateMass();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Radius", Radius);
        }

        public override AxisAlignedBoundingBox CalculateAABB(Vector2 pPosition)
        {
            return new AxisAlignedBoundingBox(pPosition, pPosition + Radius * 2);
        }

        public sealed override void CalculateMass()
        {
            Mass = Material.Density * MathF.PI * Radius * Radius;
            Inertia = Mass * Radius * Radius;
        }

        public override bool Contains(Vector2 pPoint)
        {
            return Vector2.DistanceSqrd(pPoint, Vector2.Zero) < Radius * Radius;
        }
    }
    #endregion

    #region SquareMesh
    [Serializable]
    public sealed class SquareMesh : PolygonMesh
    {
        [NonSerialized] Vector2 _offset;
        Size _size;
        public Size Size
        {
            get { return _size; }
            set
            {
                _size = value;
                SetVertices(new Vector2[] { _offset,
                                            new Vector2(_offset.X + value.Width, _offset.Y),
                                            new Vector2(_offset.X + value.Width, _offset.Y + value.Height),
                                            new Vector2(_offset.X, _offset.Y + value.Height) });
                CalculateMass();
            }
        }
        public SquareMesh(Size pSize, Material pMaterial) : this(Vector2.Zero, pSize, pMaterial) { }

        public SquareMesh(Vector2 pOffset, Size pSize, Material pMaterial) : base(new Vector2[] { pOffset,
                                                                                    new Vector2(pSize.Width, 0) + pOffset,
                                                                                    new Vector2(pSize.Width, pSize.Height) + pOffset,
                                                                                    new Vector2(0, pSize.Height) + pOffset}, pMaterial)
        {
            _size = pSize;
            _offset = pOffset;
        }

        private SquareMesh(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }
    }
    #endregion

    #region PolygonMesh
    [Serializable]
    public class PolygonMesh : CollisionMesh
    {
        public Vector2[] Vertices { get; private set; }
        public Vector2[] Normals { get; private set; }

        Vector2 _min, _max;
        public PolygonMesh(Vector2[] pVerticies, Material pMaterial) : base(Shapes.Polygon, pMaterial)
        {
            SetVertices(pVerticies);
            CalculateMass();
        }

        protected PolygonMesh(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
            Vertices = (Vector2[])pInfo.GetValue("Vertices", typeof(Vector2[]));
            Normals = (Vector2[])pInfo.GetValue("Normals", typeof(Vector2[]));
            _min = (Vector2)pInfo.GetValue("Min", typeof(Vector2));
            _max = (Vector2)pInfo.GetValue("Max", typeof(Vector2));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Vertices", Vertices, typeof(Vector2[]));
            info.AddValue("Normals", Normals, typeof(Vector2[]));
            info.AddValue("Min", _min, typeof(Vector2));
            info.AddValue("Max", _max, typeof(Vector2));
        }

        protected void SetVertices(Vector2[] pVerticies)
        {
            _min = new Vector2(float.MaxValue, float.MaxValue);
            _max = new Vector2(-float.MaxValue, -float.MaxValue);

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
            Vertices = new Vector2[actualVertexCount];
            for(int i = 0; i < actualVertexCount; i++)
            {
                Vertices[i] = pVerticies[hull[i]];
                var v = Vertices[i];
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
                Vector2 face = Vertices[i2] - Vertices[i];

                System.Diagnostics.Debug.Assert(face.LengthSqrd > .0005f);

                Normals[i] = Vector2.Normalize(new Vector2(face.Y, -face.X));
            }
        }

        public Vector2 GetSupport(Vector2 pDirection)
        {
            float best = float.MinValue;
            Vector2 bestVert = default;

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vector2 v = Vertices[i];
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
            var j = Vertices.Length - 1;
            for(int i = 0; i < Vertices.Length; i++)
            {
                //Count number of sides that intersect with Y coordinate
                if(Vertices[i].Y < pPoint.Y && Vertices[j].Y >= pPoint.Y || Vertices[j].Y < pPoint.Y && Vertices[i].Y >= pPoint.Y)
                {
                    //Check if they are polygon is to left of point
                    if(Vertices[i].X + (pPoint.Y - Vertices[i].Y) / (Vertices[j].Y - Vertices[i].Y) * (Vertices[j].X - Vertices[i].X) < pPoint.X)
                    {
                        //An odd amount means point is inside polygon
                        result = !result;
                    }
                }
                j = i;
            }

            return result;
        }

        public sealed override void CalculateMass()
        {
            Vector2 centroid = new Vector2();
            float area = 0.0f;
            float I = 0.0f;
            const float k_inv3 = 1.0f / 3.0f;

            for (int i1 = 0; i1 < Vertices.Length; ++i1)
            {
                // Triangle vertices, third vertex implied as (0, 0)
                Vector2 p1 = Vertices[i1];
                int i2 = i1 + 1 < Vertices.Length ? i1 + 1 : 0;
                Vector2 p2 = Vertices[i2];

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
            for (int i = 0; i < Vertices.Length; ++i)
                Vertices[i] -= centroid;

            Mass = Material.Density * area;
            Inertia = Material.Density * I;
        }
    }
    #endregion
}
