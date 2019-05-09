using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public class RigidBodyComponent : Component, IPhysicsBody
    {
        private List<RigidBodyComponent> Colliders = new List<RigidBodyComponent>();
        public EventHandler<CollisionEventArgs> CollisionEnter { get; set; }
        public EventHandler<CollisionEventArgs> CollisionExit { get; set; }

        #region "Properties"
        internal float InverseMass;
        internal float InverseInertia;

        private float _mass;
        public float Mass
        {
            get { return _mass; }
            private set
            {
                _mass = value;
                if (_mass == 0) InverseMass = 0;
                else InverseMass = 1 / _mass;
            }
        }

        private float _inertia;
        public float Inertia
        {
            get { return _inertia; }
            set
            {
                _inertia = value;
                if (_inertia == 0) InverseInertia = 0;
                else InverseInertia = 1 / _inertia;
            }
        }

        public Vector2 Position
        {
            get { return GameObject.Position; }
        }

        public Vector2 Velocity { get; set; }

        public Vector2 Force { get; private set; }

        public float Orientation { get; private set; }

        public float AngularVelocity { get; private set; }

        public float Torque { get; internal set; }

        CollisionMesh _mesh;
        public CollisionMesh Mesh
        {
            get { return _mesh; }
            set
            {
                _mesh = value;
                _mesh.Body = this;

                _mesh.CalculateMass(out float mass, out float inertia);
                Mass = mass;
                Inertia = inertia;
            }
        }

        public AxisAlignedBoundingBox AABB { get; private set; }

        public short Layer { get; private set; }

        public Matrix2X2 OrientationMatrix { get; private set; }

        public bool IsKinematic { get; set; }
        #endregion

        public RigidBodyComponent()
        {
            OrientationMatrix = Matrix2X2.Identity;
            Layer = 1;
        }

        public void MoveBody(Vector2 pAmount)
        {
            GameObject.Position += pAmount;
        }

        internal void Update(float pDeltaTime)
        {
            if (Mass != 0)
            {
                MoveBody(Velocity * pDeltaTime);

                if(IsKinematic)
                {
                    Velocity += PhysicsHelper.Gravity * (pDeltaTime / 2);
                }
                else
                {
                    Velocity += (InverseMass * Force + PhysicsHelper.Gravity) * (pDeltaTime / 2);
                    AngularVelocity += Torque * InverseInertia * (pDeltaTime / 2);
                    Orientation += AngularVelocity * pDeltaTime;
                    OrientationMatrix = new Matrix2X2(Orientation);
                }

                Force = Vector2.Zero;
                Torque = 0;
            }

            AABB = Mesh.CalculateAABB(Position);
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

        public void ApplyImpulse(Vector2 pImpulse, Vector2 pContact)
        {
            Velocity += InverseMass * pImpulse;
            AngularVelocity += InverseInertia * Vector2.CrossProduct(pContact, pImpulse);
        }

        public void ApplyForce(Vector2 pForce)
        {
            Force += pForce;
        }

        public void ApplyTorque(float pTorque)
        {
            Torque += pTorque;
        }

        public Graphics.Transform CreateTransform()
        {
            return OrientationMatrix.ToTransform(AABB.Center);
        }

        internal void OnCollisionEnter(RigidBodyComponent pCollider, bool pSource)
        {
            var ce = CollisionEnter;
            if(ce != null && !Colliders.Contains(pCollider))
            {
                Colliders.Add(pCollider);
                ce.Invoke(this, new CollisionEventArgs(pCollider, pSource));
            }
        }

        internal void OnCollisionExit(RigidBodyComponent pCollider, bool pSource)
        {
            var ce = CollisionExit;
            if(ce != null && Colliders.Remove(pCollider))
            {
                ce.Invoke(this, new CollisionEventArgs(pCollider, pSource));
            }
        }
    }
}
