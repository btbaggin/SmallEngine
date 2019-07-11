using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine
{
    public class BehaviorSystem : ComponentSystem
    {
        public BehaviorSystem() : base(typeof(BehaviorComponent)) { }

        public override void Process()
        {
            throw new InvalidOperationException();
        }

        public void Update(float pDeltaTime)
        {
            foreach (var c in Components)//TODO improve? Can't use normal for loop because items could get inserted before i causing an infinite loop
            {
                var u = (BehaviorComponent)c;
                u.Update(pDeltaTime);
            }
        }

        public void Draw(SmallEngine.Graphics.IGraphicsAdapter pAdapter)
        {
            foreach (var c in Components)
            {
                var u = (BehaviorComponent)c;
                u.Draw(pAdapter);
            }
        }
    }
}
