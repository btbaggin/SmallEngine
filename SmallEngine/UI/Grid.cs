using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class Grid : ContainerElement
    {
        #region GridInfo
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
        #endregion

        public static float Auto => float.MaxValue;

        public static float Fill => float.MinValue;

        readonly List<float> _columns = new List<float>();
        readonly List<float> _rows = new List<float>();
        (float Size, float Sum)[] _columnSizes;
        (float Size, float Sum)[] _rowSizes;

        UIElement[,] _grid = new UIElement[0, 0];
        readonly Dictionary<UIElement, GridInfo> _info = new Dictionary<UIElement, GridInfo>();

        public Grid() : base(null) { }

        public Grid(string pName) : base(pName) { }

        public void DefineColumns(params float[] pWidths)
        {
            _columns.Clear();
            _columns.AddRange(pWidths);
            _grid = new UIElement[_columns.Count, _grid.GetLength(1)];
        }

        public void DefineRows(params float[] pHeights)
        {
            _rows.Clear();
            _rows.AddRange(pHeights);
            _grid = new UIElement[_grid.GetLength(0), _rows.Count];
        }

        public void AddElement(UIElement pElement, int pRow, int pColumn)
        {
            AddElement(pElement, pRow, pColumn, 1, 1);
        }

        public void AddElement(UIElement pElement, int pRow, int pColumn, int pRowSpan, int pColumnSpan)
        {
            System.Diagnostics.Debug.Assert(pRow >= 0 && pColumn >= 0);
            System.Diagnostics.Debug.Assert(pRowSpan > 0 && pColumnSpan > 0);
            System.Diagnostics.Debug.Assert(pRow < _rows.Count);
            System.Diagnostics.Debug.Assert(pColumn < _columns.Count);

            for(int x = pColumn; x < pColumn + pColumnSpan; x++)
            {
                for(int y = pRow; y < pRow + pRowSpan; y++)
                {
                    _grid[x, y] = pElement;
                }
            }
            _info.Add(pElement, new GridInfo(pRow, pColumn, pRowSpan, pColumnSpan));
            base.AddElement(pElement);
        }

        public override Size MeasureOverride(Size pSize)
        {
            float width = 0;
            float height = 0;
            foreach (var c in Children)
            {
                c.Measure(pSize);
            }

            //Calc sizes of non-fill columns
            _columnSizes = new (float, float)[_columns.Count];
            int fillWidths = 0;
            for (int x = 0; x < _columns.Count; x++)
            {
                if (_columns[x] == Auto)
                {
                    for(int y = 0; y < _rows.Count; y++)
                    {
                        if(_grid[x, y] != null)
                            _columnSizes[x].Size = Math.Max(_columnSizes[x].Size, _grid[x, y].DesiredSize.Width);
                    }
                }
                else if (_columns[x] == Fill) fillWidths++;
                else _columnSizes[x].Size = _columns[x];
                width += _columnSizes[x].Size;
            }

            //Calc sizes of non-Fill rows
            _rowSizes = new (float, float)[_rows.Count];
            int fillHeights = 0;
            for (int y = 0; y < _rows.Count; y++)
            {
                if (_rows[y] == Auto)
                {
                    for(int x = 0; x < _columns.Count; x++)
                    {
                        if(_grid[x, y] != null)
                            _rowSizes[y].Size = Math.Max(_rowSizes[y].Size, _grid[x, y].DesiredSize.Height);
                    }
                }
                else if (_rows[y] == Fill) fillHeights++;
                else _rowSizes[y].Size = _rows[y];
                height += _rowSizes[y].Size;
            }

            var widthRemaining = (pSize.Width - width) / fillWidths;
            var heightRemaining = (pSize.Height - height) / fillHeights;

            //Set fill columns
            for (int x = 0; x < _columns.Count; x++)
            {
                if (_columns[x] == Fill)
                {
                    _columnSizes[x].Size = widthRemaining;
                    width += widthRemaining;
                }
                if(x > 0)
                {
                    _columnSizes[x].Sum = _columnSizes[x - 1].Size + _columnSizes[x - 1].Sum;
                }
            }

            //Set fill rows
            for (int y = 0; y < _rows.Count; y++)
            {
                if (_rows[y] == Fill)
                {
                    _rowSizes[y].Size = heightRemaining;
                    height += heightRemaining;
                }
                if (y > 0)
                {
                    _rowSizes[y].Sum = _rowSizes[y - 1].Size + _rowSizes[y - 1].Sum;
                }
            }

            //Now that we know exact sizes we need to remeasure children so they fit into the proper space
            //Instead of taking up all of pSize
            foreach (var c in Children)
            {
                var gi = _info[c];
                var x = _columnSizes[gi.Column].Sum;
                var y = _rowSizes[gi.Row].Sum;
                var child_width = _columnSizes[gi.Column + gi.ColumnSpan - 1].Sum - x + _columnSizes[gi.Column + gi.ColumnSpan - 1].Size;
                var child_height = _rowSizes[gi.Row + gi.RowSpan - 1].Sum - y + _rowSizes[gi.Row + gi.RowSpan - 1].Size;

                c.Measure(new Size(child_width, child_height));
            }

            return new Size(width, height);
        }

        public override void ArrangeOverride(Rectangle pBounds)
        {
            foreach (var c in Children)
            {
                var gi = _info[c];
                var x = _columnSizes[gi.Column].Sum;
                var y = _rowSizes[gi.Row].Sum;
                var width = _columnSizes[gi.Column + gi.ColumnSpan- 1].Sum - x +_columnSizes[gi.Column + gi.ColumnSpan - 1].Size;
                var height = _rowSizes[gi.Row + gi.RowSpan - 1].Sum - y + _rowSizes[gi.Row + gi.RowSpan - 1].Size;

                c.Arrange(new Rectangle(x + Position.X, y + Position.Y, width, height));
            }
        }

        public override void Draw(IGraphicsAdapter pSystem) { }

        public override void Update() { }

        public override bool MouseWithin()
        {
            return false;
        }
    }
}
