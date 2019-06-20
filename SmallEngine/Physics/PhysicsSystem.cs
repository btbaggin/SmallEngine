using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine.Physics
{
    class PhysicsSystem : ComponentSystem
    {
        public delegate bool CollisionResolutionDelegate(ref Manifold pM);

        readonly CollisionResolutionDelegate[,] _resolvers = { { CollisionDetection.CircleVsCircle, CollisionDetection.CirclevsPolygon },
                                                               { CollisionDetection.PolygonvsCircle, CollisionDetection.PolygonvsPolygon } };

        QuadTree<ColliderComponent> _quadTree;
        public void CreateQuadTree()
        {
            _quadTree = new QuadTree<ColliderComponent>(PhysicsHelper.WorldBounds);
        }

        protected override List<IComponent> DiscoverComponents(IGameObject pObject)
        {
            var r = pObject.GetComponent(typeof(ColliderComponent));

            List<IComponent> components = new List<IComponent>();
            if (r != null) components.Add(r);

            return components;
        }

        protected override void DoProcess()
        {
            _quadTree.Clear();
            var deltaTime = GameTime.PhysicsTime;

            foreach(var component in Components)
            {
                var r = (ColliderComponent)component;
                if (!r.Active) continue;

                //We have to update the AABB here because otherwise our bounding boxes will be one frame behind
                r.UpdateAABB();

                //Find all intersections before we insert our new entity
                foreach (var colliders in _quadTree.Retrieve(r))
                {
                    Manifold m = new Manifold(r, colliders);
                    if(colliders.Layer != 0 && r.HasLayer(colliders.Layer))
                    {

                        int rShape = (int)r.Mesh.Shape;
                        int cShape = (int)colliders.Mesh.Shape;
                        bool resolve = _resolvers[rShape, cShape].Invoke(ref m);

                        if (resolve)
                        {
                            m.Resolve();
                            m.CorrectPositions();

                            m.BodyA.OnCollisionEnter(m.BodyB, m);
                            m.BodyB.OnCollisionEnter(m.BodyA, m);
                        }
                        else
                        {
                            m.BodyA.OnCollisionExit(m.BodyB, m);
                            m.BodyB.OnCollisionExit(m.BodyA, m);
                        }
                    }
                    else
                    {
                        m.BodyA.OnCollisionExit(m.BodyB, m);
                        m.BodyB.OnCollisionExit(m.BodyA, m);
                    }
                }
                
                _quadTree.Insert(r);
                r.Update(deltaTime);
            }
        }

        internal ColliderComponent HitTest(Vector2 pPoint)
        {
            foreach(var c in _quadTree.Retrieve(pPoint))
            {
                if (c.Mesh.Contains(pPoint - c.AABB.Center)) return c;
            }

            return null;
        }
    }
}
