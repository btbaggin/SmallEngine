using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Serialization
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FileVersionAttribute : Attribute
    {
        public int MinVersion { get; set; }

        public int MaxVersion { get; set; }

        public FileVersionAttribute() { }

        public FileVersionAttribute(int pMinVersion, int pMaxVersion)
        {
            MinVersion = pMinVersion;
            MaxVersion = pMaxVersion;
        }
    }
}
