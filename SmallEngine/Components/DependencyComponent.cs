using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace SmallEngine.Components
{
    [Serializable]
    public abstract class DependencyComponent : Component
    {
        #region ImportFieldInfo
        struct ImportFieldInfo
        {
            public FieldInfo Field;
            public ImportComponentAttribute Info;
            public object SerializingValue;

            public ImportFieldInfo(FieldInfo pField, ImportComponentAttribute pInfo)
            {
                Field = pField;
                Info = pInfo;
                SerializingValue = null;
            }
        }
        #endregion

        [NonSerialized] static readonly Dictionary<Type, List<ImportFieldInfo>> _dependencies = new Dictionary<Type, List<ImportFieldInfo>>();
        [NonSerialized] List<ImportFieldInfo> _fields = new List<ImportFieldInfo>();
        protected DependencyComponent()
        {
            var t = GetType();
            DetermineDepencencies(t);

            while ((t = t.BaseType) != null && IsComponent(t))
            {
                DetermineDepencencies(t);
            }
        }

        /// <summary>
        /// Determines dependencies for the component and imports all fields marked with ImportComponentAttribute
        /// </summary>
        /// <param name="pGameObject"></param>
        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);

            foreach (var d in _fields)
            {
                IComponent component = null;
                var type = d.Field.FieldType;
                if (d.Info.AllowInheritedTypes)
                {
                    //Look for any inherited types
                    foreach(var c in pGameObject.GetComponents())
                    {
                        if(type.IsInstanceOfType(c))
                        {
                            component = c;
                            break;
                        }
                    }
                }
                else
                {
                    //Get actual type
                    component = pGameObject.GetComponent(type);
                }

                if(component == null && d.Info.Required)
                {
                    //Create if required
                    component = Create(type);
                    pGameObject.AddComponent(component);
                }

                if (component == null) continue;

                d.Field.SetValue(this, component);
            }
        }

        /// <summary>
        /// Looks for any fields marked with ImportComponentAttribute
        /// </summary>
        private void DetermineDepencencies(Type pType)
        {
            var importFields = new List<ImportFieldInfo>();
            if(!_dependencies.ContainsKey(pType))
            {
                var fields = pType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (FieldInfo f in fields)
                {
                    var type = f.FieldType;
                    var att = f.GetCustomAttribute<ImportComponentAttribute>();
                    if (att != null)
                    {
                        System.Diagnostics.Debug.Assert(IsComponent(type) && type.GetConstructor(Type.EmptyTypes) != null,
                                                        "ImportComponent must be IComponent and have an empty constructor");
                        importFields.Add(new ImportFieldInfo(f, att));
                    }
                }
                _dependencies.Add(pType, importFields);
            }
            else
            {
                importFields = _dependencies[pType];
            }

            _fields.AddRange(importFields);
        }

        #region Serialization
        /// <summary>
        /// Import dependencies from other components that have been deserialized
        /// </summary>
        /// <param name="pContext"></param>
        protected override void OnDeserializeBegin()
        {
            base.OnDeserializeBegin();
            _fields = new List<ImportFieldInfo>();
            var t = GetType();
            DetermineDepencencies(t);

            while ((t = t.BaseType) != null && IsComponent(t))
            {
                DetermineDepencencies(t);
            }
        }
        #endregion
    }
}
