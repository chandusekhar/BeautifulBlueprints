
using BeautifulBlueprints.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeautifulBlueprints.Elements
{
    public class Grid
        : BaseContainerElement
    {
        private readonly GridRow[] _rows;
        public IEnumerable<GridRow> Rows { get { return _rows; } }

        private readonly GridColumn[] _columns;
        public IEnumerable<GridColumn> Columns { get { return _columns; } }

        public Grid(
            IEnumerable<GridRow> rows,
            IEnumerable<GridColumn> columns,
            string name = null,
            decimal minWidth = DEFAULT_MIN_WIDTH,
            decimal? preferredWidth = null,
            decimal maxWidth = DEFAULT_MAX_WIDTH,
            decimal minHeight = DEFAULT_MIN_HEIGHT,
            decimal? preferredHeight = null,
            decimal maxHeight = DEFAULT_MAX_HEIGHT
        )
            : base(name, minWidth, preferredWidth, maxWidth, minHeight, preferredHeight, maxHeight)
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

        #region prepare layout
        private static SizedElement[] MeasureSizes(ISizeable[] definitions, int rowCount, int columnCount, BaseElement[] elements, Func<BaseElement[], int, int, int, decimal> measureElementSizes, Func<BaseElement[], int, int, int, decimal> measureElementPreferredSize)
        {
            var output = new SizedElement[definitions.Length];

            //Initial pass, Auto size rows will be assigned the smallest possible space
            for (int i = 0; i < definitions.Length; i++)
            {
                output[i].Mode = definitions[i].Mode;

                if (definitions[i].Mode == SizeMode.Fixed)
                {
                    output[i].Size = definitions[i].Size;
                }
                else if (definitions[i].Mode == SizeMode.Grow)
                {
                    output[i].Size = measureElementSizes(elements, i, rowCount, columnCount);
                    output[i].PreferredSize = definitions[i].Size;
                }
                else if (definitions[i].Mode == SizeMode.Auto)
                {
                    //Configure to the smallest size large enough for child elements
                    output[i].Size = measureElementSizes(elements, i, rowCount, columnCount);
                    output[i].PreferredSize = measureElementPreferredSize(elements, i, rowCount, columnCount);
                }
                else
                    throw new NotSupportedException(string.Format("Unknwon size mode {0}", definitions[i].Mode));
            }

            return output;
        }

        private static decimal MeasureRowElementHeights(BaseElement[] elements, int rowIndex, int rowCount, int columnCount)
        {
            decimal height = 0;

            for (int i = rowIndex * columnCount; i < rowIndex * columnCount + columnCount && i < elements.Length; i++)
                height = Math.Max(height, elements[i].MinHeight);

            return height;
        }

        private static decimal MeasureRowPreferredHeight(BaseElement[] elements, int rowIndex, int rowCount, int columnCount)
        {
            decimal prefer = 0;

            var minMax = decimal.MaxValue;
            var maxMin = 0m;

            for (int i = rowIndex * columnCount; i < rowIndex * columnCount + columnCount && i < elements.Length; i++)
            {
                var el = elements[i];

                //Record the most restrictive min and max constraints
                minMax = Math.Min(minMax, el.MaxHeight);
                maxMin = Math.Min(maxMin, el.MinHeight);

                //Record the most permissive preference
                prefer = Math.Max(prefer, el.PreferredHeight);
            }

            //Clamp prefer into the range of the most constrictive min and max constraints we've found
            return Math.Min(Math.Max(prefer, maxMin), minMax);
        }

        private static decimal MeasureColumnElementWidths(BaseElement[] elements, int columnIndex, int rowCount, int columnCount)
        {
            decimal width = 0;

            for (int i = columnIndex; i < elements.Length; i += columnCount)
                width = Math.Max(width, elements[i].MinWidth);

            return width;
        }

        private static decimal MeasureColumnPreferredWidth(BaseElement[] elements, int columnIndex, int rowCount, int columnCount)
        {
            decimal prefer = 0;

            var minMax = decimal.MaxValue;
            var maxMin = 0m;

            for (int i = columnIndex; i < elements.Length; i += columnCount)
            {
                var el = elements[i];

                minMax = Math.Min(minMax, el.MaxWidth);
                maxMin = Math.Min(maxMin, el.MinWidth);

                prefer = Math.Max(prefer, el.PreferredWidth);
            }

            return Math.Min(Math.Max(prefer, maxMin), minMax);
        }
        #endregion

        internal override IEnumerable<Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom)
        {
            var elements = Children.ToArray();

            //List to buffer up solutions in
            List<Solver.Solution> solutions = new List<Solver.Solution>();

            var self = FillSpace(left, right, top, bottom);
            solutions.Add(self);

            // First we lay out fixed elements (and auto elements, to their minimum allowed size), this left us with some excess space...
// ReSharper disable CoVariantArrayConversion
            var rowSizes = MeasureSizes(_rows, _rows.Length, _columns.Length, elements, MeasureRowElementHeights, MeasureRowPreferredHeight);
            var columnSizes = MeasureSizes(_columns, _rows.Length, _columns.Length, elements, MeasureColumnElementWidths, MeasureColumnPreferredWidth);
// ReSharper restore CoVariantArrayConversion

            decimal allocatedRowSpace = rowSizes.Select(a => a.Size).Sum();
            decimal allocatedColSpace = columnSizes.Select(a => a.Size).Sum();

            //Make sure we haven't exceeded our size budget with the fixed parts
            if ((self.Right - self.Left) < allocatedColSpace)
                throw new LayoutFailureException("Width of columns is greater than width of grid", this);
            if ((self.Top - self.Bottom) < allocatedRowSpace)
                throw new LayoutFailureException("Height of row is greater than height of grid", this);

            var excessWidth = (self.Right - self.Left) - allocatedColSpace;
            var excessHeight = (self.Top - self.Bottom) - allocatedRowSpace;

            // ...use up some space by expanding "Auto" elements to their preferred size...
            excessHeight = RoundToZero(AutoSizeElements(rowSizes, excessHeight));
            excessWidth = RoundToZero(AutoSizeElements(columnSizes, excessWidth));

            // ...now use up as much space as possible, expanding "Grow" elements...
            excessHeight = RoundToZero(GrowElements(rowSizes, excessHeight));
            excessWidth = RoundToZero(GrowElements(columnSizes, excessWidth));

            // ...all space must be used by now, check that this is true (fail layout if not)
            AssertAllSpaceIsUsed(excessHeight, "row", "height");
            AssertAllSpaceIsUsed(excessWidth, "column", "width");

            // Grid is laid out. Now fit each child into the space allotted for it's grid cell
            FitChildren(rowSizes, columnSizes, elements, self, solutions);

            return solutions;
        }

        private static decimal RoundToZero(decimal value)
        {
            if (value.IsEqualTo(0))
                return 0;
            return value;
        }

        private void AssertAllSpaceIsUsed(decimal excess, string el, string dim)
        {
            if (excess.IsGreaterThan(0))
                throw new LayoutFailureException(string.Format("Total {0} of all {1}s is less than total {0} of element", dim, el), this);
        }

        private void FitChildren(SizedElement[] rowSizes, SizedElement[] columnSizes, BaseElement[] elements, Solver.Solution self, List<Solver.Solution> solutions)
        {
            var yOffset = 0m;
            for (int r = 0; r < rowSizes.Length; r++)
            {
                var row = rowSizes[r].Size;
                var xOffset = 0m;

                for (int c = 0; c < columnSizes.Length; c++)
                {
                    var col = columnSizes[c].Size;

                    //If we've exceeded the element count all cells from here on are empty, early exit
                    var elIndex = r * _columns.Length + c;
                    if (elIndex >= elements.Length)
                        return;

                    //Solve this element within this cell
                    var el = elements[elIndex];
                    var sol = el.Solve(self.Left + xOffset, self.Left + xOffset + col, self.Top - yOffset, self.Top - yOffset - row);
                    solutions.AddRange(sol);

                    xOffset += col;
                }

                //Move down by the height of this row
                yOffset += row;
            }
        }

        private static decimal AutoSizeElements(SizedElement[] elements, decimal totalExcess)
        {
            decimal remainingExcess = totalExcess;

            while (!remainingExcess.IsEqualTo(0))
            {
                //Count of auto sizes which are not yet at their preferred size
                var autoSizeCount = elements.Where(a => a.Mode == SizeMode.Auto && a.Size < a.PreferredSize).Count();
                if (autoSizeCount == 0)
                    return remainingExcess;

                //Max amount each element may grow
                var growthAllocation = 1m / autoSizeCount * totalExcess;

                //Some sizes will be negative, distribute the excess space over these
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].Mode != SizeMode.Auto)
                        continue;
                    if (elements[i].Size > elements[i].PreferredSize)
                        continue;

                    // Excess is distributed equally across all auto size elements
                    var growth = Math.Min(elements[i].PreferredSize - elements[i].Size, growthAllocation);
                    elements[i].Size += growth;
                    remainingExcess -= growth;
                }

                //Have we reached a fixpoint?
                if (totalExcess.IsEqualTo(remainingExcess))
                    break;

                totalExcess = remainingExcess;
            }

            return remainingExcess;
        }

        private static decimal GrowElements(SizedElement[] elements, decimal totalExcess)
        {
            decimal remainingExcess = totalExcess;

            //We share out space by ratio of preferred sizes, so we need to know the total of all preferred sizes to figure that out
            var growSizeTotal = elements.Where(a => a.Mode == SizeMode.Grow).Select(a => a.PreferredSize).Sum();

            //Some sizes will be negative, distribute the excess space over these
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].Mode != SizeMode.Grow)
                    continue;

                //Excess space is distributed over unsized elements by the ratio of their size
                var amount = elements[i].PreferredSize / Math.Max(1, growSizeTotal) * totalExcess;
                elements[i].Size += amount;
                remainingExcess -= amount;
            }

            return remainingExcess;
        }

        private struct SizedElement
        {
            /// <summary>
            /// Sizing mode of this element
            /// </summary>
            public SizeMode Mode;

            /// <summary>
            /// Size this element would like to be
            /// </summary>
            public decimal PreferredSize;

            /// <summary>
            /// Space currently assigned to this element
            /// </summary>
            public decimal Size;
        }

        #region serialization
        internal override BaseElementContainer Wrap()
        {
            return new GridContainer(this);
        }
        #endregion
    }

    internal class GridContainer
        : BaseContainerElement.BaseContainerElementContainer
    {
        public GridRowContainer[] Rows { get; set; }

        public GridColumnContainer[] Columns { get; set; }

        public GridContainer()
        {
        }

        public GridContainer(Grid grid)
            : base(grid)
        {
            Rows = grid.Rows.Select(a => a.Wrap()).ToArray();
            Columns = grid.Columns.Select(a => a.Wrap()).ToArray();
        }

        public override BaseElement Unwrap()
        {
            return UnwrapChildren(new Grid(
                Rows.Select(a => a.Unwrap()),
                Columns.Select(a => a.Unwrap()),
                name: Name,
                minWidth: MinWidth,
                preferredWidth: PreferredWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight,
                maxHeight: MaxHeight
            ));
        }
    }
}
