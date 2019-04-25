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
        readonly Vector2 _center;

        public float Left
        {
            get { return _min.X; }
        }

        public float Right
        {
            get { return _max.X; }
        }

        public float Top
        {
            get { return _min.Y; }
        }

        public float Bottom
        {
            get { return _max.Y; }
        }

        public Vector2 Center
        {
            get { return _center; }
        }

        public AxisAlignedBoundingBox(Vector2 pMin, Vector2 pMax)
        {
            _min = pMin;
            _max = pMax;
            _center = new Vector2((_max.X + _min.X) / 2, (_max.Y + _min.Y) / 2);
        }
    }
}
