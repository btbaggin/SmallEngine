using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SmallEngine.Serialization
{
    public class SettingsFile
    {
        private Dictionary<string, object> _setting;
        const string VERSION_SETTING = "Settings_Version";
        readonly int _version;

        public static string Directory { get; private set; }

        public SettingsFile(string pDirectory, int pVersion = 1)
        {
            Directory = Path.GetFullPath(pDirectory);
            _version = pVersion;
            ReloadSettings();
        }

        public Setting<T> AddOrGetSetting<T>(string pName, T pDefault)
        {
            return new Setting<T>(pName, pDefault, this);
        }

        private void ReloadSettings()
        {
            bool loaded = false;
            if (File.Exists(Directory))
            {
                try
                {
                    using (FileStream s = new FileStream(Directory, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        _setting = (Dictionary<string, object>)bf.Deserialize(s);
                    }
                     if (GetSetting(VERSION_SETTING, -1) == _version) loaded = true;
                }
                catch (Exception)
                {
                    //Ignore any errors. loaded is already false
                }
            }

            if (!loaded)
            {
                _setting = new Dictionary<string, object>();
                SetSetting(VERSION_SETTING, _version);
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
            using (FileStream s = new FileStream(Directory, FileMode.OpenOrCreate))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(s, _setting);
                s.Flush();
            }
        }

        public T GetSetting<T>(string pName, T pDefault)
        {
            if (_setting.ContainsKey(pName)) return (T)_setting[pName];
            return pDefault;
        }

        public void SetSetting<T>(string pName, T pValue)
        {
            if (_setting.ContainsKey(pName)) _setting[pName] = pValue;
            else _setting.Add(pName, pValue);
        }
    }
}
