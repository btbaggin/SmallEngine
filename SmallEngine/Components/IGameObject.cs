using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Input;
using SmallEngine.Messages;

namespace SmallEngine
{
    public interface IGameObject : IMessageReceiver, IDisposable
    {
        //TODO? http://archive.gamedev.net/archive/reference/programming/features/scenegraph/page2.html
        #region Properties
        string Name { get; set; }

        Vector2 Position { get; set; }

        Vector2 Scale { get; set; }

        float Rotation { get; set; }

        string Tag { get; set; }

        Scene ContainingScene { get; set; }

        bool Destroyed { get; }
        #endregion

        IComponent GetComponentOfType(Type pType);
        IComponent GetComponent(Type pType);
        IEnumerable<IComponent> GetComponents();
        bool HasComponent<T>() where T : class, IComponent;

        void AddComponent(IComponent pComponent);
        void RemoveComponent(Type pComponent);

        void Initialize();
        void Update(float pDeltaTime);
        void Destroy();
    }
}
