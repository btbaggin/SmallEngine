using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using SmallEngine.Components;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SmallEngine.Serialization
{
    public class CustomGameObjectSerializer
    {
        enum SerializedDataType : byte
        {
            GameObject,
            Object,
            Null
        }

        static Dictionary<Type, List<FieldInfo>> _typeCache = new Dictionary<Type, List<FieldInfo>>();
        static Dictionary<Type, MethodInfo> _beginDeserialize = new Dictionary<Type, MethodInfo>();
        static Dictionary<Type, MethodInfo> _finishDeserialize = new Dictionary<Type, MethodInfo>();

        readonly BinaryFormatter _formatter = new BinaryFormatter();

        public int Version { get; private set; }

        public CustomGameObjectSerializer(int pVersion = 1)
        {
            Version = pVersion;
        }

        #region Serialization
        public void Serialize(Stream pStream, Scene pScene)
        {
            var gameObjects = pScene.GetGameObjects();
            Serialize(pStream, gameObjects);
        }

        public void Serialize(Stream pStream, IList<IGameObject> pObjects)
        {
            pStream.WriteInt(Version);
            pStream.WriteInt(pObjects.Count);

            for(int i = 0; i < pObjects.Count; i++)
            {
                var go = pObjects[i];

                //Write type so we can create an instance when we deserialize
                var t = go.GetType();
                pStream.WriteString(t.AssemblyQualifiedName);

                var members = GetSerializableMembers(t, Version);

                var data = FormatterServices.GetObjectData(go, members.ToArray());

                //Instead of serializing GO fields we will just put the index
                //This will allow us to not duplicate the data and correct the reference at the end
                //It doesn't matter if i isn't correct in the new scene, it just matters that it's unique in the serialized stream
                pStream.WriteInt(i);

                //Serialize game object
                foreach (var d in data)
                {
                    SerializeMember(pStream, d, pObjects);
                }

                //Serialize components
                var components = go.GetComponents().ToList();
                pStream.WriteInt(components.Count);
                foreach (var c in components)
                {
                    t = c.GetType();
                    pStream.WriteString(t.AssemblyQualifiedName);

                    members = GetSerializableMembers(t, Version);
                    data = FormatterServices.GetObjectData(c, members.ToArray());
                    foreach (var d in data)
                    {
                        SerializeMember(pStream, d, pObjects);
                    }
                }
            }
        }

        private void SerializeMember(Stream pStream, object pObject, in IList<IGameObject> pObjects)
        {
            SerializedDataType dataType;
            if (pObject is IGameObject go)
            {
                //Only write the index of the GO, not the reference
                if (pObject == null) pObject = -1;
                else pObject = pObjects.IndexOf(go);

                dataType = SerializedDataType.GameObject;
            }
            else if (pObject == null) dataType = SerializedDataType.Null;
            else dataType = SerializedDataType.Object;

            pStream.WriteByte((byte)dataType);
            if (pObject != null) _formatter.Serialize(pStream, pObject);
        }
        #endregion

        #region Deserialization
        public IGameObject[] Deserialize(Stream pStream)
        {
            var version = pStream.ReadInt();
            if (version > Version) throw new InvalidOperationException("Unable to read a file with a higher version");

            var length = pStream.ReadInt();
            IGameObject[] deserialized = new IGameObject[length];

            for (int i = 0; i < length; i++)
            {
                var typeName = pStream.ReadString();
                var t = Type.GetType(typeName);

                //Create object and initialize various fields
                var go = (IGameObject)FormatterServices.GetUninitializedObject(t);
                CallOnDeserializeBegin(t, go);

                //Get index within the stream of this GO
                var index = pStream.ReadInt();
                var members = GetSerializableMembers(t, version);

                //Deserialize game object
                foreach (var m in members)
                {
                    DeserializeMember(pStream, m, go, deserialized);
                }

                //Get number of components
                var components = pStream.ReadInt();

                //Deserialize individual components
                for (int j = 0; j < components; j++)
                {
                    var componentType = Type.GetType(pStream.ReadString());
                    var c = (IComponent)FormatterServices.GetUninitializedObject(componentType);
                    CallOnDeserializeBegin(componentType, c);

                    members = GetSerializableMembers(componentType, version);
                    foreach (var m in members)
                    {
                        DeserializeMember(pStream, m, c, deserialized);
                    }
                    go.AddComponent(c);
                    CallOnDeserializeFinish(componentType, c);
                }

                CallOnDeserializeFinish(t, go);
                deserialized[index] = go;
            }

            return deserialized;
        }

        public void DeserializeIntoScene(Stream pStream, Scene pScene)
        {
            IGameObject[] deserialized = Deserialize(pStream);

            for (int i = 0; i < deserialized.Length; i++)
            {
                pScene.AddGameObject(deserialized[i]);
            }
        }

        private void DeserializeMember(Stream pStream, FieldInfo pMember, object pObject, IGameObject[] pObjects)
        {
            SerializedDataType dataType = (SerializedDataType)pStream.ReadByte();
            object value = null;
            switch (dataType)
            {
                case SerializedDataType.Null:
                    //Null object, nothing was written
                    return;

                case SerializedDataType.GameObject:
                    //The index of the game object was written instead of the data
                    var index = (int)_formatter.Deserialize(pStream);
                    if (index >= 0) value = pObjects[index];
                    else value = null;
                    break;

                case SerializedDataType.Object:
                    value = _formatter.Deserialize(pStream);
                    break;
            }

            pMember.SetValue(pObject, value);
        }
        #endregion

        private void CallOnDeserializeBegin(Type pType, object pInstance)
        {
            if(!_beginDeserialize.ContainsKey(pType))
            {
                _beginDeserialize.Add(pType, null);
                foreach (var m in pType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (m.GetCustomAttribute(SerializationUtils.DeserializeBeginType, true) != null)
                    {
                        _beginDeserialize[pType] = m;
                        break;
                    }
                }
            }

            var method = _beginDeserialize[pType];
            if (method != null) method.Invoke(pInstance, new object[] { });
        }

        private void CallOnDeserializeFinish(Type pType, object pInstance)
        {
            if (!_finishDeserialize.ContainsKey(pType))
            {
                _finishDeserialize.Add(pType, null);
                foreach (var m in pType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (m.GetCustomAttribute(SerializationUtils.DeserializeFinishType, true) != null)
                    {
                        _finishDeserialize[pType] = m;
                        break;
                    }
                }
            }

            var method = _finishDeserialize[pType];
            if (method != null) method.Invoke(pInstance, new object[] { });
        }

        private List<FieldInfo> GetSerializableMembers(Type pType, int pVersion)
        {
            var nullableType = Nullable.GetUnderlyingType(pType);
            if (nullableType != null) pType = nullableType;

            List<FieldInfo> members;
            if (!_typeCache.ContainsKey(pType))
            {
                //Cache members
                members = new List<FieldInfo>();

                List<FieldInfo> fields = new List<FieldInfo>();
                var t = pType;
                while (t != null)
                {
                    fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                    t = t.BaseType;
                }

                foreach (var f in fields)
                {
                    if (ShouldBeSerialized(f, pVersion)) members.Add(f);
                }
                _typeCache.Add(pType, members);
            }
            else
            {
                members = _typeCache[pType];
            }

            return members;
        }

        private bool ShouldBeSerialized(FieldInfo pField, int pVersion)
        {
            var isSerialized = pField.GetCustomAttribute(SerializationUtils.NonSerializedType, true) == null &&
                               pField.GetCustomAttribute(SerializationUtils.ImportComponentType, true) == null;
            var isNotUiElement = !typeof(SmallEngine.UI.UIElement).IsAssignableFrom(pField.FieldType);

            bool versionValid = true;
            var version = pField.GetCustomAttribute<FileVersionAttribute>(true);
            if (version != null)
            {
                versionValid = pVersion >= version.MinVersion && pVersion <= version.MaxVersion;
            }
            return versionValid && isSerialized && !isNotUiElement;
        }
    }
}
