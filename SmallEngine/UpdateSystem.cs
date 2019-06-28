using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine
{
    public class UpdateSystem : ComponentSystem
    {
        public UpdateSystem()
        {
            Components = Component.GetComponentsOfType(typeof(UpdateComponent));
        }

        protected override void DoProcess()
        {
            foreach(var c in Components)
            {
                var u = (UpdateComponent)c;
                u.Update(GameTime.DeltaTime);
            }
        }
    }
}
