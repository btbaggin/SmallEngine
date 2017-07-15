using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolusim.Traits
{
    class Trait
    {
        private object _value;
        public Trait(object pValue)
        {
            _value = pValue;
        }

        public T GetValue<T>()
        {
            return (T)_value;
        }
    }
}
