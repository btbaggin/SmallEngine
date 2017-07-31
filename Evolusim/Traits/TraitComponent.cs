using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using Evolusim.Traits;

namespace Evolusim
{
    class TraitComponent : Component
    {
        public enum Traits
        {
            Speed,
            Hunger,
            Lifetime,
            Attractive,
            MateRate,
            Vision
        }

        private Dictionary<Traits, Trait> _traits;
        public TraitComponent()
        {
            _traits = new Dictionary<Traits, Trait>();
            _traits.Add(Traits.Speed, new Trait(RandomGenerator.RandomInt(50, 200)));
            _traits.Add(Traits.Hunger, new Trait(RandomGenerator.RandomInt(20, 30)));
            _traits.Add(Traits.Lifetime, new Trait(RandomGenerator.RandomInt(60, 120)));
            _traits.Add(Traits.Attractive, new Trait(RandomGenerator.RandomInt(0, 10)));
            _traits.Add(Traits.MateRate, new Trait(RandomGenerator.RandomInt(0, 3)));
            _traits.Add(Traits.Vision, new Trait(RandomGenerator.RandomInt(5, 15)));
        }

        public T GetTrait<T>(Traits pTrait)
        {
            return _traits[pTrait].GetValue<T>();
        }
    }
}
