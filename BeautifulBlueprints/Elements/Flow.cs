
using System.ComponentModel;

namespace BeautifulBlueprints.Elements
{
    public class Flow
        : BaseElement
    {
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation { get; set; }

        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment HorizontalAlignment { get; set; }

        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment VerticalAlignment { get; set; }

        [DefaultValue(Spacing.Uniform)]
        public Spacing Spacing { get; set; }

        public Flow(
            string name = null,
            float minWidth = 0,
            float maxWidth = float.PositiveInfinity,
            float minHeight = 0,
            float maxHeight = float.PositiveInfinity,
            Margin margin = null,
            Orientation orientation = Orientation.Horizontal,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment verticalAlignment = VerticalAlignment.Center,
            Spacing spacing = Spacing.Uniform
        )
            : base(name, minWidth, maxWidth, minHeight, maxHeight, margin)
        {
            Orientation = orientation;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            Spacing = spacing;
        }

        public Flow()
            : this(minWidth: 0)
        {
        }

        protected override int MaximumChildren
        {
            get
            {
                return int.MaxValue;
            }
        }

        internal override System.Collections.Generic.IEnumerable<Layout.Solver.Solution> Solve(float left, float right, float top, float bottom)
        {
            throw new System.NotImplementedException();
        }

        internal override void Prepare()
        {
            base.Prepare();

            throw new System.NotImplementedException();
        }
    }
}
