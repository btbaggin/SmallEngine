using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Messages;

namespace SmallEngine
{
    public class GameObject : IGameObject
    {
        #region Properties
        public string Name { get; private set; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; }

        public float Rotation { get; set; }

        public Rectangle Bounds
        {
            get { return new Rectangle(Position, Scale); }
        }

        public bool MarkedForDestroy { get; private set; }

        public string Tag { get; set; }

        public virtual int Order => 0;
        #endregion  

        readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        public GameObject() : this(null) { }

        public GameObject(string pName)
        {
            Name = pName;
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

        public virtual void Initialize() { }

        public virtual void Update(float pDeltaTime) { }

        public virtual void ReceiveMessage(IMessage pMessage) { }

        public void Destroy()
        {
            MarkedForDestroy = true;
            Scene.Current.Destroy(this);
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
