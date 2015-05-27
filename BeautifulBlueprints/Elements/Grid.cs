
using System.Collections.Generic;
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
            string name = null,
            float minWidth = 0,
            float maxWidth = float.PositiveInfinity,
            float minHeight = 0,
            float maxHeight = float.PositiveInfinity,
            Margin margin = null,
            bool columnFlex = false,
            bool rowFlex = false
        )
            : base(name, minWidth, maxWidth, minHeight, maxHeight, margin)
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

        protected override int MaximumChildren
        {
            get
            {
                return Rows * Columns;
            }
        }

        internal override IEnumerable<Layout.Solver.Solution> Solve(float left, float right, float top, float bottom)
        {
            var self = FillSpace(left, right, top, bottom);
            yield return self;

            throw new System.NotImplementedException();
        }

        internal override void Prepare()
        {
            base.Prepare();

            throw new System.NotImplementedException();
        }
    }
}
