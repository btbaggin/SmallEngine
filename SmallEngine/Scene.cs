﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using SmallEngine.Components;
using SmallEngine.UI;
using System.Runtime.Serialization;

namespace SmallEngine
{
    /// <summary>
    /// Contains different methods of loading new scenes
    /// </summary>
    public enum SceneLoadModes
    {
        /// <summary>
        /// Loads the new scene in addition to the current scene so that both are updated and drawn.
        /// </summary>
        Additive,

        /// <summary>
        /// Loads the new scene over the current scene so only the new one is updated and drawn,
        /// but the old scene will be restored when the current scene ends.
        /// </summary>
        LoadOver,

        /// <summary>
        /// Loads the new scene in place of the current scene. 
        /// Only the current scene will exist and the old scene will not be able to be restored.
        /// </summary>
        Replace
    }

    [Serializable]
    public class Scene : IUpdatable, ISerializable
    {
        //https://gamedevelopment.tutsplus.com/tutorials/spaces-useful-game-object-containers--gamedev-14091

        readonly Dictionary<string, IGameObject> _namedObjects;
        readonly UIManager _ui;

        static readonly ConcurrentBag<IGameObject> _toRemove = new ConcurrentBag<IGameObject>();
        static SceneGameObjectList _gameobjects = new SceneGameObjectList();
        static List<ComponentSystem> _systems = new List<ComponentSystem>();
        //It should be fine to just iterate to the count for updates
        //if something gets added it shouldn't be updated until the next frame anyway
        static IndexedStack<Scene> _scenes = new IndexedStack<Scene>();
        SceneLoadModes _mode;

        internal readonly static Dictionary<string, List<Type>> _definitions = new Dictionary<string, List<Type>>();

        #region Properties
        public bool Active { get; set; } = true;
        #endregion  

        #region Constructors
        protected Scene()
        {
            _namedObjects = new Dictionary<string, IGameObject>();
            _ui = new UIManager();
        }

        protected Scene(SerializationInfo pInfo, StreamingContext pContext)
        {
            var objects = (IList<IGameObject>)pInfo.GetValue("GameObjects", typeof(IList<IGameObject>));
            foreach(var go in objects)
            {
                foreach(var c in go.GetComponents())
                {
                    c.OnAdded(go);
                }
                go.Initialize();
                AddGameObject(go);
            }
            _mode = (SceneLoadModes)pInfo.GetInt32("Mode");
            _ui = new UIManager();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("GameObjects", _gameobjects.GetGameObjects(), typeof(IList<IGameObject>));
            info.AddValue("Mode", _mode);
        }
        #endregion

        /// <summary>
        /// Loads a new scene using the specified SceneLoadModes
        /// </summary>
        /// <typeparam name="T">Type of scene to load</typeparam>
        /// <param name="pMode">Mode to load the scene with</param>
        public static T Load<T>(SceneLoadModes pMode) where T : Scene
        {
            //TODO allow creating scene that isn't registered to all systems
            var scene = (T)Activator.CreateInstance(typeof(T));
            scene._mode = pMode;
            if (_scenes.Count > 0)
            {
                switch (pMode)
                {
                    case SceneLoadModes.Replace:
                        _scenes.Pop();
                        break;

                    case SceneLoadModes.Additive:
                        break;

                    case SceneLoadModes.LoadOver:
                        //TODO should LoadOver go over additive scenes? or should it load over the current scene?
                        var load = _scenes.Peek();
                        load.Suspend();
                        load.Active = false;
                        break;
                }
            }

            _gameobjects.StartScene(pMode);
            _scenes.Push(scene);
            scene.Begin();

            return scene;
        }

        /// <summary>
        /// Ends the current scene and, if necessary, restores existing scenes
        /// </summary>
        public void Unload()
        {
            End();
            //Dispose of all GameObjects created within the scene
            foreach (var go in _gameobjects.GetGameObjects())
            {
                if (!string.IsNullOrEmpty(go.Name))
                {
                    _namedObjects.Remove(go.Name);
                }
                go.Dispose();
            }

            //Dispose UI elements
            _ui.DisposeElements();

            if (_scenes.Count > 0)
            {
                //Remove current scene
                var scene = _scenes.Pop();
                switch(scene._mode)
                {
                    case SceneLoadModes.Additive:
                    case SceneLoadModes.Replace:
                        //Don't need to do anything
                        break;

                    case SceneLoadModes.LoadOver:
                        var peek = _scenes.Peek();
                        peek.Active = true;
                        peek.Restore();
                        break;
                }
            }

            _gameobjects.EndScene();
        }

        #region Overridable Methods
        public virtual void Update(float pDeltaTime) { }

        public virtual void Draw(IGraphicsAdapter pAdapter) { }

        /// <summary>
        /// Called when the scene is initially loaded
        /// </summary>
        protected virtual void Begin() { }

        /// <summary>
        /// Called when the scene is unloaded
        /// </summary>
        protected virtual void End() { }

        /// <summary>
        /// Called when a scene is loaded over the current scene using <see cref="SceneLoadModes.LoadOver"/> 
        /// </summary>
        protected virtual void Suspend() { }

        /// <summary>
        /// Called when the scene loaded using <see cref="SceneLoadModes.LoadOver"/> is unloaded
        /// </summary>
        protected virtual void Restore() { }
        #endregion

        public static void Register(ComponentSystem pSystem)
        {
            _systems.Add(pSystem);
        }

        #region Internal methods
        internal static void UpdateAll(float pDeltaTime)
        {
            for (int i = 0; i < _scenes.Count; i++)
            {
                var s = _scenes.PeekAt(i);
                if (s.Active) s.Update(pDeltaTime);
            }
        }

        internal static void DrawAll(IGraphicsAdapter pAdapter)
        {
            for(int i = 0; i < _scenes.Count; i++)
            {
                var s = _scenes.PeekAt(i);
                if (s.Active) s.Draw(pAdapter);
            }
        }

        internal static void DisposeDestroyedGameObjects()
        {
            while (_toRemove.TryTake(out IGameObject go))
            {
                _gameobjects.Remove(go);

                if (!string.IsNullOrEmpty(go.Name))
                {
                    go.ContainingScene._namedObjects.Remove(go.Name);
                }
                go.Dispose();
            }
        }

        internal static void EndSceneAll()
        {
            for(int i = 0; i < _scenes.Count; i++)
            {
                _scenes.PeekAt(i).Unload();
            }
        }

        internal static void RegisterUI(UIElement pElement)
        {
            var s = _scenes.Peek();
            s._ui.Register(pElement);
        }

        internal static Scene OnUIElementCreated(UIElement pElement)
        {
            if (_scenes.Count == 0) throw new InvalidOperationException("UI element created with no scene loaded");
            var s = _scenes.Peek();
            if (pElement.Name != null) s._ui.AddNamedElement(pElement);
            return s;
        }

        internal static void DrawUI(IGraphicsAdapter pAdapter)
        {
            for(int i = 0; i < _scenes.Count; i++)
            {
                var s = _scenes.PeekAt(i);
                if (s.Active) s._ui.UpdateAndDraw(pAdapter);
            }
        }

        internal static void InvalidateAllMeasure()
        {
            for(int i = 0; i < _scenes.Count; i++)
            {
                _scenes.PeekAt(i).InvalidateMeasure();
            }
        }
        #endregion

        #region GameObject Creation
        /// <summary>
        /// Defines a template for an IGameObject to be created
        /// </summary>
        /// <param name="pName">Name of the template</param>
        /// <param name="pComponents">Components that will be attached to the IGameObject when it is created</param>
        public static void Define(string pName, params Type[] pComponents)
        {
            _definitions[pName] = pComponents.ToList();
        }

        /// <summary>
        /// Creates a IGameObject with the specified components
        /// </summary>
        /// <param name="pComponents">Components to be attached to the IGameObject</param>
        public T CreateGameObject<T>(params IComponent[] pComponents) where T : IGameObject
        {
            return CreateGameObject<T>(null, pComponents);
        }

        /// <summary>
        /// Creates a IGameObject with the specified name and the specified components
        /// </summary>
        /// <param name="pName">Unique name to give to the game object</param>
        /// <param name="pComponents">Components to be attached to the IGameObject</param>
        /// <returns></returns>
        public T CreateGameObject<T>(string pName, params IComponent[] pComponents) where T : IGameObject
        {
            T go = (T)Activator.CreateInstance(typeof(T));
            go.Name = pName;
            foreach (IComponent c in pComponents)
            {
                go.AddComponent(c);
            }

            go.ContainingScene = this;
            go.Initialize();
            AddGameObject(go);
            return go;
        }

        /// <summary>
        /// Creates a IGameObject from the specified template.
        /// Template must be created by a previous call to <see cref="Define(string, Type[])"/>
        /// </summary>
        /// <param name="pTemplate">Template from which to create the IGameObject</param>
        public T CreateGameObject<T>(string pTemplate) where T : IGameObject
        {
            return CreateGameObject<T>(null, pTemplate);
        }

        /// <summary>
        /// Creates a IGameObject with the specified name from the specified template.
        /// Template must be created by a previous call to <see cref="Define(string, Type[])"/>
        /// </summary>
        /// <param name="pName">Unique name to give to the game object</param>
        /// <param name="pTemplate">Template from which to create the IGameObject</param>
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

                go.ContainingScene = this;
                go.Initialize();

                AddGameObject(go);
                return go;
            }

            return default;
        }

        private void AddGameObject(IGameObject pGameObject)
        {
            _gameobjects.Add(pGameObject);
            if (pGameObject.Name != null) _namedObjects.Add(pGameObject.Name, pGameObject);
            Game.Messages.Register(pGameObject);
        }

        /// <summary>
        /// Marks the specified IGameObject for destruction
        /// </summary>
        /// <remarks>
        /// Destruction may not occur immediately. 
        /// It is up to the scene to determine the best time for GameObjects to be removed
        /// </remarks>
        /// <param name="pGameObject">IGameObject that will be destroyed</param>
        public void Destroy(IGameObject pGameObject)
        {
            _toRemove.Add(pGameObject);
        }
        #endregion

        #region GameObject Queries
        /// <summary>
        /// Finds a IGameObject across all currently loaded scenes with the specified name.
        /// Will return null if no IGameObject is found
        /// </summary>
        /// <param name="pName">Name for which to search</param>
        public static IGameObject FindGameObjectInScenes(string pName)
        {
            for(int i = 0; i < _scenes.Count; i++)
            {
                var scene = _scenes.PeekAt(i);
                if(scene._namedObjects.ContainsKey(pName))
                {
                    return scene._namedObjects[pName];
                }
            }

            throw new GameObjectNotFoundException(pName);
        }

        /// <summary>
        /// Finds a IGameObject within the scene with the specified name.
        /// </summary>
        /// <param name="pName">Name for which to search</param>
        public T FindGameObject<T>(string pName) where T : IGameObject
        {
            if (_namedObjects.ContainsKey(pName))
            {
                return (T)_namedObjects[pName];
            }

            throw new GameObjectNotFoundException(pName);
        }

        public long CreatePointerToObject(string pName)
        {
            if(_namedObjects.ContainsKey(pName))
            {
                return _namedObjects[pName].GetPointer();
            }

            throw new GameObjectNotFoundException(pName);
        }

        public bool TryGetGameObject(long pPointer, out IGameObject pObject)
        {
            return _gameobjects.GetByPointer(pPointer, out pObject);
        }

        /// <summary>
        /// Finds all IGameObjects within the scene that have the specified tag
        /// </summary>
        /// <param name="pTag">Tag for which to search</param>
        public IEnumerable<IGameObject> FindGameObjectsWithTag(string pTag)
        {
            return _gameobjects.GetGameObjects().WithTag(pTag);
        }

        public IList<IGameObject> GetGameObjects()
        {
            return _gameobjects.GetGameObjects();
        }
        #endregion

        #region UI Methods
        internal void InvalidateMeasure()
        {
            _ui.InvalidateMeasure();
        }

        /// <summary>
        /// Finds a UIElement within the scene with the specified name.
        /// Will return null if no UIElement is found
        /// </summary>
        /// <param name="pName">Name for which to search</param>
        public T FindUIElement<T>(string pName) where T : UIElement
        {
            return (T)_ui.GetElement(pName);
        }

        /// <summary>
        /// Finds a UIElement across all currently loaded scenes with the specified name.
        /// Will return null if no UIElement is found
        /// </summary>
        /// <param name="pName">Name for which to search</param>
        public static UIElement FindUIElementInScenes(string pName)
        {
            for(int i = 0; i < _scenes.Count; i++)
            {
                var e = _scenes.PeekAt(i).FindUIElement<UIElement>(pName);
                if (e != null) return e;
            }

            return null;
        }
        #endregion
    }
}
