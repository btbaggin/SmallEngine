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
            //It's ok to just go to the initial count
            //because any components added during the update should be updated until the next frame anyway
            var count = Components.Count;
            for(int i = 0; i < count; i++)
            {
                var u = (BehaviorComponent)Components[i];
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
