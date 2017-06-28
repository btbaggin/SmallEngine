using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public class Scene
    {
        private List<IGameObject> _gameObjects;
        private Dictionary<string, IGameObject> _namedObjects;
        private static List<IGameObject> _persistantObjects;

        #region "Properties"
        public List<IGameObject> GameObjects
        {
            get { return _gameObjects; }
        }

        public string Name { get; private set; }
        #endregion  

        #region "Constructors"
        static Scene()
        {
            _persistantObjects = new List<IGameObject>();
        }

        internal Scene(string pName)
        {
            _gameObjects = new List<IGameObject>();
            _namedObjects = new Dictionary<string, IGameObject>();
            Name = pName;
        }
        #endregion

        internal void AddGameObject(IGameObject pGameObject)
        {
            _gameObjects.Add(pGameObject);
        }

        internal void AddGameObject(IGameObject pGameObject, string pName)
        {
            _gameObjects.Add(pGameObject);
            _namedObjects.Add(pName, pGameObject);
        }

        internal IGameObject FindGameObject(string pName)
        {
            if(_namedObjects.ContainsKey(pName))
            {
                return _namedObjects[pName];
            }

            return null;
        }

        internal void Begin()
        {
            _gameObjects.AddRange(_persistantObjects);

            _persistantObjects.Clear();
        }

        internal void End()
        {
            foreach(var g in _gameObjects.Where((pG) => pG.Persistant))
            {
                _persistantObjects.Add(g);
            }
        }
    }
}
