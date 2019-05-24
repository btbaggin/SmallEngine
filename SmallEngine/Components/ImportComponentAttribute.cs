using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ImportComponentAttribute : Attribute
    {
        public bool Required { get; private set; }

        public bool AllowInheritedTypes { get; private set; }

        public ImportComponentAttribute() : this(true, false) { }

        public ImportComponentAttribute(bool pRequired) : this (pRequired, false) { }

        public ImportComponentAttribute(bool pRequired, bool pAllowInheritedTypes)
        {
            Required = pRequired;
            AllowInheritedTypes = pAllowInheritedTypes;
        }
    }
}
