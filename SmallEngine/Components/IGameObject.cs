using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public interface IGameObject : IDisposable
    {
        #region "Properties"
        string Name { get; }

        Vector2 Position { get; set; }

        Vector2 Scale { get; set; }

        float Rotation { get; set; }

        RectangleF Bounds { get; }

        bool Persistant { get; set; }
        #endregion

        T GetComponent<T>() where T : class, IComponent;
        IComponent GetComponent(Type pType);
        IEnumerable<IComponent> GetComponents();
        bool HasComponent<T>() where T : class, IComponent;

        void AddComponent(IComponent pComponent);
        void RemoveComponent(Type pComponent);

        void SetGame(Game pGame);
        void Initialize();
        void PreUpdate();
        void Update(float pDeltaTime);
        void Destroy();
    }
}
