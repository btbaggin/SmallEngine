﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace SmallEngine
{
    public class GameObject : IGameObject
    {
        #region Properties
        public string Name { get; private set; }

        public Vector2 Position { get; set; }

        public Vector2 ScreenPosition
        {
            get { return Game.ActiveCamera.ToCameraSpace(Position); }
        }

        public Vector2 Scale { get; set; }

        public float Rotation { get; set; }

        public Rectangle Bounds
        {
            get { return new Rectangle(Position, Scale); }
        }

        protected Game Game { get; private set; }

        public bool MarkedForDestroy { get; private set; }

        public string Tag { get; set; }

        public virtual int Order => 0;
        #endregion  

        private Dictionary<Type, IComponent> _components;

        public GameObject() : this(null) { }

        public GameObject(string pName)
        {
            Name = pName;
            _components = new Dictionary<Type, IComponent>();
        }

        public T GetComponent<T>()
        {
            if (_components.ContainsKey(typeof(T)))
            {
                return (T)_components[typeof(T)];
            }

            return default(T);
        }

        public IComponent GetComponent(Type pType)
        {
            if (Component.IsComponent(pType))
            {
                if (_components.ContainsKey(pType))
                {
                    return _components[pType];
                }
            }

            return null;
        }

        public IEnumerable<IComponent> GetComponents()
        {
            foreach(var c in _components.Values)
            {
                yield return c;
            }
        }

        bool IGameObject.HasComponent<T>()
        {
            return _components.ContainsKey(typeof(T));
        }

        public void AddComponent(IComponent pComponent)
        {
            if(!_components.ContainsKey(pComponent.GetType()))
            {
                _components.Add(pComponent.GetType(), pComponent);
                pComponent.OnAdded(this);
            }
        }

        public void RemoveComponent(Type pComponent)
        {
            _components[pComponent].OnRemoved();
            _components.Remove(pComponent);
        }

        public void SetGame(Game pGame)
        {
            Game = pGame;
        }

        public virtual void Initialize() { }

        public virtual void ReceiveMessage(GameMessage pM) { }

        public void Destroy()
        {
            MarkedForDestroy = true;
            Game.Destroy(this);
        }

        public virtual void Dispose()
        {
            foreach(var c in _components.Values)
            {
                c.OnRemoved();
                c.Dispose();
            }
            _components.Clear();
        }
    }
}
