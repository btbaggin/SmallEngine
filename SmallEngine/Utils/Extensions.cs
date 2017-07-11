using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public static class Extensions
    {
        public static float NextFloat(this Random r)
        {
            return (float)r.NextDouble();
        }

        public static float Range(this Random r, float min, float max)
        {
            return min + r.NextFloat() * (max - min);
        }
    }
}
