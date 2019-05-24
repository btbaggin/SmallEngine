using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Input
{
    public class KeyMapping
    {
        readonly Dictionary<string, Keys> _mappings = new Dictionary<string, Keys>();

        public void AddMapping(string pName, Keys pKey)
        {
            _mappings.Add(pName, pKey);
        }

        public void UpdateMapping(string pName, Keys pKey)
        {
            if (!_mappings.ContainsKey(pName)) throw new KeyNotFoundException();
            _mappings[pName] = pKey;
        }

        public Keys GetKey(string pName)
        {
            if (!_mappings.ContainsKey(pName)) throw new KeyNotFoundException();
            return _mappings[pName];
        }

        public bool IsPressed(string pName)
        {
            if (!_mappings.ContainsKey(pName)) throw new KeyNotFoundException();
            return Keyboard.KeyPressed(_mappings[pName]);
        }

        public bool KeyDown(string pName)
        {
            if (!_mappings.ContainsKey(pName)) throw new KeyNotFoundException();
            return Keyboard.KeyDown(_mappings[pName]);
        }

        public bool KeyUp(string pName)
        {
            if (!_mappings.ContainsKey(pName)) throw new KeyNotFoundException();
            return Keyboard.KeyUp(_mappings[pName]);
        }
    }
}
