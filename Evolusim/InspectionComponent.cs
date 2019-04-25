using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;
using SmallEngine.Messages;

namespace Evolusim
{
    class InspectionComponent : DependencyComponent
    {
        [ImportComponent(false, true)]
        private RenderComponent _render = null;

        public override void Update(float pDeltaTime)
        {
            if (InputManager.KeyPressed(Mouse.Left) && InputManager.IsFocused(_render))
            {
                Game.Messages.SendMessage(new GameMessage("ToolbarOpen", GameObject));
            }
        }
    }
}
