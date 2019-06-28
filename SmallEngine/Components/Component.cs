using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    public abstract class Component : IComponent
    {
        public EventHandler<EventArgs> Added { get; set; }
        public EventHandler<EventArgs> Removed { get; set; }

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

        protected Component(SerializationInfo pInfo, StreamingContext pContext)
        {
            Active = pInfo.GetBoolean("Active");
            if (!_components.ContainsKey(GetType())) _components.Add(GetType(), new List<IComponent>());
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Active", Active);
        }
        #endregion

        public virtual void OnAdded(IGameObject pGameObject)
        {
            GameObject = pGameObject;
            _components[RegistrationType].AddOrdered(this);
            Added?.Invoke(this, new EventArgs());
        }

        public virtual void OnRemoved()
        {
            Removed?.Invoke(this, new EventArgs());
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

        public static IEnumerable<IComponent> GetComponentsOfType(Type pType)
        {
            if (!_components.ContainsKey(pType)) _components.Add(pType, new List<IComponent>());
            return _components[pType];
        }
    }
}
