using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using SmallEngine.Components;

namespace SmallEngine
{

    public enum SceneLoadMode
    {
        Replace,
        Additive
    }

    public class Scene : IUpdatable
    {
        readonly Dictionary<string, IGameObject> _namedObjects;
        readonly List<IGameObject> _toRemove = new List<IGameObject>();

        static List<ComponentSystem> _systems = new List<ComponentSystem>();
        static Stack<Scene> _scenes = new Stack<Scene>();
        internal readonly static Dictionary<string, List<Type>> _definitions = new Dictionary<string, List<Type>>();

        #region Properties
        public List<IGameObject> GameObjects { get; }

        public static Scene Current { get; private set; }
        #endregion  

        #region Constructors
        public Scene()
        {
            GameObjects = new List<IGameObject>();
            _namedObjects = new Dictionary<string, IGameObject>();
        }
        #endregion

        public virtual void Update(float pDeltaTime)
        {
            foreach(var go in GameObjects)
            {
                go.Update(pDeltaTime);
            }
        }

        protected internal virtual void Begin()
        {
        }

        protected internal virtual void End()
        {
            DisposeGameObjects();
        }

        #region Static methods
        public static void Register(ComponentSystem pSystem)
        {
            _systems.Add(pSystem);
        }

        public static void Define(string pName, params Type[] pComponents)
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

        public static void BeginScene(Scene pScene)
        {
            BeginScene(pScene, SceneLoadMode.Replace);
        }

        public static void BeginScene(Scene pScene, SceneLoadMode pMode)
        {
            if (pMode == SceneLoadMode.Additive)
            {
                _scenes.Push(Current);
            }

            Current = pScene;
            Current.Begin();
        }

        public static void EndScene()
        {
            Current.End();
            if (_scenes.Count > 0)
            {
                Current = _scenes.Pop();
            }
        }
        #endregion

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

            go.Initialize();
            AddGameObject(go, pName, null);
            return go;
        }

        public T CreateGameObject<T>(params IComponent[] pComponents) where T : IGameObject
        {
            return CreateGameObject<T>(null, pComponents);
        }

        public T CreateGameObject<T>(string pName, params IComponent[] pComponents) where T : IGameObject
        {
            T go = (T)Activator.CreateInstance(typeof(T), new object[] { pName });
            foreach (IComponent c in pComponents)
            {
                go.AddComponent(c);
            }

            go.Initialize();
            AddGameObject(go, pName, null);
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
                T go = (T)Activator.CreateInstance(typeof(T), new object[] { pName });
                foreach (Type t in _definitions[pTemplate])
                {
                    go.AddComponent(Component.Create(t));
                }

                go.Initialize();
                AddGameObject(go, pName, pTemplate);
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

                go.Initialize();
                AddGameObject(go, pName, pTemplate);
                return go;
            }

            return null;
        }
        #endregion  

        private void AddGameObject(IGameObject pGameObject, string pName, string pTemplate)
        {
            GameObjects.Add(pGameObject);
            if (pName != null) _namedObjects.Add(pName, pGameObject);
            foreach(var s in _systems)
            {
                s.GameObjectAdded(pTemplate, pGameObject);
            }
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
                GameObjects.Remove(go);
                foreach(var s in _systems)
                {
                    s.GameObjectRemoved(go);
                }

                if (!string.IsNullOrEmpty(go.Name) && _namedObjects.ContainsKey(go.Name))
                {
                    _namedObjects.Remove(go.Name);
                }
                go.Dispose();
            }

            _toRemove.Clear();
        }
    }
}
