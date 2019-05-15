using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using System.Drawing;

namespace SmallEngine.UI
{
    public class Grid : UIElement
    {
        struct GridInfo
        {
            public int Row { get; private set; }
            public int Column { get; private set; }
            public int RowSpan { get; private set; }
            public int ColumnSpan { get; private set; }

            public GridInfo(int pRow, int pColumn, int pRowSpan, int pColumnSpan)
            {
                Row = pRow;
                Column = pColumn;
                RowSpan = pRowSpan;
                ColumnSpan = pColumnSpan;
            }
        }

        Dictionary<UIElement, GridInfo> _info = new Dictionary<UIElement, GridInfo>();
        int _maxColumn, _maxRow;
        public Grid() : base(null) { }

        public Grid(string pName) : base(pName) { }

        public void AddElement(UIElement pElement, int pRow, int pColumn)
        {
            AddElement(pElement, pRow, pColumn, 1, 1);
        }

        public void AddElement(UIElement pElement, int pRow, int pColumn, int pRowSpan, int pColumnSpan)
        {
            System.Diagnostics.Debug.Assert(pRow >= 0 && pColumn >= 0);
            System.Diagnostics.Debug.Assert(pRowSpan >= 0 && pColumnSpan >= 0);

            if (pRow > _maxRow) _maxRow = pRow;
            if (pColumn > _maxColumn) _maxColumn = pColumn;

            base.AddChild(pElement);
            _info.Add(pElement, new GridInfo(pRow, pColumn, pRowSpan, pColumnSpan));
        }

        public override Size MeasureOverride(Size pSize)
        {
            foreach (var c in Children)
            {
                c.Measure(pSize);
            }

            return pSize;
        }

        public override void ArrangeOverride(Rectangle pBounds)
        {
            int rowHeight = (int)(pBounds.Height / (_maxRow + 1));
            int columnWidth = (int)(pBounds.Width / (_maxColumn + 1));

            var p = Position;
            foreach (var c in Children)
            {
                var gi = _info[c];

                System.Diagnostics.Debug.Assert(gi.ColumnSpan <= _maxRow);
                System.Diagnostics.Debug.Assert(gi.RowSpan <= _maxColumn);

                var pos = new Vector2(columnWidth * gi.Column, rowHeight * gi.Row) + Position;
                var width = columnWidth * gi.ColumnSpan;
                var height = rowHeight * gi.RowSpan;

                c.Arrange(new Rectangle(pos, width, height));
            }
        }

        public override void Draw(IGraphicsAdapter pSystem) { }

        public override void Update(float pDeltaTime) { }
    }
}
