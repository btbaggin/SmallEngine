using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine
{
    [Serializable]
    public abstract class BehaviorComponent : DependencyComponent, IUpdatable
    {
        //All inherited members should register as update components so only 1 system needs to handle them
        public override Type RegistrationType => typeof(BehaviorComponent);

        public virtual void Update(float pDeltaTime) { }

        public virtual void Draw(SmallEngine.Graphics.IGraphicsAdapter pAdapter) { } //TODO have render system handle this?

        protected BehaviorComponent() { }
    }
}
