
using System.ComponentModel;

namespace BeautifulBlueprints.Elements
{
    public class Grid
        : BaseElement
    {
        [DefaultValue(false)]
        public bool ColumnFlex { get; set; }

        [DefaultValue(false)]
        public bool RowFlex { get; set; }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public Grid(
            int columns,
            int rows,
            float minWidth = 0,
            float maxWidth = float.PositiveInfinity,
            float minHeight = 0,
            float maxHeight = float.PositiveInfinity,
            Margin? margin = null,
            bool columnFlex = false,
            bool rowFlex = false
        )
            : base(minWidth, maxWidth, minHeight, maxHeight, margin)
        {
            ColumnFlex = columnFlex;
            RowFlex = rowFlex;

            Rows = rows;
            Columns = columns;
        }

        public Grid()
            : this(0, 0)
        {
        }

        protected override bool AllowChildren
        {
            get
            {
                return true;
            }
        }

        internal override System.Collections.Generic.IEnumerable<Layout.Solver.Solution> Solve(float left, float right, float top, float bottom)
        {
            throw new System.NotImplementedException();
        }
    }
}
