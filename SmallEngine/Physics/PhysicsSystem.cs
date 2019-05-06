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

        QuadTree<RigidBodyComponent> _quadTree;
        public PhysicsSystem()
        {
            Scene.Register(this);
        }

        public void CreateQuadTree()
        {
            _quadTree = new QuadTree<RigidBodyComponent>(PhysicsParameters.WorldBounds);
        }

        protected override List<IComponent> DiscoverComponents(string pTemplate, IGameObject pObject)
        {
            var r = pObject.GetComponent(typeof(RigidBodyComponent));

            List<IComponent> components = new List<IComponent>();
            if (r != null)
            {
                components.Add(r);
                Remember(pTemplate, r.GetType());
            }

            return components;
        }

        public override void Update(float pDeltaTime)
        {
            _quadTree.Clear();

            foreach(var component in Components)
            {
                var r = (RigidBodyComponent)component;
                bool collided = false;

                //Find all intersections before we insert our new entity
                foreach (var colliders in _quadTree.Retrieve(r))
                {
                    if(r.Layer == colliders.Layer)
                    {
                        Manifold m = new Manifold(r, colliders);

                        int rShape = (int)r.Mesh.Shape;
                        int cShape = (int)colliders.Mesh.Shape;
                        bool resolve = _resolvers[rShape, cShape].Invoke(ref m);

                        if (resolve)
                        {
                            m.Resolve();
                            m.CorrectPositions();

                            collided = true;
                            m.BodyA.OnCollisionOccurred(m.BodyB, true);
                            m.BodyB.OnCollisionOccurred(m.BodyA, false);
                        }
                    }
                }
                
                _quadTree.Insert(r);
                r.Update(pDeltaTime);
                r.Collided = collided;
            }
        }
    }
}
