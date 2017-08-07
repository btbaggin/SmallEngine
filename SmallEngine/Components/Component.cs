using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public abstract class Component : IComponent, IUpdatable
    {
        public EventHandler<EventArgs> Added;
        public EventHandler<EventArgs> Removed;

        public Component()
        {
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
            try
            {
                return (IComponent)Activator.CreateInstance(pType);
            }
            catch
            {
                throw new Exception();
            }
        }

        public virtual void OnAdded(IGameObject pGameObject)
        {
            GameObject = pGameObject;
            SceneManager.Current.AddUpdatable(this);
            Added?.Invoke(this, new EventArgs());
        }

        public virtual void OnRemoved()
        {
            Removed?.Invoke(this, new EventArgs());
            GameObject = null;
        }

        public virtual void OnActiveChanged(bool pActive)
        {
        }

        public virtual void Update(float pDeltaTime)
        {
        }

        public virtual void Dispose()
        {
        }

        public static bool IsComponent(Type pType)
        {
            return typeof(IComponent).IsAssignableFrom(pType);
        }
    }
}
