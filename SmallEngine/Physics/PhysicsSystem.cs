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
        public static int WorldSize { get; set; } = Int16.MaxValue;

        public delegate bool CollisionResolutionDelegate(ref Manifold pM);

        readonly CollisionResolutionDelegate[,] _resolvers = { { CollisionDetection.CircleVsCircle, CollisionDetection.CirclevsPolygon },
                                                               { CollisionDetection.PolygonvsCircle, CollisionDetection.PolygonvsPolygon } };
        public PhysicsSystem()
        {
            Scene.Register(this);
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
            var qt = new QuadTree<RigidBodyComponent>(new Rectangle(0, 0, WorldSize, WorldSize));
            foreach(var component in Components)
            {
                var r = (RigidBodyComponent)component;

                //Find all intersections before we insert our new entity
                foreach (var colliders in qt.Retrieve(r))
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

                            m.BodyA.OnCollisionOccurred(m.BodyB ,true);
                            m.BodyB.OnCollisionOccurred(m.BodyA, false);
                        }
                    }
                }
                
                qt.Insert(r);
                r.Update(pDeltaTime);

            }
        }
    }
}
