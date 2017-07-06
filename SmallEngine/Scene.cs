﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine
{
    public class Scene
    {
        private SceneManager _manager;
        private List<IGameObject> _gameObjects;
        private Dictionary<string, IGameObject> _namedObjects;
        private static List<IGameObject> _persistantObjects;

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
        }

        public Scene()
        {
            _gameObjects = new List<IGameObject>();
            _namedObjects = new Dictionary<string, IGameObject>();
        }
        #endregion

        public virtual void Draw(IGraphicsSystem pSystem)
        {
        }

        public virtual void Update(float pDeltaTime)
        {
        }

        public virtual void OnBegin()
        {
        }

        public virtual Scene OnEnd()
        {
            foreach (var g in _gameObjects.Where((pG) => pG.Persistant))
            {
                _persistantObjects.Add(g);
            }

            return null;
        }

        protected void End()
        {
            _manager.EndScene();
        }

        internal void BeginScene(Game pGame, SceneManager pManager)
        {
            _gameObjects.AddRange(_persistantObjects);
            _persistantObjects.Clear();
            _manager = pManager;
            Game = pGame;
            OnBegin();
        }


        //TODO look at all this stuff
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

        internal void DisposeGameObject(IGameObject pGameObject)
        {
            _gameObjects.Remove(pGameObject);
            //TODO named objects?
        }
    }
}
