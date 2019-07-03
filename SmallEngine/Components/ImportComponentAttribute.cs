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
        /// <summary>
        /// Indicates this component is required and will be added if the game object does not already contain it
        /// </summary>
        public bool Required { get; private set; }

        /// <summary>
        /// Allows types inherited from the component type to be imported in it's place
        /// </summary>
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
