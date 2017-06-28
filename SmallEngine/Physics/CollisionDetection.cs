using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmallEngine.Threading;

namespace SmallEngine.Physics
{
    class CollisionDetection
    {
        #region BroadphasePair class
        protected class BroadphasePair
        {
            /// <summary>
            /// The first body.
            /// </summary>
            public RigidBodyComponent Entity1;
            /// <summary>
            /// The second body.
            /// </summary>
            public RigidBodyComponent Entity2;

            /// <summary>
            /// A resource pool of Pairs.
            /// </summary>
            public static Pool<BroadphasePair> Pool = new Pool<BroadphasePair>();
        }
        #endregion

        #region SweepPoint class
        private class SweepPoint
        {
            public RigidBodyComponent Body;
            public bool Begin;
            public int Axis;

            public SweepPoint(RigidBodyComponent body, bool begin, int axis)
            {
                this.Body = body;
                this.Begin = begin;
                this.Axis = axis;
            }

            public float Value
            {
                get
                {
                    if (Begin)
                    {
                        if (Axis == 0) return Body.Bounds.Left;
                        else return Body.Bounds.Top;
                    }
                    else
                    {
                        if (Axis == 0) return Body.Bounds.Right;
                        else return Body.Bounds.Bottom;
                    }
                }
            }


        }
        #endregion

        #region OverlapPair class
        private struct OverlapPair
        {
            // internal values for faster access within the engine
            public RigidBodyComponent Entity1, Entity2;

            /// <summary>
            /// Initializes a new instance of the BodyPair class.
            /// </summary>
            /// <param name="entity1"></param>
            /// <param name="entity2"></param>
            public OverlapPair(RigidBodyComponent entity1, RigidBodyComponent entity2)
            {
                this.Entity1 = entity1;
                this.Entity2 = entity2;
            }

            /// <summary>
            /// Don't call this, while the key is used in the arbitermap.
            /// It changes the hashcode of this object.
            /// </summary>
            /// <param name="entity1">The first body.</param>
            /// <param name="entity2">The second body.</param>
            internal void SetBodies(RigidBodyComponent entity1, RigidBodyComponent entity2)
            {
                this.Entity1 = entity1;
                this.Entity2 = entity2;
            }

            /// <summary>
            /// Checks if two objects are equal.
            /// </summary>
            /// <param name="obj">The object to check against.</param>
            /// <returns>Returns true if they are equal, otherwise false.</returns>
            public override bool Equals(object obj)
            {
                OverlapPair other = (OverlapPair)obj;
                return (other.Entity1.Equals(Entity1) && other.Entity2.Equals(Entity2) ||
                    other.Entity1.Equals(Entity2) && other.Entity2.Equals(Entity1));
            }

            /// <summary>
            /// Returns the hashcode of the BodyPair.
            /// The hashcode is the same if an BodyPair contains the same bodies.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return Entity1.GetHashCode() + Entity2.GetHashCode();
            }
        }
        #endregion

        private List<SweepPoint> axis1 = new List<SweepPoint>();
        private List<SweepPoint> axis2 = new List<SweepPoint>();

        private HashSet<OverlapPair> fullOverlaps = new HashSet<OverlapPair>();

        public CollisionDetection()
        {
        }

        #region Coherent Update - Insertionsort
        private void SortAxis(object pAxis)
        {
            var axis = (List<SweepPoint>)pAxis;
            for (int j = 1; j < axis.Count; j++)
            {
                SweepPoint keyelement = axis[j];
                float key = keyelement.Value;

                int i = j - 1;

                while (i >= 0 && axis[i].Value > key)
                {
                    SweepPoint swapper = axis[i];

                    if (keyelement.Begin && !swapper.Begin)
                    {
                        if (swapper.Body.Bounds.IntersectsWith(keyelement.Body.Bounds))
                        {
                            lock (fullOverlaps)
                            {
                                fullOverlaps.Add(new OverlapPair(swapper.Body, keyelement.Body));
                            }
                        }
                    }

                    if (!keyelement.Begin && swapper.Begin)
                    {
                        lock (fullOverlaps)
                        {
                            fullOverlaps.Remove(new OverlapPair(swapper.Body, keyelement.Body));
                        }
                    }

                    axis[i + 1] = swapper;
                    i--;
                }
                axis[i + 1] = keyelement;
            }
        }
        #endregion

        public void AddEntity(RigidBodyComponent body)
        {
            axis1.Add(new SweepPoint(body, true, 0));
            axis1.Add(new SweepPoint(body, false, 0));
            axis2.Add(new SweepPoint(body, true, 1));
            axis2.Add(new SweepPoint(body, false, 1));
        }

        Stack<OverlapPair> depricated = new Stack<OverlapPair>();
        public bool RemoveEntity(RigidBodyComponent body)
        {
            int count;

            count = 0;
            for (int i = 0; i < axis1.Count; i++)
            {
                if (axis1[i].Body == body)
                {
                    count++;
                    axis1.RemoveAt(i);
                    if (count == 2) { break; }
                    i--;
                }
            }

            count = 0;
            for (int i = 0; i < axis2.Count; i++)
            { if (axis2[i].Body == body) { count++; axis2.RemoveAt(i); if (count == 2) break; i--; } }

            foreach (var pair in fullOverlaps) if (pair.Entity1 == body || pair.Entity2 == body) depricated.Push(pair);
            while (depricated.Count > 0) fullOverlaps.Remove(depricated.Pop());

            return true;
        }

        bool swapOrder = false;

        /// <summary>
        /// Tells the collisionsystem to check all bodies for collisions. Hook into the
        /// <see cref="CollisionSystem.PassedBroadphase"/>
        /// and <see cref="CollisionSystem.CollisionDetected"/> events to get the results.
        /// </summary>
        /// <param name="multiThreaded">If true internal multithreading is used.</param>
        public void Detect()
        {
            //Broadphase
            Threading.JobManager.Instance.BeginBatch();
            Threading.JobManager.Instance.Queue(SortAxis, axis1);
            Threading.JobManager.Instance.Queue(SortAxis, axis2);
            Threading.JobManager.Instance.ExecuteBatch();

            Threading.JobManager.Instance.BeginBatch();
            foreach (OverlapPair key in fullOverlaps)
            {
                if (!key.Entity1.Active || !key.Entity2.Active) { continue; }

                BroadphasePair pair = BroadphasePair.Pool.Get();
                if (swapOrder)
                {
                    pair.Entity1 = key.Entity1;
                    pair.Entity2 = key.Entity2;
                }
                else
                {
                    pair.Entity2 = key.Entity2;
                    pair.Entity1 = key.Entity1;
                }
                Threading.JobManager.Instance.Queue(DetectCallback, pair);

                swapOrder = !swapOrder;
            }
            Threading.JobManager.Instance.ExecuteBatch();
        }

        private void DetectCallback(object obj)
        {
            BroadphasePair pair = obj as BroadphasePair;
            NarrowPhase(pair.Entity1, pair.Entity2);
            BroadphasePair.Pool.Give(pair);
        }

        public void NarrowPhase(RigidBodyComponent pCollider1, RigidBodyComponent pCollider2)
        {
            //if(false)
            //{
            //    pCollider1.OnCollisionOccurred(pCollider2);
            //    pCollider2.OnCollisionOccurred(pCollider1);
            //}
        }
    }
}