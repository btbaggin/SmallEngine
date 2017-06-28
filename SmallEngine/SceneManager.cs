using System;
using System.Collections.Generic;
using System.Linq;

namespace SmallEngine
{
    public class SceneEventArgs : EventArgs
    {
        public string Scene { get; private set; }
        public SceneEventArgs(string pScene)
        {
            Scene = pScene;
        }
    }

    public class SceneManager
    {
        public EventHandler<SceneEventArgs> SceneBegin;
        public EventHandler<SceneEventArgs> SceneEnd;

        private Dictionary<string, List<Type>> _definitions;

        #region "Properties"
        public Scene Current { get; private set; }

        private static SceneManager _instance;
        public static SceneManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SceneManager();
                }

                return _instance;
            }
        }
        #endregion

        #region "Constructor"
        public SceneManager()
        {
            _definitions = new Dictionary<string, List<Type>>();
            _instance = this;
        }
        #endregion

        #region "SpawnGameObject"
        public IGameObject SpawnGameObject(params IComponent[] pComponents)
        {
            return SpawnGameObject(null, pComponents);
        }

        public IGameObject SpawnGameObject(string pName, params IComponent[] pComponents)
        {
            var go = new GameObject(pName);
            foreach (IComponent c in pComponents)
            {
                go.AddComponent(c);
            }

            go.Initialize();
            if (pName == null) { Current.AddGameObject(go); } else { Current.AddGameObject(go, pName); }
            return go;
        }

        public T SpawnGameObject<T>(params IComponent[] pComponents) where T : IGameObject
        {
            return SpawnGameObject<T>(null, pComponents);
        }

        public T SpawnGameObject<T>(string pName, params IComponent[] pComponents) where T : IGameObject
        {
            T go = (T)Activator.CreateInstance(typeof(T), new object[] { });
            foreach (IComponent c in pComponents)
            {
                go.AddComponent(c);
            }
            go.Initialize();
             if (pName == null) { Current.AddGameObject(go); } else { Current.AddGameObject(go, pName); }
            return go;
        }

        public T SpawnGameObject<T>(string pTemplate) where T : IGameObject
        {
            return SpawnGameObject<T>(null, pTemplate);
        }

        public T SpawnGameObject<T>(string pName, string pTemplate) where T : IGameObject
        {
            if (_definitions.ContainsKey(pTemplate))
            {
                T go = (T)Activator.CreateInstance(typeof(T), new object[] { });
                foreach (Type t in _definitions[pTemplate])
                {
                    go.AddComponent(Component.Create(t));
                }
                go.Initialize();
                if (pName == null) { Current.AddGameObject(go); } else { Current.AddGameObject(go, pName); }
                return go;
            }

            return default(T);
        }

        public IGameObject SpawnGameObject(string pTemplate)
        {
            return SpawnGameObject(null, pTemplate);
        }

        public IGameObject SpawnGameObject(string pName, string pTemplate)
        {
            if (_definitions.ContainsKey(pTemplate))
            {
                GameObject go = new GameObject(pName);
                foreach (Type t in _definitions[pTemplate])
                {
                    go.AddComponent(Component.Create(t));
                }

                go.Initialize();
                if (pName == null) {Current.AddGameObject(go); } else { Current.AddGameObject(go, pName); }
                return go;
            }

            return null;
        }
        #endregion  

        public IGameObject CreateGameObject(params IComponent[] pComponents)
        {
            var go = new GameObject();
            foreach (IComponent c in pComponents)
            {
                go.AddComponent(c);
            }

            go.Initialize();
            return go;
        }

        public T CreateGameObject<T>(params IComponent[] pComponents) where T : IGameObject
        {
            T go = (T)Activator.CreateInstance(typeof(T), new object[] { });
            foreach (IComponent c in pComponents)
            {
                go.AddComponent(c);
            }
            go.Initialize();
            return go;
        }

        public void AddToScene(IGameObject pGameObject)
        {
            Current.AddGameObject(pGameObject);
        }

        public void Define(string pName, params Type[] pComponents)
        {
            if (!_definitions.ContainsKey(pName))
            {
                _definitions.Add(pName, pComponents.ToList());
            }
            else
            {
                _definitions[pName] = pComponents.ToList();
            }
        }

        public IGameObject FindGameObject(string pName)
        {
            return Current.FindGameObject(pName);
        }
        
        public void BeginScene(string pName)
        {
            Current = new Scene(pName);
            Current.Begin();

            SceneBegin?.Invoke(this, new SceneEventArgs(pName));
        }
        
        public void EndScene()
        {
            Current.End();

            SceneEnd?.Invoke(this, new SceneEventArgs(Current.Name));
        } 
    }
}
