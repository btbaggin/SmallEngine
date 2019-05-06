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
        public EventHandler<CollisionEventArgs> CollisionOccurred { get; set; }

        #region "Properties"
        internal bool Collided;
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

        public byte Layer { get; private set; }

        public Matrix2X2 OrientationMatrix { get; private set; }
        #endregion

        public RigidBodyComponent()
        {
            OrientationMatrix = Matrix2X2.Identity;
        }

        internal void MoveBody(Vector2 pAmount)
        {
            GameObject.Position += pAmount;
        }

        internal void Update(float pDeltaTime)
        {
            if (Mass != 0)
            {
                MoveBody(Velocity * pDeltaTime);
                Orientation += AngularVelocity * pDeltaTime;
                OrientationMatrix = new Matrix2X2(Orientation);

                Velocity += (InverseMass * Force + PhysicsParameters.Gravity) * (pDeltaTime / 2);
                AngularVelocity += Torque * InverseInertia * (pDeltaTime / 2);
                Force = Vector2.Zero;
                Torque = 0;
            }

            AABB = Mesh.CalculateAABB(Position);
        }

        public void AddToLayer(byte pLayer)
        {
            Layer |= pLayer;
        }

        public void RemoveFromLayer(byte pLayer)
        {
            Layer &= (byte)~(int)pLayer;
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

        internal void OnCollisionOccurred(RigidBodyComponent pCollider, bool pSource)
        {
            if(!Collided)
            {
                CollisionOccurred?.Invoke(this, new CollisionEventArgs(pCollider, pSource));
            }
        }
    }
}
