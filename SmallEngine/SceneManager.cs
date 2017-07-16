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
        internal static Dictionary<string, List<Type>> _definitions;
        private static Game _game;
        private static Stack<Scene> _scenes;

        #region Properties
        public static Scene Current { get; private set; }
        #endregion

        #region Constructor
        internal SceneManager(Game pGame)
        {
            _definitions = new Dictionary<string, List<Type>>();
            _scenes = new Stack<Scene>();
            _game = pGame;
        }
        #endregion

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
            if(pMode == SceneLoadMode.Additive)
            {
                _scenes.Push(Current);
            }

            Current = pScene;
            Current.BeginScene(_game);
        }

        public static void EndScene()
        {
            Current.End();
            if(_scenes.Count > 0)
            {
                Current = _scenes.Pop();
            }
        } 
    }
}
