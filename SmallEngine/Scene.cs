using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using SmallEngine.Components;
using SmallEngine.Messages;

namespace SmallEngine
{
    public class Scene : IUpdatable
    {
        //https://gamedevelopment.tutsplus.com/tutorials/spaces-useful-game-object-containers--gamedev-14091

        readonly Dictionary<string, IGameObject> _namedObjects;
        readonly ConcurrentBag<IGameObject> _toRemove = new ConcurrentBag<IGameObject>();

        static List<ComponentSystem> _systems = new List<ComponentSystem>();
        static List<Scene> _scenes = new List<Scene>();
        internal readonly static Dictionary<string, List<Type>> _definitions = new Dictionary<string, List<Type>>();

        private bool _started;

        #region Properties
        public List<IGameObject> GameObjects { get; }

        public bool Active { get; set; } = true;
        #endregion  

        #region Constructors
        protected Scene()
        {
            GameObjects = new List<IGameObject>();
            _namedObjects = new Dictionary<string, IGameObject>();
        }

        public static T Create<T>() where T : Scene
        {
            //TODO allow creating scene that isn't registered to all systems
            return (T)Activator.CreateInstance(typeof(T));
        }
        #endregion

        public void Start()
        {
            if (_started) throw new InvalidOperationException("Scene has already started");
            _scenes.Add(this);
            _started = true;
            Begin();
        }

        public void Stop()
        {
            _scenes.Remove(this);
            End();
        }

        #region Overridable Methods
        public virtual void Update(float pDeltaTime)
        {
            foreach(var go in GameObjects)
            {
                go.Update(pDeltaTime);
            }
        }

        protected internal virtual void Begin() { }

        protected internal virtual void End()
        {
            DisposeGameObjects();
        }
        #endregion

        #region Static methods
        public static void Register(ComponentSystem pSystem)
        {
            _systems.Add(pSystem);
        }

        internal static void UpdateAll(float pDeltaTime)
        {
            foreach (var s in _scenes)
            {
                if (s.Active)
                {
                    s.Update(pDeltaTime);
                }
            }
        }

        internal static void DisposeGameObjectsAll()
        {
            foreach (var s in _scenes)
            {
                s.DisposeGameObjects();
            }
        }

        internal static void EndSceneAll()
        {
            foreach (var s in _scenes.ToList())
            {
                s.Stop();
            }
        }
        #endregion

        #region GameObject Creation
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
            T go = (T)Activator.CreateInstance(typeof(T));
            go.Name = pName;
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
                T go = (T)Activator.CreateInstance(typeof(T));
                go.Name = pName;
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

        private void AddGameObject(IGameObject pGameObject, string pName, string pTemplate)
        {
            GameObjects.Add(pGameObject);
            if (pName != null) _namedObjects.Add(pName, pGameObject);
            foreach (var s in _systems)
            {
                s.GameObjectAdded(pTemplate, pGameObject);
            }
            Game.Messages.Register(pGameObject);
            pGameObject.ContainingScene = this;
        }

        public void Destroy(IGameObject pGameObject)
        {
            _toRemove.Add(pGameObject);
        }
        #endregion

        #region GameObject Queries
        public IGameObject FindGameObject(string pName)
        {
            if(_namedObjects.ContainsKey(pName))
            {
                return _namedObjects[pName];
            }

            return null;
        }

        public IEnumerable<IGameObject> FindGameObjectsWithTag(string pTag)
        {
            foreach (var go in GameObjects)
            {
                if (go.Tag == pTag) yield return go;
            }
        }
        #endregion

        internal void DisposeGameObjects()
        {
            while(_toRemove.TryTake(out IGameObject go))
            {
                GameObjects.Remove(go);
                foreach (var s in _systems)
                {
                    s.GameObjectRemoved(go);
                }

                if (!string.IsNullOrEmpty(go.Name) && _namedObjects.ContainsKey(go.Name))
                {
                    _namedObjects.Remove(go.Name);
                }
                go.Dispose();
            }
        }
    }
}
