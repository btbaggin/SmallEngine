using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    public abstract class ComponentSystem : IUpdatable
    {
        readonly Dictionary<string, IList<Type>> _cache = new Dictionary<string, IList<Type>>();
        readonly Dictionary<IGameObject, IEnumerable<IComponent>> _mapping = new Dictionary<IGameObject, IEnumerable<IComponent>>();

        protected List<IComponent> Components { get; private set; } = new List<IComponent>();

        protected ComponentSystem()
        {
            Scene.Register(this);
        }

        public void GameObjectAdded(string pTemplate, IGameObject pObject)
        {
            IList<IComponent> comps = null;
            //Check if this template has been cached before
            if(!string.IsNullOrEmpty(pTemplate) && _cache.ContainsKey(pTemplate))
            {
                comps = new List<IComponent>();
                foreach(var t in _cache[pTemplate])
                {
                    comps.Add(pObject.GetComponent(t));
                }
            }
            else
            {
                comps = DiscoverComponents(pObject);
            }

            //If it hasn't been cached before we need to discover the components
            lock(Components)
            {
                foreach (var c in comps)
                {
                    Components.AddOrdered(c);
                }
            }

            //Cache any components
            if (!string.IsNullOrEmpty(pTemplate) && !_cache.ContainsKey(pTemplate))
            {
                _cache.Add(pTemplate, new List<Type>());
                foreach(var c in comps)
                {
                    _cache[pTemplate].Add(c.GetType());
                }
            }

            //TODO _mapping.Add(pObject, comps);
        }

        public void GameObjectRemoved(IGameObject pObject)
        {
            foreach(var c in _mapping[pObject])
            {
                Components.Remove(c);
            }
            _mapping.Remove(pObject);
        }

        protected abstract List<IComponent> DiscoverComponents(IGameObject pObject);

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
