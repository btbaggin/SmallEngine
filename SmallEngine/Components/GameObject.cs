﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Messages;
using SmallEngine.Graphics;
using SmallEngine.Components;

namespace SmallEngine
{
    public class GameObject : IGameObject
    {
        #region Properties
        public string Name { get; set; }

        public Vector2 Center
        {
            get { return new Vector2(Position.X + Scale.Width / 2, Position.Y + Scale.Height / 2); }
        }

        public Vector2 Position { get; set; }

        public Size Scale { get; set; }

        float _rotation;
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                ((IGameObject)this).RotationMatrix = new Matrix2X2(_rotation);
            }
        }

        Matrix2X2 IGameObject.RotationMatrix { get; set; } = Matrix2X2.Identity;

        public Matrix3X2 TransformMatrix { get; protected set; } = Matrix3X2.Identity;

        public bool Destroyed { get; private set; }

        public string Tag { get; set; }

        public Scene ContainingScene { get; set; }
        #endregion  

        readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        public GameObject() : this(null) { }

        public GameObject(string pName)
        {
            Name = pName;
        }

        #region Components
        public T GetComponent<T>()
        {
            if (_components.ContainsKey(typeof(T)))
            {
                return (T)_components[typeof(T)];
            }

            return default;
        }

        public IComponent GetComponent(Type pType)
        {
            if (_components.ContainsKey(pType))
            {
                return _components[pType];
            }

            return null;
        }

        public IComponent GetComponentOfType(Type pType)
        {
            foreach(var kv in _components)
            {
                if(pType.IsAssignableFrom(kv.Key))
                {
                    return kv.Value;
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
        #endregion

        #region Overridable Methods
        public virtual void Initialize() { }

        public virtual void Update(float pDeltaTime) { }

        public virtual void ReceiveMessage(IMessage pMessage) { }
        #endregion

        protected void SendMessage(string pType, object pData)
        {
            SendMessage(pType, pData, null);
        }

        protected void SendMessage(string pType, object pData, IMessageReceiver pRecipient)
        {
            Game.Messages.SendMessage(new GameMessage(pType, pData, this, pRecipient));
        }

        protected void SendMessage(IMessage pMessage)
        {
            Game.Messages.SendMessage(pMessage);
        }

        public void Destroy()
        {
            Destroyed = true;
            ContainingScene.Destroy(this);
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
