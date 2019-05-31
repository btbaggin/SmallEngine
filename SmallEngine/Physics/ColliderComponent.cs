using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine.Physics
{
    public sealed class ColliderComponent : DependencyComponent, IPhysicsBody
    {
        readonly  HashSet<ColliderComponent> Colliders = new HashSet<ColliderComponent>();
        public EventHandler<CollisionEventArgs> CollisionEnter { get; set; }
        public EventHandler<CollisionEventArgs> CollisionExit { get; set; }
        public EventHandler<CollisionEventArgs> TriggerEnter { get; set; }
        public EventHandler<CollisionEventArgs> TriggerExit { get; set; }

        public AxisAlignedBoundingBox AABB { get; private set; }

        public Vector2 Position
        {
            get { return GameObject.Position; }
        }

        CollisionMesh _mesh;
        public CollisionMesh Mesh
        {
            get { return _mesh; }
            set
            {
                _mesh = value;
                _mesh.Body = this;

                _mesh.CalculateMass(out float mass, out float inertia);
                if(_body != null)
                {
                    _body.Mass = mass;
                    _body.Inertia = inertia;
                }
            }
        }

        [ImportComponent(false)]
        RigidBodyComponent _body;
        public RigidBodyComponent Body
        {
            get { return _body; }
        }

        public short Layer { get; private set; }

        public bool IsTrigger { get; set; }

        bool _triggerEnter;
        bool _triggerExit;
        public bool TriggerOnlyOnce { get; set; }

        public ColliderComponent()
        {
            Layer = 1;
        }

        internal void Update(float pDeltaTime)
        {
            AABB = Mesh.CalculateAABB(Position);

            _body?.Update(pDeltaTime);
        }

        internal void OnCollisionEnter(ColliderComponent pCollider, Manifold pManifold)
        {
            EventHandler<CollisionEventArgs> ce = null;
            if (IsTrigger)
            {
                //Check if we have already triggered this collider
                if(!TriggerOnlyOnce || !_triggerEnter)
                {
                    ce = TriggerEnter;
                    _triggerEnter = true;
                }
            }
            else ce = CollisionEnter;

            if (ce != null && Colliders.Add(pCollider))
            {
                ce.Invoke(this, new CollisionEventArgs(pCollider, pManifold));
            }
        }

        internal void OnCollisionExit(ColliderComponent pCollider, Manifold pManifold)
        {
            EventHandler<CollisionEventArgs> ce = null;
            if (IsTrigger)
            {
                //Check if we have already triggered this collider
                if (!TriggerOnlyOnce || !_triggerExit)
                {
                    ce = TriggerExit;
                    _triggerExit = true;
                }
            }
            else ce = CollisionExit;

            if (ce != null && Colliders.Remove(pCollider))
            {
                ce.Invoke(this, new CollisionEventArgs(pCollider, pManifold));
            }
        }

        public void AddToLayer(short pLayer)
        {
            Layer |= pLayer;
        }

        public void RemoveFromLayer(short pLayer)
        {
            Layer &= (short)~(int)pLayer;
        }

        public bool HasLayer(short pLayer)
        {
            return (Layer & pLayer) != 0;
        }
    }
}
