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
            Speed
        }

        private Dictionary<Traits, Trait> _traits;
        public TraitComponent()
        {
            _traits = new Dictionary<Traits, Trait>();
            _traits.Add(Traits.Speed, new Trait(Game.RandomInt(50, 200)));
        }

        public T GetTrait<T>(Traits pTrait)
        {
            return _traits[pTrait].GetValue<T>();
        }
    }
}
