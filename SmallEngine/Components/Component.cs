using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    public abstract class Component : IComponent
    {
        public EventHandler<EventArgs> Added { get; set; }
        public EventHandler<EventArgs> Removed { get; set; }

        static Dictionary<Type, HashSet<IComponent>> _components = new Dictionary<Type, HashSet<IComponent>>();

        protected Component()
        {
            var t = GetType();
            if(!_components.ContainsKey(t))
            {
                _components.Add(t, new HashSet<IComponent>());
            }
        }

        private bool _active = true;
        public bool Active
        {
            get { return _active; }
            set
            {
                if(value != _active)
                {
                    _active = value;
                    OnActiveChanged(_active);
                }
            }
        }

        public IGameObject GameObject { get; protected set; }

        public static IComponent Create(Type pType)
        {
            return (IComponent)Activator.CreateInstance(pType);
        }

        public virtual void OnAdded(IGameObject pGameObject)
        {
            GameObject = pGameObject;
            Added?.Invoke(this, new EventArgs());
            _components[GetType()].Add(this);
        }

        public virtual void OnRemoved()
        {
            Removed?.Invoke(this, new EventArgs());
            GameObject = null;
            _components[GetType()].Remove(this);
        }

        public virtual void OnActiveChanged(bool pActive)
        {
        }

        public virtual void Dispose()
        {
        }

        public static bool IsComponent(Type pType)
        {
            return typeof(IComponent).IsAssignableFrom(pType);
        }

        public virtual int CompareTo(IComponent other)
        {
            return 0;
        }

        public static IEnumerable<IComponent> GetComponentsOfType(Type pType)
        {
            if (!_components.ContainsKey(pType)) _components.Add(pType, new HashSet<IComponent>());
            return _components[pType];
        }
    }
}
