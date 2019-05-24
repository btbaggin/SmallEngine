using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Serialization
{
    public class Setting<T>
    {
        public string Name { get; }
        public T Default { get; }

        T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _file.SetSetting(Name, value);
            }
        }

        readonly SettingsFile _file;
        internal Setting(string pName, T pDefault, SettingsFile pFile)
        {
            Name = pName;
            Default = pDefault;
            _file = pFile;
            Value = _file.GetSetting(pName, pDefault);
        }
    }
}
