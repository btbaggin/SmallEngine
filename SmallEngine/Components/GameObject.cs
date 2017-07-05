using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public class GameObject : IGameObject
    {
        public string Name { get; private set; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; }

        public float Rotation { get; set; }

        public RectangleF Bounds
        {
            get { return new RectangleF(Position.X, Position.Y, Scale.X, Scale.Y); }
        }

        public bool Persistant { get; set; }

        protected Game Game { get; private set; }

        private Dictionary<Type, IComponent> _components;

        public GameObject() : this(null) { }

        public GameObject(string pName)
        {
            Name = pName;
            _components = new Dictionary<Type, IComponent>();
        }

        T IGameObject.GetComponent<T>()
        {
            if (_components.ContainsKey(typeof(T)))
            {
                return (T)_components[typeof(T)];
            }

            return null;
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
            _components.Add(pComponent.GetType(), pComponent);
            pComponent.OnAdded(this);
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

        public virtual void PreUpdate() { }

        public virtual void Update(float pDeltaTime) { }

        public void Destroy()
        {
            Game.Destroy(this);
        }

        public void Dispose()
        {
            foreach(var c in _components.Values)
            {
                c.OnRemoved();
                c.Dispose();
            }
        }
    }
}
