using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine
{
    public class Scene : IUpdatable
    {
        private List<IGameObject> _gameObjects;
        private List<IGameObject> _requestedGO;
        private Dictionary<string, IGameObject> _namedObjects;
        private static List<IGameObject> _persistantObjects;
        private static List<IGameObject> _toRemove;

        #region Properties
        public List<IGameObject> GameObjects
        {
            get { return _gameObjects; }
        }

        public Game Game { get; private set; }
        #endregion  

        #region Constructors
        static Scene()
        {
            _persistantObjects = new List<IGameObject>();
            _toRemove = new List<IGameObject>();
        }

        public Scene()
        {
            _gameObjects = new List<IGameObject>();
            _requestedGO = new List<IGameObject>();
            _namedObjects = new Dictionary<string, IGameObject>();
        }
        #endregion

        public virtual void Draw(IGraphicsSystem pSystem)
        {
        }

        public virtual void Update(float pDeltaTime)
        {
        }

        protected internal virtual void Begin()
        {
        }

        protected internal virtual void End()
        {
            foreach (var g in _gameObjects.Where((pG) => pG.Persistant))
            {
                _persistantObjects.Add(g);
            }

            DisposeGameObjects();
        }

        internal void BeginScene(Game pGame)
        {
            _gameObjects.AddRange(_persistantObjects);
            _persistantObjects.Clear();
            Game = pGame;
            Begin();
        }

        #region CreateGameObject
        public IGameObject CreateGameObject(params IComponent[] pComponents)
        {
            return CreateGameObject(null, pComponents);
        }

        public IGameObject CreateGameObject(string pName, params IComponent[] pComponents)
        {
            var go = new GameObject(pName);
            foreach (IComponent c in pComponents)
            {
                go.AddComponent(c);
            }

            go.SetGame(Game);
            go.Initialize();
            if (pName == null) { AddGameObject(go); } else { AddGameObject(go, pName); }
            return go;
        }

        public T CreateGameObject<T>(params IComponent[] pComponents) where T : IGameObject
        {
            return CreateGameObject<T>(null, pComponents);
        }

        public T CreateGameObject<T>(string pName, params IComponent[] pComponents) where T : IGameObject
        {
            T go = (T)Activator.CreateInstance(typeof(T), new object[] { });
            foreach (IComponent c in pComponents)
            {
                go.AddComponent(c);
            }

            go.SetGame(Game);
            go.Initialize();
            if (pName == null) { AddGameObject(go); } else { AddGameObject(go, pName); }
            return go;
        }

        public T CreateGameObject<T>(string pTemplate) where T : IGameObject
        {
            return CreateGameObject<T>(null, pTemplate);
        }

        public T CreateGameObject<T>(string pName, string pTemplate) where T : IGameObject
        {
            if (SceneManager._definitions.ContainsKey(pTemplate))
            {
                T go = (T)Activator.CreateInstance(typeof(T), new object[] { });
                foreach (Type t in SceneManager._definitions[pTemplate])
                {
                    go.AddComponent(Component.Create(t));
                }

                go.SetGame(Game);
                go.Initialize();
                if (pName == null) { AddGameObject(go); } else { AddGameObject(go, pName); }
                return go;
            }

            return default(T);
        }

        public IGameObject CreateGameObject(string pTemplate)
        {
            return CreateGameObject(null, pTemplate);
        }

        public IGameObject CreateGameObject(string pName, string pTemplate)
        {
            if (SceneManager._definitions.ContainsKey(pTemplate))
            {
                GameObject go = new GameObject(pName);
                foreach (Type t in SceneManager._definitions[pTemplate])
                {
                    go.AddComponent(Component.Create(t));
                }

                go.SetGame(Game);
                go.Initialize();
                if (pName == null) { AddGameObject(go); } else { AddGameObject(go, pName); }
                return go;
            }

            return null;
        }
        #endregion  

        private void AddGameObject(IGameObject pGameObject)
        {
            if(Game._isInUpdate)
            {
                _requestedGO.Add(pGameObject);
            }
            else
            {
                _gameObjects.Add(pGameObject);
            }
        }

        internal void AddRequestedGameObjects()
        {
            foreach(var go in _requestedGO)
            {
                _gameObjects.Add(go);
            }
            _requestedGO.Clear();
        }

        internal void AddGameObject(IGameObject pGameObject, string pName)
        {
            _gameObjects.Add(pGameObject);
            _namedObjects.Add(pName, pGameObject);
        }

        public IGameObject FindGameObject(string pName)
        {
            if(_namedObjects.ContainsKey(pName))
            {
                return _namedObjects[pName];
            }

            return null;
        }

        public void Destroy(IGameObject pGameObject)
        {
            _toRemove.Add(pGameObject);
        }

        internal void DisposeGameObjects()
        {
            foreach(var go in _toRemove)
            {
                _gameObjects.Remove(go);
                go.Dispose();
                //TODO named objects?
            }

            _toRemove.Clear();
        }
    }
}
