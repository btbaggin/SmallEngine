using System;
using System.Collections.Generic;
using System.Linq;

namespace SmallEngine
{
    public enum SceneLoadMode
    {
        Replace,
        Additive
    }

    public class SceneManager
    {
        private static Dictionary<string, List<Type>> _definitions;
        private static List<IGameObject> _toRemove;
        private static Game _game;
        private static Stack<Scene> _scenes;

        #region Properties
        public static Scene Current { get; private set; }
        #endregion

        #region Constructor
        internal SceneManager(Game pGame)
        {
            _definitions = new Dictionary<string, List<Type>>();
            _toRemove = new List<IGameObject>();
            _scenes = new Stack<Scene>();
            _game = pGame;
        }
        #endregion

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

            go.SetGame(_game);
            go.Initialize();
            if (pName == null) { Current.AddGameObject(go); } else { Current.AddGameObject(go, pName); }
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

            go.SetGame(_game);
            go.Initialize();
             if (pName == null) { Current.AddGameObject(go); } else { Current.AddGameObject(go, pName); }
            return go;
        }

        public T CreateGameObject<T>(string pTemplate) where T : IGameObject
        {
            return CreateGameObject<T>(null, pTemplate);
        }

        public T CreateGameObject<T>(string pName, string pTemplate) where T : IGameObject
        {
            if (_definitions.ContainsKey(pTemplate))
            {
                T go = (T)Activator.CreateInstance(typeof(T), new object[] { });
                foreach (Type t in _definitions[pTemplate])
                {
                    go.AddComponent(Component.Create(t));
                }

                go.SetGame(_game);
                go.Initialize();
                if (pName == null) { Current.AddGameObject(go); } else { Current.AddGameObject(go, pName); }
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
            if (_definitions.ContainsKey(pTemplate))
            {
                GameObject go = new GameObject(pName);
                foreach (Type t in _definitions[pTemplate])
                {
                    go.AddComponent(Component.Create(t));
                }

                go.SetGame(_game);
                go.Initialize();
                if (pName == null) {Current.AddGameObject(go); } else { Current.AddGameObject(go, pName); }
                return go;
            }

            return null;
        }
        #endregion  

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

        public void Destroy(IGameObject pGameObject)
        {
            _toRemove.Add(pGameObject);
        }

        internal void DisposeGameObjects()
        {
            foreach(IGameObject go in _toRemove)
            {
                Current.DisposeGameObject(go);
                go.Dispose();
            }
            _toRemove.Clear();
        }
        
        public static void BeginScene(Scene pScene)
        {
            BeginScene(pScene, SceneLoadMode.Replace);
        }

        public static void BeginScene(Scene pScene, SceneLoadMode pMode)
        {
            if(pMode == SceneLoadMode.Additive)
            {
                //TODO fix
                _scenes.Push(pScene);
            }
            else
            {
                _scenes.Pop();
                _scenes.Push(pScene);
            }
            Current = pScene;
            Current.BeginScene(_game);
        }

        public static void EndScene()
        {
            if(_scenes.Count > 1)
            {
                Current = _scenes.Pop();
            }
            else
            {
                Current.End();
            }
        } 
    }
}
