using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmallEngine
{
    public abstract class DependencyComponent : Component
    {
        private Dictionary<FieldInfo, ImportComponentAttribute> _dependencies;
        public DependencyComponent()
        {
            _dependencies = new Dictionary<FieldInfo, ImportComponentAttribute>();
            var t = GetType();
            DetermineDepencencies(t);

            while ((t = t.BaseType) != null &&
                   IsComponent(t))
            {
                DetermineDepencencies(t);
            }
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            GameObject = pGameObject;
            foreach (var d in _dependencies)
            {
                var component = pGameObject.GetComponent(d.Key.FieldType);
                if(component == null)
                {
                    component = Create(d.Key.FieldType);
                    pGameObject.AddComponent(component);
                }
                if (component == null)
                {
                    continue;
                }

                d.Key.SetValue(this, component);
            }
        }

        private void DetermineDepencencies(Type pType)
        {
            var fields = pType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo f in fields)
            {
                var type = f.FieldType;
                var att = f.GetCustomAttribute<ImportComponentAttribute>();
                if (att != null)
                {
                    System.Diagnostics.Debug.Assert(IsComponent(type) && 
                                                    type.GetConstructor(Type.EmptyTypes) != null, 
                                                    "ImportComponent must be IComponent and have an empty constructor");
                    _dependencies.Add(f, att);
                }
            }
        }
    }
}
