using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Physics
{
    public struct AxisAlignedBoundingBox
    {
        readonly Vector2 _min;
        readonly Vector2 _max;

        /// <summary>
        /// The left boundary of the box
        /// </summary>
        public float Left
        {
            get { return _min.X; }
        }

        /// <summary>
        /// The right boundary of the box
        /// </summary>
        public float Right
        {
            get { return _max.X; }
        }

        /// <summary>
        /// The top boundary of the box
        /// </summary>
        public float Top
        {
            get { return _min.Y; }
        }

        /// <summary>
        /// The bottom boundary of the box
        /// </summary>
        public float Bottom
        {
            get { return _max.Y; }
        }

        /// <summary>
        /// The center point of the box
        /// </summary>
        public Vector2 Center { get; private set; }

        public AxisAlignedBoundingBox(Vector2 pMin, Vector2 pMax)
        {
            _min = pMin;
            _max = pMax;
            Center = new Vector2((_max.X + _min.X) / 2, (_max.Y + _min.Y) / 2);
        } 

        /// <summary>
        /// Returns if the specified point is inside the bounding box
        /// </summary>
        public bool Contains(Vector2 pPoint)
        {
            return pPoint.X >= Left && pPoint.X <= Right && pPoint.Y >= Top && pPoint.Y <= Bottom;
        }
    }
}
