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
        public Pen DebugBoxes { get; set; }

        readonly IGraphicsAdapter _adapter;
        public DebugRenderSystem(IGraphicsAdapter pAdapter) : base(typeof(ColliderComponent))
        {
            _adapter = pAdapter;
            DebugBoxes = Pen.Create(Color.Aqua, 1);
        }

        public override void Process()
        {
            foreach(var c in Components)
            {
                var collider = (ColliderComponent)c;
                _adapter.SetTransform(Transform.CreateBasic(collider.GameObject));
                switch (collider.Mesh.Shape)
                {
                    case Shapes.Circle:
                        var cir = (CircleMesh)collider.Mesh;
                        var center = Game.ActiveCamera.ToCameraSpace(collider.AABB.Center);
                        _adapter.DrawElipseOutline(center, cir.Radius, DebugBoxes);
                        break;

                    case Shapes.Polygon:
                        var p = (PolygonMesh)collider.Mesh;
                        var pos = collider.AABB.Center;
                        Vector2 v1;
                        Vector2 v2;
                        for (int i = 0; i < p.Vertices.Length - 1; i++)
                        {
                            v1 = Game.ActiveCamera.ToCameraSpace(p.Vertices[i] + pos);
                            v2 = Game.ActiveCamera.ToCameraSpace(p.Vertices[i + 1] + pos);
                            _adapter.DrawLine(v1, v2, DebugBoxes);
                        }
                        v1 = Game.ActiveCamera.ToCameraSpace(p.Vertices[p.Vertices.Length - 1] + pos);
                        v2 = Game.ActiveCamera.ToCameraSpace(p.Vertices[0] + pos);

                        _adapter.DrawLine(v1, v2, DebugBoxes);
                        break;
                }
            }
        }
    }
}
