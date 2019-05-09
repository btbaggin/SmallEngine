﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine.Graphics
{
    class RenderSystem : ComponentSystem
    {
        readonly IGraphicsAdapter _adapter;
        public RenderSystem(IGraphicsAdapter pAdapter)
        {
            _adapter = pAdapter;
            Scene.Register(this);
        }

        public override void Update(float pDeltaTime)
        {
            foreach(var c in Components)
            {
                var r = (RenderComponent)c;
                if(r.IsVisible())
                {
                    r.Draw(_adapter, pDeltaTime);
                }
            }
        }

        protected override List<IComponent> DiscoverComponents(string pTemplate, IGameObject pObject)
        {
            var r = pObject.GetComponentOfType(typeof(RenderComponent));

            List<IComponent> components = new List<IComponent>();
            if (r != null)
            {
                components.Add(r);
                Remember(pTemplate, r.GetType());
            }

            return components;
        }
    }
}