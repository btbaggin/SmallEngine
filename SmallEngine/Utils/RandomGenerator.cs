using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public class RandomGenerator
    {
        private static Random _random = new Random();

        public static int RandomInt()
        {
            return _random.Next();
        }

        public static int RandomInt(int pMin, int pMax)
        {
            return _random.Next(pMin, pMax);
        }

        public static float RandomFloat()
        {
            return _random.NextFloat();
        }

        public static float RandomFloat(float pMin, float pMax)
        {
            return _random.Range(pMin, pMax);
        }

        public static double RandomDouble()
        {
            return _random.NextDouble();
        }

        public static double RandomDouble(double pMin, double pMax)
        {
            return pMin + (pMax - pMin) * _random.NextDouble();
        }
    }
}
