using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using SharpDX;

namespace SmallEngine
{
    public struct Matrix2X2
    {
        public static Matrix2X2 Identity { get; } = new Matrix2X2(1, 0, 0, 1);
        readonly float m00, m01, m10, m11;
        readonly float _r;

        public Matrix2X2(float pM00, float pM01, float pM10, float pM11)
        {
            _r = 0;
            m00 = pM00;
            m01 = pM01;
            m10 = pM10;
            m11 = pM11;
        }

        public Matrix2X2(float pRadians)
        {
            float c = MathF.Cos(pRadians);
            float s = MathF.Sin(pRadians);

            _r = pRadians;
            m00 = c;
            m01 = -s;
            m10 = s;
            m11 = c;
        }

        public Matrix2X2 Transpose()
        {
            return new Matrix2X2(m00, m10, m01, m11);
        }

        public Transform ToTransform(Vector2 pCenter)
        {
            Matrix3x2.Rotation(_r, new SharpDX.Vector2(pCenter.X, pCenter.Y), out Matrix3x2 m);
            return new Transform(m);
        }

        public static Vector2 operator *(Matrix2X2 pM, Vector2 pV)
        {
            return new Vector2(pM.m00 * pV.X + pM.m01 * pV.Y, pM.m10 * pV.X + pM.m11 * pV.Y);
        }

        public static Vector2 operator *(Vector2 pV, Matrix2X2 pM)
        {
            return new Vector2(pM.m00 * pV.X + pM.m01 * pV.Y, pM.m10 * pV.X + pM.m11 * pV.Y);
        }

    }
}
