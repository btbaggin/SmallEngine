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
        public UpdateSystem() : base(typeof(UpdateComponent)) { }

        public override void Process()
        {
            foreach(var c in Components.ToList())//TODO improve? Can't use normal for loop because items could get inserted before i causing an infinite loop
            {
                var u = (UpdateComponent)c;
                u.Update(GameTime.DeltaTime);
            }
        }
    }
}
