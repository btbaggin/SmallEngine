using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Components
{
    public interface IComponent : IDisposable, IComparable<IComponent>
    {
        bool Active { get; set; }
        IGameObject GameObject { get; }
        void OnAdded(IGameObject pGameObject);
        void OnRemoved();
        void OnActiveChanged(bool pActive);
    }
}
