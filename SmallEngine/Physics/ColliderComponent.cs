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
        public EventHandler<CollisionEventArgs> CollisionStay { get; set; }
        public EventHandler<CollisionEventArgs> CollisionExit { get; set; }
        public EventHandler<CollisionEventArgs> TriggerEnter { get; set; }
        public EventHandler <CollisionEventArgs> TriggerStay { get; set; }
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

        public short Layer { get; set; }

        public bool IsTrigger { get; set; }

        bool _triggerEnter;
        bool _triggerExit;
        public bool TriggerOnlyOnce { get; set; }

        internal void UpdateAABB()
        {
            AABB = Mesh.CalculateAABB(Position);
        }

        internal void Update(float pDeltaTime)
        {
            _body?.Update(pDeltaTime);
        }

        internal void OnCollisionEnter(ColliderComponent pCollider, Manifold pManifold)
        {
            bool _event = false;
            if (IsTrigger)
            {
                //Check if we have already triggered this collider
                if (!TriggerOnlyOnce || !_triggerEnter)
                {
                    _event = true;
                    _triggerEnter = true;
                }
            }
            else _event = true;

            if (_event)
            {
                EventHandler<CollisionEventArgs> ce = null;
                if (Colliders.Add(pCollider))
                {
                    if (IsTrigger) ce = TriggerEnter;
                    else ce = CollisionEnter;
                }
                else
                {
                    if (IsTrigger) ce = TriggerStay;
                    else ce = CollisionStay;
                }

                if(ce != null) ce.Invoke(this, new CollisionEventArgs(pCollider, pManifold));
            }
        }

        internal void OnCollisionExit(ColliderComponent pCollider, Manifold pManifold)
        {
            if(Colliders.Remove(pCollider))
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

                ce?.Invoke(this, new CollisionEventArgs(pCollider, pManifold));
            }
        }

        public bool HasLayer(short pLayer)
        {
            return (Layer & pLayer) != 0;
        }
    }
}
