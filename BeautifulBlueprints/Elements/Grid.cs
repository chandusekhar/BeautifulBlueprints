
using System;
using System.Collections.Generic;
using System.Linq;
using BeautifulBlueprints.Layout;

namespace BeautifulBlueprints.Elements
{
    public class Grid
        : BaseElement
    {
        private readonly GridRow[] _rows;
        public IEnumerable<GridRow> Rows { get { return _rows; } }

        private readonly GridColumn[] _columns;
        public IEnumerable<GridColumn> Columns { get { return _columns; } }

        public Grid(
            IEnumerable<GridRow> rows,
            IEnumerable<GridColumn> columns,
            string name = null,
            float minWidth = DEFAULT_MIN_WIDTH,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float maxHeight = DEFAULT_MAX_HEIGHT,
            Margin margin = null
        )
            : base(name, minWidth, maxWidth, minHeight, maxHeight, margin)
        {
            _rows = rows.ToArray();
            _columns = columns.ToArray();
        }

        protected override int MaximumChildren
        {
            get
            {
                return _rows.Length * _columns.Length;
            }
        }

        private float _totalUnsizedRowSpace, _totalSizedRowSpace, _totalUnsizedColSpace, _totalSizedColSpace;
        private float[] _rowSizes;
        private float[] _columnSizes;
        internal override void Prepare()
        {
            base.Prepare();

            var elements = Children.ToArray();

            //Determine row sizes
            _rowSizes = _rowSizes ?? new float[_rows.Length];
            MeasureRows(_rowSizes, out _totalSizedRowSpace, elements, out _totalUnsizedRowSpace);

            //Determine column sizes
            _columnSizes = _columnSizes ?? new float[_columns.Length];
            MeasureColumns(_columnSizes, out _totalSizedColSpace, elements, out _totalUnsizedColSpace);
        }

        internal override IEnumerable<Solver.Solution> Solve(float left, float right, float top, float bottom)
        {
            //List to buffer up solutions in
            List<Solver.Solution> solutions = new List<Solver.Solution>();

            var self = FillSpace(left, right, top, bottom);
            solutions.Add(self);

            if ((self.Right - self.Left) < _totalSizedColSpace)
                throw new LayoutFailureException(string.Format("Width of columns is greater than width of grid for element {0}({1})", GetType().Name, Name));

            if ((self.Top - self.Bottom) < _totalSizedRowSpace)
                throw new LayoutFailureException(string.Format("Height of row is greater than height of grid for element {0}({1})", GetType().Name, Name));

            var elements = Children.ToArray();

            // "Fixed" and "Auto" rows/cols have been laid out already, now size "Grow" rows/cols
            DistributeExcessSpace(_rowSizes, _totalUnsizedRowSpace, (self.Right - self.Left) - _totalSizedRowSpace);
            DistributeExcessSpace(_columnSizes, _totalUnsizedColSpace, (self.Top - self.Bottom) - _totalSizedColSpace);

            //Check that the grid consumes all space
            AssertAllSpaceIsUsed(_rowSizes, self.Right - self.Left, "row", "height");
            AssertAllSpaceIsUsed(_columnSizes, self.Top - self.Bottom, "column", "width");

            //Now fit each element into the space allotted for it's grid cell
            FitChildren(_rowSizes, _columnSizes, elements, self, solutions);

            return solutions;
        }

        private void AssertAllSpaceIsUsed(float[] sizes, float total, string el, string dim)
        {
            if (Math.Abs(sizes.Sum() - total) > float.Epsilon)
                throw new LayoutFailureException(string.Format("Total {0} of all {1}s is less than total {0} of element for element {2}({3})", dim, el, GetType().Name, Name));
        }

        private void FitChildren(float[] rowSizes, float[] columnSizes, BaseElement[] elements, Solver.Solution self, ICollection<Solver.Solution> solutions)
        {
            var yOffset = 0f;
            for (int r = 0; r < rowSizes.Length; r++)
            {
                var row = rowSizes[r];
                var xOffset = 0f;

                for (int c = 0; c < columnSizes.Length; c++)
                {
                    var col = columnSizes[c];

                    //If we've exceeded the element count all cells from here on are empty, early exit
                    var elIndex = r * _rows.Length + c;
                    if (elIndex >= elements.Length)
                        return;

                    //Solve this element within this cell
                    var el = elements[elIndex];
                    var sol = el.Solve(self.Left + xOffset, self.Left + xOffset + col, self.Top - yOffset, self.Top - yOffset - row);
                    foreach (var solution in sol)
                        solutions.Add(solution);

                    xOffset += columnSizes[c];
                }

                //Move down by the height of this row
                yOffset += rowSizes[r];
            }
        }

        private static void DistributeExcessSpace(float[] sizes, float totalUnsizedSpace, float excess)
        {
            //Some sizes will be NaN, distribute the excess space over these
            for (int i = 0; i < sizes.Length; i++)
            {
                if (sizes[i] >= 0)
                    continue;

                //Excess space is distributed over unsized elements by the ratio of their size
                var amount = totalUnsizedSpace / -sizes[i];
                sizes[i] = excess * amount;
            }
        }

        private void MeasureColumns(float[] columnSizes, out float totalSizedColSpace, BaseElement[] elements, out float totalUnsizedColSpace)
        {
            totalSizedColSpace = 0;
            totalUnsizedColSpace = 0;

            for (int i = 0; i < _columns.Length; i++)
            {
                switch (_columns[i].Mode)
                {
                    case SizeMode.Fixed:
                        columnSizes[i] = _columns[i].Size;
                        totalSizedColSpace += columnSizes[i];
                        break;
                    case SizeMode.Auto:
                        //Smallest size wide enough for child elements
                        columnSizes[i] = MeasureColumnElementWidths(elements, i * _columns.Length, _columns.Length);
                        totalSizedColSpace += columnSizes[i];
                        break;
                    case SizeMode.Grow:
                        //negative space indicates we have more work to do on this columns
                        columnSizes[i] = -_columns[i].Size;
                        totalUnsizedColSpace += _columns[i].Size;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected void MeasureRows(float[] rowSizes, out float totalSizedRowSpace, BaseElement[] elements, out float totalUnsizedRowSpace)
        {
            totalSizedRowSpace = 0;
            totalUnsizedRowSpace = 0;

            for (int i = 0; i < _rows.Length; i++)
            {
                switch (_rows[i].Mode)
                {
                    case SizeMode.Fixed:
                        rowSizes[i] = _rows[i].Size;
                        totalSizedRowSpace += rowSizes[i];
                        break;
                    case SizeMode.Auto:
                        //Smallest size wide enough for child elements
                        rowSizes[i] = MeasureRowElementHeights(elements, i * _columns.Length, _columns.Length);
                        totalSizedRowSpace += rowSizes[i];
                        break;
                    case SizeMode.Grow:
                        //Negative space indicates we have more work to do on this row later
                        rowSizes[i] = -_rows[i].Size;
                        totalUnsizedRowSpace += _rows[i].Size;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private float MeasureRowElementHeights(BaseElement[] allElements, int min, int count)
        {
            float height = 0;
            for (int i = min; i < min + count; i++)
            {
                if (i < allElements.Length)
                    height = Math.Max(height, allElements[i].MinHeight);
            }
            return height;
        }

        private float MeasureColumnElementWidths(BaseElement[] allElements, int min, int count)
        {
            float width = 0;
            for (int i = min; i < min + count; i++)
            {
                if (i < allElements.Length)
                    width = Math.Max(width, allElements[i].MinWidth);
            }
            return width;
        }

        internal override BaseElementContainer Contain()
        {
            return new GridContainer(this);
        }
    }

    internal class GridContainer
        : BaseElement.BaseElementContainer
    {
        public GridRowContainer[] Rows { get; set; }

        public GridColumnContainer[] Columns { get; set; }

        public GridContainer()
        {
        }

        public GridContainer(Grid grid)
            : base(grid)
        {
            Rows = grid.Rows.Select(a => a.Contain()).ToArray();
            Columns = grid.Columns.Select(a => a.Contain()).ToArray();
        }

        public override BaseElement Unwrap()
        {
            var s = new Grid(
                Rows.Select(a => a.Unwrap()),
                Columns.Select(a => a.Unwrap()),
                name: Name,
                minWidth: MinWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                maxHeight: MaxHeight,
                margin: (Margin ?? new MarginContainer()).Unwrap()
            );

            foreach (var child in Children)
                s.Add(child.Unwrap());

            return s;
        }
    }
}
