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
            for(int i = 0; i < Components.Count; i++)
            {
                var u = (UpdateComponent)Components[i];
                u.Update(GameTime.DeltaTime);
            }
        }
    }
}
