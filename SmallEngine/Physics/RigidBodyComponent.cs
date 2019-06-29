using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine.Physics
{
    [Serializable]
    public sealed class RigidBodyComponent : DependencyComponent
    {
        #region "Properties"
        internal float InverseMass;
        internal float InverseInertia;

        private float _mass;
        public float Mass
        {
            get { return _mass; }
            internal set
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

        public float AngularVelocity { get; private set; }

        public float Torque { get; internal set; }

        public bool IsKinematic { get; set; }
        #endregion

        #region Constructor
        public RigidBodyComponent() : base() { }

        public RigidBodyComponent(bool pIsKinematic) : base()
        {
            IsKinematic = pIsKinematic;
        }
        #endregion

        public void MoveBody(Vector2 pAmount)
        {
            GameObject.Position += pAmount;
        }

        internal void Update(float pDeltaTime)
        {
            if (Mass != 0)
            {
                MoveBody(Velocity * pDeltaTime);

                if(!IsKinematic)
                {
                    Velocity += (InverseMass * Force + PhysicsHelper.Gravity) * pDeltaTime;
                    AngularVelocity += Torque * InverseInertia * pDeltaTime;
                    GameObject.Rotation += AngularVelocity * pDeltaTime;
                }

                Force = Vector2.Zero;
                Torque = 0;
            }
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
    }
}
