using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace SmallEngine.Physics
{
    public class World
    {
        private CollisionDetection _detection;

        private float angularDamping = 0.85f;
        private float linearDamping = 0.85f;

        private HashSet<RigidBodyComponent> _colliders = new HashSet<RigidBodyComponent>();

        public Vector2 Gravity { get; set; } = new Vector2(0, -9.81f);

        private static World _instance;
        public static World Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new World();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Create a new instance of the <see cref="World"/> class.
        /// </summary>
        /// <param name="collision">The collisionSystem which is used to detect
        /// collisions. See for example: <see cref="CollisionSystemSAP"/>
        /// or <see cref="CollisionSystemBrute"/>.
        /// </param>
        public World()
        {
            _detection = new CollisionDetection();
            _instance = this;
            //this.CollisionSystem.CollisionDetected += collisionDetectionHandler;
        }

        /// <summary>
        /// Removes all objects from the world and removes all memory cached objects.
        /// </summary>
        public void Clear()
        {
            // remove bodies from collision system
            foreach (RigidBodyComponent body in _colliders)
            {
                _detection.RemoveEntity(body);
            }

            // remove bodies from the world
            _colliders.Clear();
        }


        public void SetDampingFactors(float angularDamping, float linearDamping)
        {
            if (angularDamping < 0.0f || angularDamping > 1.0f)
                throw new ArgumentException("Angular damping factor has to be between 0.0 and 1.0", "angularDamping");

            if (linearDamping < 0.0f || linearDamping > 1.0f)
                throw new ArgumentException("Linear damping factor has to be between 0.0 and 1.0", "linearDamping");

            this.angularDamping = angularDamping;
            this.linearDamping = linearDamping;
        }

        /// <summary>
        /// Removes a <see cref="RigidBody"/> from the world.
        /// </summary>
        /// <param name="body">The body which should be removed.</param>
        /// <returns>Returns false if the body could not be removed from the world.</returns>
        public bool RemoveCollider(RigidBodyComponent body)
        {
            return _colliders.Remove(body);
        }

        /// <summary>
        /// Adds a <see cref="RigidBody"/> to the world.
        /// </summary>
        /// <param name="body">The body which should be added.</param>
        public void AddBody(RigidBodyComponent body)
        {
            if (body == null) throw new ArgumentNullException("body", "body can't be null.");
            if (!_colliders.Contains(body))
            {
                _colliders.Add(body);
            }

            _detection.AddEntity(body);
        }

        private float currentLinearDampFactor = 1.0f;
        private float currentAngularDampFactor = 1.0f;

        public void Update(float timestep)
        {
            // yeah! nothing to do!
            if (timestep == 0.0f) return;

            // throw exception if the timestep is smaller zero.
            if (timestep < 0.0f) throw new ArgumentException("The timestep can't be negative.", "timestep");

            // Calculate this
            currentAngularDampFactor = (float)Math.Pow(angularDamping, timestep);
            currentLinearDampFactor = (float)Math.Pow(linearDamping, timestep);

            foreach (RigidBodyComponent col in _colliders)
            {
                col.PreStep(timestep);
            }

            _detection.Detect();

            //Integrate
            Threading.JobManager.Instance.BeginBatch();
            foreach (RigidBodyComponent body in _colliders.Where((pR) => !pR.Active))
            {
                Threading.JobManager.Instance.Queue(IntegrateCallback, body);
            }
            Threading.JobManager.Instance.ExecuteBatch();

            foreach(RigidBodyComponent col in _colliders)
            {
                col.PostStep();
            }
        }

        private void IntegrateCallback(object o)
        {
            //RigidBody body = obj as RigidBody;

            //JVector temp;
            //JVector.Multiply(ref body.linearVelocity, timestep, out temp);
            //JVector.Add(ref temp, ref body.position, out body.position);

            //if (!(body.isParticle))
            //{

            //    //exponential map
            //    JVector axis;
            //    float angle = body.angularVelocity.Length();

            //    if (angle < 0.001f)
            //    {
            //        // use Taylor's expansions of sync function
            //        // axis = body.angularVelocity * (0.5f * timestep - (timestep * timestep * timestep) * (0.020833333333f) * angle * angle);
            //        JVector.Multiply(ref body.angularVelocity, (0.5f * timestep - (timestep * timestep * timestep) * (0.020833333333f) * angle * angle), out axis);
            //    }
            //    else
            //    {
            //        // sync(fAngle) = sin(c*fAngle)/t
            //        JVector.Multiply(ref body.angularVelocity, ((float)Math.Sin(0.5f * angle * timestep) / angle), out axis);
            //    }

            //    JQuaternion dorn = new JQuaternion(axis.X, axis.Y, axis.Z, (float)Math.Cos(angle * timestep * 0.5f));
            //    JQuaternion ornA; JQuaternion.CreateFromMatrix(ref body.orientation, out ornA);

            //    JQuaternion.Multiply(ref dorn, ref ornA, out dorn);

            //    dorn.Normalize(); JMatrix.CreateFromQuaternion(ref dorn, out body.orientation);
            //}

            //if ((body.Damping & RigidBody.DampingType.Linear) != 0)
            //    JVector.Multiply(ref body.linearVelocity, currentLinearDampFactor, out body.linearVelocity);

            //if ((body.Damping & RigidBody.DampingType.Angular) != 0)
            //    JVector.Multiply(ref body.angularVelocity, currentAngularDampFactor, out body.angularVelocity);

            //body.Update();


            //if (CollisionSystem.EnableSpeculativeContacts || body.EnableSpeculativeContacts)
            //    body.SweptExpandBoundingBox(timestep);
        }
    }
}
