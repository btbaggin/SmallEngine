using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace Evolusim.Traits
{
    class Trait
    {
        public float Min { get; private set; }

        public float Max { get; private set; }

        public float Value { get; private set; }

        public Trait(float pMin, float pMax)
        {
            Min = pMin;
            Max = pMax;
            Value = SmallEngine.Generator.Random.Range(Min, Max);
        }
    }
}
