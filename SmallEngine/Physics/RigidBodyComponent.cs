using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public class RigidBodyComponent : Component
    {
        public EventHandler<EventArgs> CollisionOccurred;

        #region "Properties"
        public bool Static { get; set; }

        public float Mass { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        public Shape ColliderShape { get; set; }

        public Material ColliderMaterial { get; set; }

        public Rectangle Bounds
        {
            get { return GameObject.Bounds; }
        }
        #endregion

        public override void OnAdded(IGameObject pGameObject)
        {
            World.Instance.AddBody(this);
            base.OnAdded(pGameObject);
        }

        public override void OnRemoved()
        {
            World.Instance.RemoveCollider(this);
            base.OnRemoved();
        }

        public override void OnActiveChanged(bool pActive)
        {
            if (Active) { World.Instance.AddBody(this); }
            else { World.Instance.RemoveCollider(this); }
        }

        internal void OnCollisionOccurred(RigidBodyComponent pCollider)
        {
            CollisionOccurred?.Invoke(this, new EventArgs());
        }

        public virtual void PreStep(float pDeltaTime)
        {
        }

        public virtual void PostStep()
        {
        }

        public RigidBodyComponent()
        {
            ColliderMaterial = new Material();
            ColliderShape = Shape.Circle;
            Static = false;
        }

        public void ApplyForce(Vector2 pForce)
        {
            Acceleration += pForce;
        }

        public override void Update(float pDeltaTime)
        {
            if (Static) { return; }

            Velocity += Acceleration;
            GameObject.Position += Velocity * pDeltaTime;
            Acceleration *= 1f;
            //Acceleration = Vector2.Zero; //TODO
            Velocity *= 0;
        }
    }
}
