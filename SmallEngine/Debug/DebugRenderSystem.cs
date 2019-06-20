using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;
using SmallEngine.Graphics;
using SmallEngine.Physics;

namespace SmallEngine.Debug
{
    public class DebugRenderSystem : ComponentSystem
    {
        public Color DebugColor
        {
            get { return _pen.Color; }
            set { _pen.Color = value; }
        }

        readonly Pen _pen;
        readonly Brush _brush;
        readonly IGraphicsAdapter _adapter;
        public DebugRenderSystem(IGraphicsAdapter pAdapter)
        {
            _adapter = pAdapter;
            _pen = Pen.Create(Color.Aqua, 1, _adapter);
            _brush = Brush.CreateOutlineBrush(Color.Aqua, 1, _adapter);
        }

        protected override List<IComponent> DiscoverComponents(IGameObject pObject)
        {
            var r = pObject.GetComponent(typeof(ColliderComponent));
            var c = new List<IComponent>();
            if (r != null) c.Add(r);
            return c;
        }

        protected override void DoProcess()
        {
            foreach(var c in Components)
            {
                var collider = (ColliderComponent)c;
                _adapter.SetTransform(Transform.CreateBasic(collider.GameObject));
                switch (collider.Mesh.Shape)
                {
                    case Shapes.Circle:
                        var cir = (CircleMesh)collider.Mesh;
                        _adapter.DrawElipse(collider.AABB.Center, cir.Radius, _brush);
                        break;

                    case Shapes.Polygon:
                        var p = (PolygonMesh)collider.Mesh;
                        var pos = collider.AABB.Center;
                        for (int i = 0; i < p.Verticies.Length - 1; i++)
                        {
                            var v1 = p.Verticies[i] + pos;
                            var v2 = p.Verticies[i + 1] + pos;
                            _adapter.DrawLine(v1, v2, _pen);
                        }
                        _adapter.DrawLine(p.Verticies[p.Verticies.Length - 1] + pos, p.Verticies[0] + pos, _pen);
                        break;
                }
            }
        }
    }
}
