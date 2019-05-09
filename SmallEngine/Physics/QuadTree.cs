using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public class QuadTree<T> where T : IPhysicsBody
    {
        private const int MAX_LEVELS = 8;
        private const int MAX_OBJECTS = 128;
        private readonly List<T> _entities;
        private Rectangle _bounds;

        private readonly QuadTree<T>[] _nodes = new QuadTree<T>[4];

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
        public QuadTree(Rectangle pBounds) : this(0, pBounds) { }

        private QuadTree(int pLevel, Rectangle pBounds)
        {
            Level = pLevel;
            _entities = new List<T>();
            _bounds = pBounds;
        }
        #endregion

        #region "Public Functions"
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
                if(n != null) n.Clear();
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
        public void Insert(T pEntity)
        {
            if (_nodes[0] != null)
            {
                int index = GetIndex(pEntity);

                if (index != -1)
                {
                    _nodes[index].Insert(pEntity);

                    return;
                }
            }

            _entities.Add(pEntity);

            if (_entities.Count > MAX_OBJECTS && Level < MAX_LEVELS)
            {
                if (_nodes[0] == null)
                {
                    Split();
                }

                int i = 0;
                while (i < _entities.Count)
                {
                    var e = _entities[i];
                    int index = GetIndex(e);
                    if (index != -1)
                    {
                        _entities.Remove(e);
                        _nodes[index].Insert(e);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the specified object from the QuadTree
        /// </summary>
        /// <param name="pEntity">Object to remove</param>
        /// <returns>True is the entity is found</returns>
        public bool Remove(T pEntity)
        {
            //recurse until we cannot fit the rectangle in a smaller node
            var i = GetIndex(pEntity);
            if (i != -1 && Level <= MAX_LEVELS)
            {
                if (_nodes[i] == null)
                {
                    return false;
                }

                _nodes[i].Remove(pEntity);
            }

            //Remove object from smallest node and all bigger nodes it recursed through
            if (!_entities.Contains(pEntity)) return false;

            _entities.Remove(pEntity);

            return true;
        }

        /// <summary>
        /// Retrieves all objects currently colliding with the given point
        /// </summary>
        /// <param name="pPoint">Point to check collisions against</param>
        /// <returns>IEnumerable of colliding objects</returns>
        public IEnumerable<T> Retrieve(Vector2 pPoint)
        {
            var l = new List<T>();
            return Retrieve(ref l, new AxisAlignedBoundingBox(pPoint, Vector2.Unit));
        }

        /// <summary>
        /// Retrieves all objects currently colliding with the given object
        /// </summary>
        /// <param name="pObject">Object to check collisions against</param>
        /// <returns>IEnumerable of colliding objects</returns>
        public IEnumerable<T> Retrieve(T pEntitiy)
        {
            var l = new List<T>();
            return Retrieve(ref l, pEntitiy.AABB);
        }

        public void Resize(Rectangle pBounds)
        {
            Clear();
            _bounds = pBounds;
        }

        /// <summary>
        /// Returns the sub-QuadTree at the given index
        /// </summary>
        /// <param name="pIndex">Index of the subtree to retrieve</param>
        /// <returns>Sub-QuadTree at the given index</returns>
        public QuadTree<T> GetSubtree(int pIndex)
        {
            return _nodes[pIndex];
        }
        #endregion

        #region "Private Functions"
        private List<T> Retrieve(ref List<T> pReturnObjects, AxisAlignedBoundingBox pRect)
        {
            int index = GetIndex(pRect);
            if (index != -1 && _nodes[index] != null)
            {
                _nodes[index].Retrieve(ref pReturnObjects, pRect);
            }

            pReturnObjects.AddRange(_entities);

            return pReturnObjects;
        }

        private void Split()
        {
            var subWidth = _bounds.Width / 2;
            var subHeight = _bounds.Height / 2;

            _nodes[0] = new QuadTree<T>(Level + 1, new Rectangle(_bounds.X, _bounds.Y, subWidth, subHeight));
            _nodes[1] = new QuadTree<T>(Level + 1, new Rectangle(_bounds.X + subWidth, _bounds.Y, subWidth, subHeight));
            _nodes[2] = new QuadTree<T>(Level + 1, new Rectangle(_bounds.X, _bounds.Y + subHeight, subWidth, subHeight));
            _nodes[3] = new QuadTree<T>(Level + 1, new Rectangle(_bounds.X + subWidth, _bounds.Y + subHeight, subWidth, subHeight));
        }

        private int GetIndex(T pEntity)
        {
            return GetIndex(pEntity.AABB);
        }

        private int GetIndex(AxisAlignedBoundingBox pRect)
        {
            var i = -1;
            double verticalMidpoint = _bounds.X + (_bounds.Width / 2);
            double horizontalMidpoint = _bounds.Y + (_bounds.Height / 2);

            //Object can completely fit within the top quadrants
            var topQuadrant = pRect.Bottom < horizontalMidpoint;
            //Object can completely fit within the bottom quadrants
            var bottomQuadrant = pRect.Top > horizontalMidpoint;

            //Object can completely fit within the left quadrants
            if (pRect.Right < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    i = 0;
                }
                else if (bottomQuadrant)
                {
                    i = 2;
                }
            } //Object can completely fit within the right quadrants
            else if (pRect.Left > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    i = 1;
                }
                else if (bottomQuadrant)
                {
                    i = 3;
                }
            }

            return i;
        }
        #endregion
    }
}
