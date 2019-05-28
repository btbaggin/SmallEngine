﻿using System;
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
        public PhysicsSystem()
        {
            Scene.Register(this);
        }

        public void CreateQuadTree()
        {
            _quadTree = new QuadTree<ColliderComponent>(PhysicsHelper.WorldBounds);
        }

        protected override List<IComponent> DiscoverComponents(string pTemplate, IGameObject pObject)
        {
            var r = pObject.GetComponent(typeof(ColliderComponent));

            List<IComponent> components = new List<IComponent>();
            if (r != null)
            {
                components.Add(r);
                Remember(pTemplate, r.GetType());
            }

            return components;
        }

        public override void RunUpdate(float pDeltaTime)
        {
            _quadTree.Clear();

            foreach(var component in Components)
            {
                var r = (ColliderComponent)component;

                //Find all intersections before we insert our new entity
                foreach (var colliders in _quadTree.Retrieve(r))
                {
                    if(r.HasLayer(colliders.Layer))
                    {
                        Manifold m = new Manifold(r, colliders);

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
                }
                
                _quadTree.Insert(r);
                r.Update(pDeltaTime);
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
