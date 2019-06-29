using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    [Serializable]
    public abstract class Component : IComponent
    {
        static Dictionary<Type, List<IComponent>> _components = new Dictionary<Type, List<IComponent>>();

        #region Properties
        protected virtual Type RegistrationType => GetType();

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
        #endregion

        #region Constructors
        public static IComponent Create(Type pType)
        {
            return (IComponent)Activator.CreateInstance(pType);
        }

        protected Component()
        {
            var t = GetType();
            if (!_components.ContainsKey(t))
            {
                _components.Add(t, new List<IComponent>());
            }
        }
        #endregion

        public virtual void OnAdded(IGameObject pGameObject)
        {
            GameObject = pGameObject;
            _components[RegistrationType].AddOrdered(this);
        }

        public virtual void OnRemoved()
        {
            _components[RegistrationType].Remove(this);
            GameObject = null;
        }

        public virtual void OnActiveChanged(bool pActive)
        {
            if(pActive)
            {
                _components[RegistrationType].Add(this);
            }
            else
            {
                _components[RegistrationType].Remove(this);
            }
        }

        public virtual void Dispose() { }

        public static bool IsComponent(Type pType)
        {
            return typeof(IComponent).IsAssignableFrom(pType);
        }

        public virtual int CompareTo(IComponent other)
        {
            return 0;
        }

        public static List<IComponent> GetComponentsOfType(Type pType)
        {
            if (!_components.ContainsKey(pType)) _components.Add(pType, new List<IComponent>());
            return _components[pType];
        }
    }
}
