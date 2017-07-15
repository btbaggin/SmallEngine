using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ImportComponentAttribute : Attribute
    {
        public ImportComponentAttribute()
        {

        }
    }
}
