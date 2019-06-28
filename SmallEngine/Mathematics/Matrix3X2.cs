using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Mathematics
{
    [Serializable]
    public struct Matrix3X2
    {
        public readonly static Matrix3X2 Identity = new Matrix3X2(1, 0, 0, 1, 0, 0);

        float _m00, _m01, _m10, _m11, _m20, _m21; 
        public Matrix3X2(float pM00, float pM01, float pM10, float pM11, float pM20, float pM21)
        {
            _m00 = pM00;
            _m01 = pM01;
            _m10 = pM10;
            _m11 = pM11;
            _m20 = pM20;
            _m21 = pM21;
        }

        public static Matrix3X2 operator *(Matrix3X2 pM1, Matrix3X2 pM2)
        {
            var m00 = (pM1._m00 * pM2._m00) + (pM1._m01 * pM2._m10);
            var m01 = (pM1._m00 * pM2._m01) + (pM1._m01 * pM2._m11);
            var m10 = (pM1._m10 * pM2._m00) + (pM1._m11 * pM2._m10);
            var m11 = (pM1._m10 * pM2._m01) + (pM1._m11 * pM2._m11);
            var m20 = (pM1._m20 * pM2._m00) + (pM1._m21 * pM2._m10) + pM2._m20;
            var m21 = (pM1._m20 * pM2._m01) + (pM1._m21 * pM2._m11) + pM2._m21;
            return new Matrix3X2(m00, m01, m10, m11, m20, m21);
        }

        public static Matrix3X2 Scale(float pX, float pY, Vector2 pCenter)
        {
            Matrix3X2 result;

            result._m00 = pX;
            result._m01 = 0;
            result._m10 = 0;
            result._m11 = pY;

            result._m20 = pCenter.X - (pX * pCenter.X);
            result._m21 = pCenter.Y - (pY * pCenter.Y);

            return result;
        }

        public static implicit operator SharpDX.Matrix3x2(Matrix3X2 pMatrix)
        {
            return new SharpDX.Matrix3x2(pMatrix._m00, pMatrix._m01, pMatrix._m10, pMatrix._m11, pMatrix._m20, pMatrix._m21);
        }
    }
}
