using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Serialization
{
    public abstract class ConfigFile : IDisposable
    {
        readonly string _file;
        readonly FileSystemWatcher _fsw;
        protected ConfigFile(string pFile)
        {
            _file = Path.GetFullPath(pFile);
            ParseFile();

#if DEBUG
            //Do not allow hot loading config in release build
            var path = Path.GetDirectoryName(_file);
            var file = Path.GetFileName(_file);
            _fsw = new FileSystemWatcher(path, file);
            _fsw.NotifyFilter = NotifyFilters.LastWrite;
            _fsw.Changed += OnFileChanged;

            _fsw.EnableRaisingEvents = true;
#endif
        }

        private void OnFileChanged(object pSender, FileSystemEventArgs e)
        {
            if(e.FullPath == _file) ParseFile();
        }

        private void ParseFile()
        {
            System.Diagnostics.Debug.Assert(File.Exists(_file));

            var file = File.ReadAllLines(_file);
            foreach(var l in file)
            {
                ParseLine(l);
            }
        }

        public abstract void ParseLine(string pLine);

        public void Dispose()
        {
#if DEBUG
            _fsw.Dispose();
#endif
        }
    }
}
