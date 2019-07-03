using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Messages;
using SmallEngine.Graphics;
using SmallEngine.Components;
using System.Runtime.Serialization;

namespace SmallEngine
{
    [Serializable]
    public class GameObject : IGameObject
    {
        #region Properties
        /// <inheritdoc/>
        public string Name { get; set; }

        public Vector2 Center
        {
            get { return new Vector2(Position.X + Scale.Width / 2, Position.Y + Scale.Height / 2); }
        }

        /// <inheritdoc/>
        public Vector2 Position { get; set; }

        /// <inheritdoc/>
        public Size Scale { get; set; }

        float _rotation;
        /// <inheritdoc/>
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _rotationMatrix = new Mathematics.Matrix2X2(_rotation);
            }
        }

        Mathematics.Matrix2X2 _rotationMatrix = Mathematics.Matrix2X2.Identity;
        /// <inheritdoc/>
        Mathematics.Matrix2X2 IGameObject.RotationMatrix { get { return _rotationMatrix; } } 

        /// <inheritdoc/>
        public Mathematics.Matrix3X2 TransformMatrix { get; set; } = Mathematics.Matrix3X2.Identity;

        /// <inheritdoc/>
        [field: NonSerialized]
        public bool Destroyed { get; private set; }

        /// <inheritdoc/>
        public string Tag { get; set; }

        /// <inheritdoc/>
        [field: NonSerialized]
        int IGameObject.Index { get; set; }

        /// <inheritdoc/>
        [field: NonSerialized]
        ushort IGameObject.Version { get; set; }

        /// <inheritdoc/>
        [field: NonSerialized]
        public Scene ContainingScene { get; set; }
        #endregion  

        readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        #region Constructor
        public GameObject() : this(null) { }

        public GameObject(string pName)
        {
            Name = pName;
        }
        #endregion

        #region Components
        /// <inheritdoc/>
        public T GetComponent<T>()
        {
            if (_components.ContainsKey(typeof(T)))
            {
                return (T)_components[typeof(T)];
            }

            return default;
        }

        /// <inheritdoc/>
        public IComponent GetComponent(Type pType)
        {
            if (_components.ContainsKey(pType))
            {
                return _components[pType];
            }

            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<IComponent> GetComponents()
        {
            foreach(var c in _components.Values)
            {
                yield return c;
            }
        }

        /// <inheritdoc/>
        bool IGameObject.HasComponent<T>()
        {
            return _components.ContainsKey(typeof(T));
        }

        /// <inheritdoc/>
        public void AddComponent(IComponent pComponent)
        {
            if(!_components.ContainsKey(pComponent.GetType()))
            {
                _components.Add(pComponent.GetType(), pComponent);
                pComponent.OnAdded(this);
            }
        }

        /// <inheritdoc/>
        public void RemoveComponent(Type pComponent)
        {
            _components[pComponent].OnRemoved();
            _components.Remove(pComponent);
        }
        #endregion

        #region Overridable Methods
        /// <inheritdoc/>
        public virtual void Initialize() { }

        /// <inheritdoc/>
        public virtual void ReceiveMessage(IMessage pMessage) { }
        #endregion

        #region Messages
        /// <summary>
        /// Sends a message to the games default message queue
        /// </summary>
        protected void SendMessage(string pType, object pData)
        {
            SendMessage(pType, pData, null);
        }

        /// <summary>
        /// Sends a message to the games default message queue
        /// </summary>
        protected void SendMessage(string pType, object pData, IMessageReceiver pRecipient)
        {
            Game.Messages.SendMessage(new GameMessage(pType, pData, this, pRecipient));
        }

        /// <summary>
        /// Sends a message to the games default message queue
        /// </summary>
        protected void SendMessage(IMessage pMessage)
        {
            Game.Messages.SendMessage(pMessage);
        }
        #endregion

        #region Pointer
        /// <inheritdoc/>
        public long GetPointer()
        {
            var go = (IGameObject)this;
            long pointer = 0;
            pointer |= (long)go.Version;
            pointer = pointer << 32;
            pointer |= (long)go.Index;
            return pointer + 1; //Add 1 so that 0 is an invalid pointer
        }

        internal static void GetIndexAndVersion(long pPointer, out int pIndex, out ushort pVersion)
        {
            var pointer = pPointer - 1;
            pIndex = (int)pointer;
            pVersion = (ushort)(pointer >> 32);
        }
        #endregion

        /// <inheritdoc/>
        public void Destroy()
        {
            Destroyed = true;
            ContainingScene.Destroy(this);
        }

        /// <inheritdoc/>
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
