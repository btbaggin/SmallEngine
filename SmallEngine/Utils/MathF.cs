using System;
using System.Runtime.CompilerServices;

namespace SmallEngine
{
    public static class MathF
    {
        public static float PI
        {
            get { return (float)Math.PI; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(double pA)
        {
            return (float)Math.Sin(pA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(double pA)
        {
            return (float)Math.Cos(pA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(double pA)
        {
            return (float)Math.Tan(pA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float pValue, float pMin, float pMax)
        {
            if (pValue < pMin) return pMin;
            else if (pValue > pMax) return pMax;
            return pValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float pValueFrom, float pValueTo, float pAmount)
        {
            return pValueFrom + pAmount * (pValueTo - pValueFrom);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Log(double pA)
        {
            return (float)Math.Log(pA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(double pX, double pY)
        {
            return (float)Math.Pow(pX, pY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(double pD)
        {
            return (float)Math.Sqrt(pD);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegreesToRadians(float pAngle)
        {
            return (float)(pAngle * (Math.PI / 180));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float a, int pPlaces)
        {
            return (float)Math.Round(a, pPlaces);
        }
    }

}
