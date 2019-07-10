using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine.Physics
{
    [Serializable]
    public sealed class ColliderComponent : DependencyComponent, IPhysicsBody
    {
        [NonSerialized] Dictionary<ColliderComponent, bool> Colliders;
        [field: NonSerialized] public EventHandler<CollisionBeginEventArgs> CollisionEnter { get; set; }
        [field: NonSerialized] public EventHandler<CollisionBeginEventArgs> CollisionStay { get; set; }
        [field: NonSerialized] public EventHandler<CollisionEventArgs> CollisionExit { get; set; }
        [field: NonSerialized] public EventHandler<CollisionBeginEventArgs> TriggerEnter { get; set; }
        [field: NonSerialized] public EventHandler<CollisionBeginEventArgs> TriggerStay { get; set; }
        [field: NonSerialized] public EventHandler<CollisionEventArgs> TriggerExit { get; set; }

        #region Properties
        [field: NonSerialized]
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

                if(_body != null)
                {
                    _body.Mass = value.Mass;
                    _body.Inertia = value.Inertia;
                }
            }
        }

        [ImportComponent(false)][NonSerialized]
        RigidBodyComponent _body;
        public RigidBodyComponent Body
        {
            get { return _body; }
        }

        public short Layer { get; set; }

        public bool IsTrigger { get; set; }

        [NonSerialized] bool _triggerEnter;
        [NonSerialized] bool _triggerExit;
        public bool TriggerOnlyOnce { get; set; }
        #endregion

        #region Constructors
        public ColliderComponent() : base() { }

        public ColliderComponent(CollisionMesh pMesh) : base()
        {
            Mesh = pMesh;
        }
        #endregion

        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);
            Colliders = new Dictionary<ColliderComponent, bool>();

            if (Mesh != null && _body != null)
            {
                _body.Mass = Mesh.Mass;
                _body.Inertia = Mesh.Inertia;
            }
        }

        internal void UpdateAABB()
        {
            AABB = Mesh.CalculateAABB(Position);
        }

        internal void Update(float pDeltaTime)
        {
            _body?.Update(pDeltaTime);
        }

        internal bool OnCollisionEnter(ColliderComponent pCollider, Manifold pManifold)
        {
            bool _event = false;
            bool isTrigger = IsTrigger || pCollider.IsTrigger;
            if (isTrigger)
            {
                //Check if we have already triggered this collider
                if (!TriggerOnlyOnce || !_triggerEnter)
                {
                    _event = true;
                    _triggerEnter = true;
                }
            }
            else _event = true;

            bool resolve = true;
            if (_event)
            {
                EventHandler<CollisionBeginEventArgs> ce = null;
                if (!Colliders.ContainsKey(pCollider))
                {
                    //By default we will collide with everything
                    Colliders.Add(pCollider, false);

                    if (isTrigger) ce = TriggerEnter;
                    else ce = CollisionEnter;
                }
                else if (!Colliders[pCollider])
                {
                    if (isTrigger) ce = TriggerStay;
                    else ce = CollisionStay;
                }
                else
                {
                    //We have this collider and we are ignoring it
                    resolve = false;
                }

                if (ce != null)
                {
                    var args = new CollisionBeginEventArgs(pCollider, pManifold);
                    ce.Invoke(this, args);

                    //Cancelling the collision means we ignore this collider
                    Colliders[pCollider] = args.Cancel;
                    return !args.Cancel;
                }
            }

            return resolve;
        }

        internal void OnCollisionExit(ColliderComponent pCollider, Manifold pManifold)
        {
            //TODO check ignore?
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
