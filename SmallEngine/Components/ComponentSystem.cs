using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    public abstract class ComponentSystem
    {
        readonly Dictionary<string, IEnumerable<Type>> _cache = new Dictionary<string, IEnumerable<Type>>();

        protected List<IComponent> Components { get; private set; } = new List<IComponent>();

        public void GameObjectAdded(string pTemplate, IGameObject pObject)
        {
            if(!string.IsNullOrEmpty(pTemplate) && _cache.ContainsKey(pTemplate))
            {
                lock(Components)
                {
                    foreach (var t in _cache[pTemplate])
                    {
                        Components.Add(pObject.GetComponent(t));
                    }
                }
                return;
            }

            lock(Components)
            {
                foreach (var c in DiscoverComponents(pTemplate, pObject))
                {
                    Components.Add(c);
                }
            }
        }

        public void GameObjectRemoved(IGameObject pObject)
        {
            foreach(var c in DiscoverComponents(null, pObject))
            {
                Components.Remove(c);
            }
        }

        protected abstract List<IComponent> DiscoverComponents(string pTemplate, IGameObject pObject);

        protected void Remember(string pTemplate, Type pType)
        {
            Remember(pTemplate, new Type[] { pType });
        }

        protected void Remember(string pTemplate, IEnumerable<Type> pType)
        {
            if (string.IsNullOrEmpty(pTemplate)) return;

            _cache.Add(pTemplate, pType);
        }

        public void Update(float pDeltaTime)
        {
            lock(Components)
            {
                RunUpdate(pDeltaTime);
            }
        }
        public abstract void RunUpdate(float pDeltaTime);
    }
}
