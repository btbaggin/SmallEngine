using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace SmallEngine.Serialization
{
    /// <summary>
    /// Provides (de)Serialization functionality for game objects
    /// </summary>
    public class GameObjectSerializer
    {
        readonly BinaryFormatter _formatter;
        public GameObjectSerializer()
        {
            _formatter = new BinaryFormatter();
        }

        /// <summary>
        /// Serializes the specified game object into the stream
        /// </summary>
        public void Serialize(System.IO.Stream pStream, IGameObject pGraph)
        {
            _formatter.Serialize(pStream, pGraph);
        }

        /// <summary>
        /// Deserializes a game object from the stream
        /// </summary>
        public IGameObject Deserialize(System.IO.Stream pStream)
        {
            var go = (IGameObject)_formatter.Deserialize(pStream);
            foreach(var c in go.GetComponents())
            {
                c.OnAdded(go);
            }
            return go;
        }

        /// <summary>
        /// Deserializes all game objects from the stream and loads them into the current scene
        /// </summary>
        public void LoadIntoScene(Scene pScene, System.IO.Stream pStream)
        {
            while(pStream.Position < pStream.Length)
            {
                var go = Deserialize(pStream);
                pScene.AddGameObject(go);
            }
        }
    }
}
