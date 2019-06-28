using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Input;
using SmallEngine.Messages;
using SmallEngine.Graphics;
using SmallEngine.Components;
using System.Runtime.Serialization;

namespace SmallEngine
{
    public interface IGameObject : IMessageReceiver, IDisposable, ISerializable
    {
        //TODO? http://archive.gamedev.net/archive/reference/programming/features/scenegraph/page2.html
        #region Properties
        string Name { get; set; }

        Vector2 Position { get; set; }

        Size Scale { get; set; }

        float Rotation { get; set; }

        Mathematics.Matrix2X2 RotationMatrix { get; set; }

        Mathematics.Matrix3X2 TransformMatrix { get; set; }

        string Tag { get; set; }

        Scene ContainingScene { get; set; }

        bool Destroyed { get; }

        int Index { get; set; }

        ushort Version { get; set; }
        #endregion

        //TODO sleep/awake?

        IComponent GetComponent(Type pType);
        IEnumerable<IComponent> GetComponents();
        bool HasComponent<T>() where T : class, IComponent;

        void AddComponent(IComponent pComponent);
        void RemoveComponent(Type pComponent);

        void Initialize();
        void Update(float pDeltaTime);
        void Destroy();

        long GetPointer();
    }
}
