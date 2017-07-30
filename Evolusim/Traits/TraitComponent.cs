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
            _traits.Add(Traits.Speed, new Trait(Game.RandomInt(50, 200)));
            _traits.Add(Traits.Hunger, new Trait(Game.RandomInt(20, 30)));
            _traits.Add(Traits.Lifetime, new Trait(Game.RandomInt(60, 120)));
            _traits.Add(Traits.Attractive, new Trait(Game.RandomInt(0, 10)));
            _traits.Add(Traits.MateRate, new Trait(Game.RandomInt(0, 3)));
            _traits.Add(Traits.Vision, new Trait(Game.RandomInt(20, 20)));
        }

        public T GetTrait<T>(Traits pTrait)
        {
            return _traits[pTrait].GetValue<T>();
        }
    }
}
