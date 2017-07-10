using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public class MathF
    {
        public static float Pi
        {
            get { return (float)Math.PI; }
        }

        public static float Sin(double pA)
        {
            return (float)Math.Sin(pA);
        }

        public static float Cos(double pA)
        {
            return (float)Math.Cos(pA);
        }

        public static float Tan(double pA)
        {
            return (float)Math.Tan(pA);
        }

        public static float Clamp(float pValue, float pMin, float pMax)
        {
            return (pValue < pMin) ? pMin : ((pValue > pMax) ? pMax : pValue);
        }

        public static float Lerp(float pValueFrom, float pValueTo, float pAmount)
        {
            return (float)((1.0 - pAmount) * pValueFrom + pAmount * pValueTo);
        }

        public static float Log(double pA)
        {
            return (float)Math.Log(pA);
        }

        public static float Pow(double pX, double pY)
        {
            return (float)Math.Pow(pX, pY);
        }

        public static float Sqrt(double pD)
        {
            return (float)Math.Sqrt(pD);
        }

        public static float DegreesToRadians(float pAngle)
        {
            return (float)(pAngle * (Math.PI / 180));
        }
    }

}
