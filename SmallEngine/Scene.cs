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
        public List<IDrawable> _drawable;
        private List<IUpdatable> _updatable;
        private Dictionary<string, IGameObject> _namedObjects;
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
            _toRemove = new List<IGameObject>();
        }

        public Scene()
        {
            _gameObjects = new List<IGameObject>();
            _drawable = new List<IDrawable>();
            _updatable = new List<IUpdatable>();
            _namedObjects = new Dictionary<string, IGameObject>();
        }
        #endregion

        public virtual void Draw(IGraphicsSystem pSystem)
        {
            foreach(var d in _drawable)
            {
                d.Draw(pSystem);
            }
        }

        public virtual void Update(float pDeltaTime)
        {
            for(int i = 0; i < _updatable.Count; i++)
            {
                _updatable[i].Update(pDeltaTime);
            }
        }

        protected internal virtual void Begin()
        {
        }

        protected internal virtual void End()
        {
            DisposeGameObjects();
        }

        internal void BeginScene(Game pGame)
        {
            Game = pGame;
            Begin();
        }

        //TODO use pooling of GO
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
            AddGameObject(go, pName);
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
            AddGameObject(go, pName);
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
                AddGameObject(go, pName);
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
                AddGameObject(go, pName);
                return go;
            }

            return null;
        }
        #endregion  

        private void AddGameObject(IGameObject pGameObject, string pName)
        {
            _gameObjects.Add(pGameObject);
            if (pName != null) _namedObjects.Add(pName, pGameObject);
        }
        public void AddUpdatable(IUpdatable pUpdatable)
        {
            _updatable.Add(pUpdatable);
        }

        public void RemoveUpdatable(IUpdatable pUpdatable)
        {
            _updatable.Remove(pUpdatable);
        }

        public void AddDrawable(IDrawable pDrawable)
        {
            var i = _drawable.BinarySearch(pDrawable, RenderComponent.Comparer);
            if (i == -1) _drawable.Add(pDrawable);
            else _drawable.Insert(i, pDrawable);
        }

        public void RemoveDrawable(IDrawable pDrawable)
        {
            _drawable.Remove(pDrawable);
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
                go.Dispose();
                _gameObjects.Remove(go);
                if(!string.IsNullOrEmpty(go.Name) && _namedObjects.ContainsKey(go.Name))
                {
                    _namedObjects.Remove(go.Name);
                }
            }

            _toRemove.Clear();
        }
    }
}
