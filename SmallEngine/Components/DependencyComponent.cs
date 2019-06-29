using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace SmallEngine.Components
{
    [Serializable]
    public abstract class DependencyComponent : Component
    {
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
        //These methods will null out any fields with the ImportComponent attribute
        //Since these fields are references to other components that are being serializing
        //storing the reference is pointless

        //OnSerializing will null them out
        //OnSerialized will reset the values back
        //OnDeserializing will determine the dependencies so they can be added back when OnAdded is called
        [OnSerializing]
        private void OnSerializing(StreamingContext pContext)
        {
            var fields = _dependencies[GetType()];
            for(int i = 0; i < fields.Count; i++)
            {
                ImportFieldInfo field = fields[i];
                field.SerializingValue = fields[i].Field.GetValue(this);
                field.Field.SetValue(this, null);
                fields[i] = field;
            }
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext pContext)
        {
            var fields = _dependencies[GetType()];
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                field.Field.SetValue(this, null);
                field.SerializingValue = null;
                fields[i] = field;
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext pContext)
        {
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
