using System;

namespace SmallEngine
{
    public struct Vector2
    {
        #region Properties
        /// <summary>
        /// X component of the Vector2
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y component of the Vector2
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Returns if the vector is normalized
        /// </summary>
        public bool Normalized
        {
            get { return Math.Abs(Length - 1f) < .005; }
        }

        /// <summary>
        /// Returns the length of the Vector2
        /// </summary>
        public float Length
        {
            get { return MathF.Sqrt((X * X) + (Y * Y)); }
        }

        /// <summary>
        /// Returns the length of the Vector2 squared
        /// </summary>
        public float LengthSqrd
        {
            get { return (X * X) + (Y * Y); }
        }

        /// <summary>
        /// Returns if this is a zero vector
        /// </summary>
        public bool IsZero
        {
            get { return X == 0 && Y == 0; }
        }

        static readonly Vector2 _zeroVector = new Vector2(0f, 0f);
        /// <summary>
        /// Vector with 0 X and Y components
        /// </summary>
        public static Vector2 Zero
        {
            get { return _zeroVector; }
        }

        static readonly Vector2 _unitVector = new Vector2(1f, 1f);
        /// <summary>
        /// Vector with 1 X and Y components
        /// </summary>
        public static Vector2 Unit
        {
            get { return _unitVector; }
        }

        static readonly Vector2 _unitXVector = new Vector2(1f, 0f);
        /// <summary>
        /// Vector with 1 X and 0 Y components
        /// </summary>
        public static Vector2 UnitX
        {
            get { return _unitXVector; }
        }

        static readonly Vector2 _unitYVector = new Vector2(0f, 1f);
        /// <summary>
        /// Vector with 0 X and 1 Y components
        /// </summary>
        public static Vector2 UnitY
        {
            get { return _unitYVector; }
        }
        #endregion

        #region Construtors
        public Vector2(float pXY)
        {
            this.X = pXY;
            this.Y = pXY;
        }

        public Vector2(float pX, float pY) : this()
        {
            this.X = pX;
            this.Y = pY;
        }
        #endregion

        #region Public functions
        /// <summary>
        /// Normalizes the vector to a length of 1
        /// </summary>
        public void Normalize()
        {
            var len = Length;
            if (len == 0) return;

            X /= len;
            Y /= len;
        }

        /// <summary>
        /// Multiplies the vector by pScalar in pDirection
        /// </summary>
        /// <param name="pScalar">Amount to multiply</param>
        /// <param name="pDirection">Direction to apply the multiplication</param>
        public void MultiplyInDirection(float pScalar, Vector2 pDirection)
        {
            pDirection.Normalize();
            var f = pDirection * pScalar;

            X += pDirection.X;
            Y += pDirection.Y;
        }

        /// <summary>
        /// Linearly interpolates between From and To vectors by pAmount
        /// </summary>
        /// <param name="pV1">From vector</param>
        /// <param name="pV2">To vector</param>
        /// <param name="pAmount">Amount to interpolate where 0 = From, 1 = To, .5 = Average of From and To</param>
        /// <returns>Vector interpolated by pAmount</returns>
        public void Lerp(Vector2 pVectorTo, float pAmount)
        {
            X = (X - pVectorTo.X) * pAmount;
            Y = (Y - pVectorTo.Y) * pAmount;
        }

        /// <summary>
        /// Points the vector2 in the opposite direction
        /// </summary>
        public void Reverse()
        {
            X = -X;
            Y = -Y;
        }

        public override string ToString()
        {
            return "X: " + X + ", Y: " + Y;
        }
        #endregion

        #region Static functions
        /// <summary>
        /// Returns a Vector2 pointing in the opposite direction
        /// </summary>
        /// <param name="pV1"></param>
        /// <returns>Vector2 pointing in the opposite direction</returns>
        public static Vector2 Reverse(Vector2 pV1)
        {
            return new Vector2(-pV1.X, -pV1.Y);
        }

        /// <summary>
        /// Linearly interpolates between From and To vectors by pAmount
        /// </summary>
        /// <param name="pVectorFrom">From vector</param>
        /// <param name="pVectorTo">To vector</param>
        /// <param name="pAmount">Amount to interpolate where 0 = From, 1 = To, .5 = Average of From and To</param>
        /// <returns>Vector interpolated by pAmount</returns>
        public static Vector2 Lerp(Vector2 pVectorFrom, Vector2 pVectorTo, float pAmount)
        {
            if (pVectorFrom == null) throw new ArgumentNullException("pV1");
            if (pVectorTo == null) throw new ArgumentNullException("pV2");

            return  pVectorFrom + ((pVectorTo - pVectorFrom) * pAmount);
        }

        public static Vector2 MoveTowards(Vector2 pVectorFrom, Vector2 pVectorTo, float pAmount)
        {
            if (pVectorFrom == null) throw new ArgumentNullException("pV1");
            if (pVectorTo == null) throw new ArgumentNullException("pV2");

            float d = DistanceSqrd(pVectorFrom, pVectorTo);
            Vector2 direction = pVectorFrom + Normalize(pVectorTo - pVectorFrom) * pAmount;
            if(Vector2.DistanceSqrd(pVectorFrom, direction) >= d)
            {
                direction = pVectorTo;
            }
            return direction;
        }

        /// <summary>
        /// Normalizes the vector to a unit vector
        /// </summary>
        /// <param name="pV1">Vector to normalize</param>
        /// <returns>Vector with a length of 1</returns>
        public static Vector2 Normalize(Vector2 pV1)
        {
            if (pV1 == null) throw new ArgumentNullException("pV1");

            if (pV1.Length == 0) return pV1;
            return pV1 / pV1.Length;
        }

        /// <summary>
        /// Caluclates the distance between pV1 and pV2
        /// </summary>
        /// <param name="pV1">First vector to calculate distance</param>
        /// <param name="pV2">Second vector to calculate distance</param>
        /// <returns>Distance between pV1 and pV2</returns>
        public static float Distance(Vector2 pV1, Vector2 pV2)
        {
            if (pV1 == null) throw new ArgumentNullException("pV1");
            if (pV2 == null) throw new ArgumentNullException("pV2");

            var x = pV2.X - pV1.X;
            var y = pV2.Y - pV1.Y;
            return MathF.Sqrt((x * x) + (y * y));
        }

        /// <summary>
        /// Calculates the distance squared between pV1 and pV2
        /// </summary>
        /// <param name="pV1">First vector to calculate distance</param>
        /// <param name="pV2">Second vector to calculate distance</param>
        /// <returns>Distance between pV1 and pV2</returns>
        public static float DistanceSqrd(Vector2 pV1, Vector2 pV2)
        {
            if (pV1 == null) throw new ArgumentNullException("pV1");
            if (pV2 == null) throw new ArgumentNullException("pV2");

            var x = pV2.X - pV1.X;
            var y = pV2.Y - pV1.Y;
            return (x * x) + (y * y);
        }

        /// <summary>
        /// Calculates the dot product of the two vectors
        /// </summary>
        /// <param name="pV1">First vector to calculate the dot product</param>
        /// <param name="pV2">Second vector to calculate the dot product</param>
        /// <returns>Dot product of pV1 and pV2</returns>
        public static float DotProduct(Vector2 pV1, Vector2 pV2)
        {
            if (pV1 == null) throw new ArgumentNullException("pV1");
            if (pV2 == null) throw new ArgumentNullException("pV2");

            return (pV1.X * pV2.X + pV1.Y * pV2.Y);
        }

        /// <summary>
        /// Calculates the cross product of the two vectors
        /// </summary>
        /// <param name="pV1">First vector to calculate the cross product</param>
        /// <param name="pV2">Second vector to calculate the cross product</param>
        /// <returns>Cross product of pV1 and pV2</returns>
        public static Vector2 CrossProduct(Vector2 pV1, Vector2 pV2)
        {
            if (pV1 == null) throw new ArgumentNullException("pV1");
            if (pV2 == null) throw new ArgumentNullException("pV2");

            return new Vector2(pV1.Y - pV2.Y, pV2.X - pV1.X);
        }

        public static Vector2 Clamp(Vector2 pValue, Vector2 pMin, Vector2 pMax)
        {
            if (pValue.X > pMax.X) pValue.X = pMax.X;
            if (pValue.X < pMin.X) pValue.X = pMin.X;
            if (pValue.Y > pMax.Y) pValue.Y = pMax.Y;
            if (pValue.Y < pMin.Y) pValue.Y = pMin.Y;
            return pValue;
        }

        /// <summary>
        /// Returns the vector with the highest X and Y components
        /// </summary>
        /// <param name="pVectors">Vectors to get the max from</param>
        /// <returns>Returns the vector with the highest X and Y components</returns>
        public static Vector2 Max(params Vector2[] pVectors)
        {
            if (pVectors.Length == 0)
            {
                return Vector2.Zero;
            }

            Vector2 max = Vector2.Zero;
            for (int i = 0; i < pVectors.Length; i++)
            {
                if (pVectors[i].X > max.X && pVectors[i].Y > max.Y)
                {
                    max = pVectors[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Creates a vector with the greatest X and Y from the passed in Vectors
        /// </summary>
        /// <param name="pVectors">Vectors to create the most vector from</param>
        /// <returns>Vector with the greatest X and Y</returns>
        public static Vector2 CreateMax(params Vector2[] pVectors)
        {
            if (pVectors.Length == 0)
            {
                return Vector2.Zero;
            }

            var maxX = 0f;
            var maxY = 0f;
            for (int i = 0; i < pVectors.Length; i++)
            {
                if (pVectors[i].X > maxX)
                {
                    maxX = pVectors[i].X;
                }

                if (pVectors[i].Y > maxY)
                {
                    maxY = pVectors[i].Y;
                }
            }

            return new Vector2(maxX, maxY);
        }

        /// <summary>
        /// Returns the vector with the lowest X and Y components
        /// </summary>
        /// <param name="pVectors">Vectors to get the min from</param>
        /// <returns>Returns the vector with the lowest X and Y components</returns>
        public static Vector2 Min(params Vector2[] pVectors)
        {
            if (pVectors.Length == 0)
            {
                return Vector2.Zero;
            }

            Vector2 min = Vector2.Zero;
            for (int i = 0; i < pVectors.Length; i++)
            {
                if (pVectors[i].X > min.X && pVectors[i].Y > min.Y)
                {
                    min = pVectors[i];
                }
            }

            return min;
        }

        /// <summary>
        /// Creates a vector with the least X and Y from the passed in Vectors
        /// </summary>
        /// <param name="pVectors">Vectors to create the lowest vector from</param>
        /// <returns>Vector with the least X and Y</returns>
        public static Vector2 CreateMin(params Vector2[] pVectors)
        {
            if (pVectors.Length == 0)
            {
                return Vector2.Zero;
            }

            var minX = 0f;
            var minY = 0f;
            for (int i = 0; i < pVectors.Length; i++)
            {
                if (pVectors[i].X < minX)
                {
                    minX = pVectors[i].X;
                }

                if (pVectors[i].Y > minY)
                {
                    minY = pVectors[i].Y;
                }
            }

            return new Vector2(minX, minY);
        }

        /// <summary>
        /// Calclates the vector halfway between pV1 and pV2
        /// </summary>
        /// <param name="pV1">First vector for the midpoint</param>
        /// <param name="pV2"><Second vector for the midpoint/param>
        /// <returns>Vector halfway between pV1 and pV2</returns>
        public static Vector2 Midpoint(Vector2 pV1, Vector2 pV2)
        {
            return new Vector2((pV1.X + pV2.X) / 2, (pV1.Y + pV2.Y) / 2);
        }

        /// <summary>
        /// Multiplies pV1 by pScalar in the direction of pDirection
        /// </summary>
        /// <param name="pScalar">Amount to multiply the vector by</param>
        /// <param name="pV1">Vector to multiply</param>
        /// <param name="pDirection">Direction to apply the multiplication</param>
        /// <returns>Multiplied vector</returns>
        public static Vector2 MultiplyInDirection(float pScalar, Vector2 pV1, Vector2 pDirection)
        {
            pDirection.Normalize();
            return pV1 + (pDirection * pScalar);
        }
        #endregion

        #region Operators
        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                return X.Equals(this.X) && Y.Equals(this.Y);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public static explicit operator System.Drawing.Point(Vector2 pVector)
        {
            return new System.Drawing.Point((int)pVector.X, (int)pVector.Y);
        }

        public static explicit operator Vector2(System.Drawing.Point pPoint)
        {
            return new Vector2(pPoint.X, pPoint.Y);
        }

        public static explicit operator System.Drawing.PointF(Vector2 pVector)
        {
            return new System.Drawing.PointF(pVector.X, pVector.Y);
        }

        public static explicit operator Vector2(System.Drawing.PointF pPoint)
        {
            return new Vector2(pPoint.X, pPoint.Y);
        }

        public static implicit operator SharpDX.Mathematics.Interop.RawVector2(Vector2 pVector)
        {
            return new SharpDX.Mathematics.Interop.RawVector2(pVector.X, pVector.Y);
        }

        public static Vector2 operator +(Vector2 pV1, Vector2 pV2)
        {
            return new Vector2(pV1.X + pV2.X, pV1.Y + pV2.Y);
        }

        public static Vector2 operator +(Vector2 pV1, float pScalar)
        {
            return new Vector2(pV1.X + pScalar, pV1.Y + pScalar);
        }

        public static Vector2 operator +(float pScalar, Vector2 pV1)
        {
            return new Vector2(pV1.X + pScalar, pV1.Y + pScalar);
        }

        public static Vector2 operator -(Vector2 pV1, Vector2 pV2)
        {
            return new Vector2(pV1.X - pV2.X, pV1.Y - pV2.Y);
        }

        public static Vector2 operator -(Vector2 pV1, float pScalar)
        {
            return new Vector2(pV1.X - pScalar, pV1.Y - pScalar);
        }

        public static Vector2 operator -(float pScalar, Vector2 pV1)
        {
            return new Vector2(pV1.X - pScalar, pV1.Y - pScalar);
        }

        public static Vector2 operator *(Vector2 pV1, float pScalar)
        {
            return new Vector2(pV1.X * pScalar, pV1.Y * pScalar);
        }

        public static Vector2 operator *(float pScalar, Vector2 pV1)
        {
            return new Vector2(pV1.X * pScalar, pV1.Y * pScalar);
        }

        public static Vector2 operator *(Vector2 pV1, Vector2 pV2)
        {
            return new Vector2(pV1.X * pV2.X, pV1.Y * pV2.Y);
        }

        public static Vector2 operator /(Vector2 pV1, float pScalar)
        {
            return new Vector2(pV1.X / pScalar, pV1.Y / pScalar);
        }

        public static Vector2 operator /(float pScalar, Vector2 pV1)
        {
            return new Vector2(pV1.X / pScalar, pV1.Y / pScalar);
        }

        public static Vector2 operator /(Vector2 pV1, Vector2 pV2)
        {
            return new Vector2(pV1.X / pV2.X, pV1.Y / pV2.Y);
        }

        public static bool operator ==(Vector2 pV1, Vector2 pV2)
        {
            return pV1.X == pV2.X && pV1.Y == pV2.Y;
        }

        public static bool operator !=(Vector2 pV1, Vector2 pV2)
        {
            return !(pV1 == pV2);
        }
        #endregion
    }
}
