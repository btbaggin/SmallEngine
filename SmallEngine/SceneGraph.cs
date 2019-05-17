using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine
{
    public class SceneGraph
    {
        private const int MAX_LEVELS = 8;
        private const int MAX_OBJECTS = 128;
        private readonly List<IGameObject> _entities;
        private Rectangle _bounds;

        private readonly SceneGraph[] _nodes = new SceneGraph[4];

        #region Properties
        public float Width
        {
            get { return _bounds.Width; }
        }

        public float Height
        {
            get { return _bounds.Height; }
        }

        public int Count
        {
            get { return _entities.Count; }
        }

        public int Level { get; private set; }
        #endregion

        #region Constructor
        private SceneGraph(int pLevel, Rectangle pBounds)
        {
            Level = pLevel;
            _entities = new List<IGameObject>();
            _bounds = pBounds;
        }

        public static SceneGraph CreateFrom(Scene pScene)
        {
            var g = new SceneGraph(0, Physics.PhysicsHelper.WorldBounds);
            foreach(var go in pScene.GameObjects)
            {
                g.Insert(go);
            }
            return g;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Removes all objects from the QuadTree
        /// </summary>
        public void Clear()
        {
            //remove all objects in this node
            _entities.Clear();

            //clear all child nodes
            foreach (var n in _nodes)
            {
                if (n != null) n.Clear();
            }

            _nodes[0] = null;
            _nodes[1] = null;
            _nodes[2] = null;
            _nodes[3] = null;
        }

        /// <summary>
        /// Inserts the specified object into the QuadTree.
        /// </summary>
        /// <param name="pEntity">Object to insert</param>
        public void Insert(IGameObject pEntity)
        {
            if (_nodes[0] != null)
            {
                int index = GetIndex(pEntity.Position);
                _nodes[index].Insert(pEntity);
                return;
            }

            _entities.Add(pEntity);

            if (_entities.Count > MAX_OBJECTS && Level < MAX_LEVELS)
            {
                if (_nodes[0] == null)
                {
                    Split();
                }

                int i = 0;
                while (_entities.Count > 0)
                {
                    var e = _entities[i];
                    int index = GetIndex(e.Position);
                    _entities.Remove(e);
                    _nodes[index].Insert(e);
                }
            }
        }

        /// <summary>
        /// Retrieves all objects currently colliding with the given point
        /// </summary>
        /// <param name="pPoint">Point to check collisions against</param>
        /// <returns>IEnumerable of colliding objects</returns>
        public IEnumerable<IGameObject> RetrieveWithinDistance(Vector2 pPoint, float pDistance)
        {
            var l = new List<IGameObject>();
            return Retrieve(ref l, pPoint, pDistance);
        }
        #endregion

        #region Private Functions
        private List<IGameObject> Retrieve(ref List<IGameObject> pReturnObjects, Vector2 pPoint, float pDistance)
        {
            var distanceSqrd = pDistance * pDistance;
            foreach(var i in GetNearbyIndexes(pPoint, distanceSqrd))
            {
                if (i != -1 && _nodes[i] != null)
                {
                    _nodes[i].Retrieve(ref pReturnObjects, pPoint, pDistance);
                }
            }
            
            foreach(var go in _entities)
            {
                if(Vector2.DistanceSqrd(pPoint, go.Position) < distanceSqrd)
                {
                    pReturnObjects.Add(go);
                }
            }

            return pReturnObjects;
        }

        private void Split()
        {
            var subWidth = _bounds.Width / 2;
            var subHeight = _bounds.Height / 2;

            _nodes[0] = new SceneGraph(Level + 1, new Rectangle(_bounds.X, _bounds.Y, subWidth, subHeight));
            _nodes[1] = new SceneGraph(Level + 1, new Rectangle(_bounds.X + subWidth, _bounds.Y, subWidth, subHeight));
            _nodes[2] = new SceneGraph(Level + 1, new Rectangle(_bounds.X, _bounds.Y + subHeight, subWidth, subHeight));
            _nodes[3] = new SceneGraph(Level + 1, new Rectangle(_bounds.X + subWidth, _bounds.Y + subHeight, subWidth, subHeight));
        }

        private int GetIndex(Vector2 pPoint)
        {
            double verticalMidpoint = _bounds.X + (_bounds.Width / 2);
            double horizontalMidpoint = _bounds.Y + (_bounds.Height / 2);

            //Object can completely fit within the top quadrants
            var topQuadrant = pPoint.Y < horizontalMidpoint;
            var leftQuadrant = pPoint.X < verticalMidpoint;

            if (leftQuadrant)
            {
                return topQuadrant ? 0 : 2;
            }
            else
            {
                return topQuadrant ? 1 : 3;
            }
        }

        private int[] GetNearbyIndexes(Vector2 pPoint, float pDistance)
        {
            double verticalMidpoint = _bounds.X + (_bounds.Width / 2);
            double horizontalMidpoint = _bounds.Y + (_bounds.Height / 2);

            int[] retval = new int[3];
            for (int i = 0; i < retval.Length; i++) retval[i] = -1;

            //Object can completely fit within the top quadrants
            var topQuadrant = pPoint.Y < horizontalMidpoint;
            var leftQuadrant = pPoint.X < verticalMidpoint;

            var midX = new Vector2(_bounds.X, pPoint.Y);
            var midY = new Vector2(pPoint.X, _bounds.Y);

            if (leftQuadrant)
            {
                retval[0] = topQuadrant ? 0 : 2;
            }
            else
            {
                retval[0] = topQuadrant ? 1 : 3;
            }

            //Check if the closest point on the quadrant boundaries are within distance
            //Add those quadrants to the return list
            if (Vector2.DistanceSqrd(pPoint, midX) <= pDistance)
            {
                if (leftQuadrant) retval[1] = retval[0] + 1;
                else retval[1] = retval[0] - 1;
            }
            if (Vector2.DistanceSqrd(pPoint, midY) <= pDistance)
            {
                if (topQuadrant) retval[2] = retval[0] + 2;
                else retval[2] = retval[0] - 2;
            }

            return retval;
        }
        #endregion
    }
}
