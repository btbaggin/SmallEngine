using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace SmallEngine.Serialization
{
    public class GameObjectSerializer
    {
        readonly BinaryFormatter _formatter;
        public GameObjectSerializer()
        {
            _formatter = new BinaryFormatter();
        }

        public void Serialize(System.IO.Stream pStream, IGameObject pGraph)
        {
            _formatter.Serialize(pStream, pGraph);
        }

        public IGameObject Deserialize(System.IO.Stream pStream)
        {
            var go = (IGameObject)_formatter.Deserialize(pStream);
            foreach(var c in go.GetComponents())
            {
                c.OnAdded(go);
            }
            go.Initialize();
            return go;
        }
    }
}
