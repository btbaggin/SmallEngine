using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace Evolusim
{
    class ToolbarComponent : DependencyComponent
    {
        [ImportComponent(false, true)]
        private RenderComponent _render = null;

        public override void Update(float pDeltaTime)
        {
            if (InputManager.KeyPressed(Mouse.Left) && InputManager.IsFocused(_render))
            {
                MessageBus.SendMessage(new GameMessage("ToolbarOpen", this));
            }
        }
    }
}
