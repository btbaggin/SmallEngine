using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    class ImportComponentAttribute : Attribute
    {
        public ImportComponentAttribute()
        {

        }
    }
}
