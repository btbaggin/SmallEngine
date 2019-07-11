﻿using System;
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
        /// <summary>
        /// Indicates the type that will be registered with systems.
        /// If you wish to provide a type that can be inherited from set the RegistrationType to the base class
        /// </summary>
        protected virtual Type RegistrationType => GetType();

        private bool _active = true;
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IGameObject GameObject { get; protected set; }

        protected IComparer<IComponent> Comparer { get; set; }
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

        /// <inheritdoc/>
        public virtual void OnAdded(IGameObject pGameObject)
        {
            GameObject = pGameObject;
            if (Active)
            {
                //TODO need this for serialization? idk...
                if (!_components.ContainsKey(RegistrationType)) _components.Add(RegistrationType, new List<IComponent>());
                _components[RegistrationType].AddOrdered(this, Comparer);
            }
        }

        /// <inheritdoc/>
        public virtual void OnRemoved()
        {
            _components[RegistrationType].Remove(this);
            GameObject = null;
        }

        /// <inheritdoc/>
        public virtual void OnActiveChanged(bool pActive)
        {
            if(pActive)
            {
                _components[RegistrationType].AddOrdered(this, Comparer);
            }
            else
            {
                _components[RegistrationType].Remove(this);
            }
        }

        public virtual void Dispose() { }

        /// <summary>
        /// Returns true if the type is an IComponent
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static bool IsComponent(Type pType)
        {
            return typeof(IComponent).IsAssignableFrom(pType);
        }

        /// <summary>
        /// Returns all registered components of the specified type
        /// </summary>
        public static List<IComponent> GetComponentsOfType(Type pType)
        {
            if (!_components.ContainsKey(pType)) _components.Add(pType, new List<IComponent>());
            return _components[pType];
        }
    }
}
