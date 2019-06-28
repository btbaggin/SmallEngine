using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine
{
    public abstract class UpdateComponent : DependencyComponent, IUpdatable
    {
        //All inherited members should register as update components so only 1 system needs to handle them
        protected override Type RegistrationType => typeof(UpdateComponent);

        public abstract void Update(float pDeltaTime);
    }
}
