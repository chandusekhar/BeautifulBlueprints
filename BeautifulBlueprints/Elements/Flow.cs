
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BeautifulBlueprints.Elements
{
    public class Flow
        : BaseElement
    {
        internal const Orientation DEFAULT_ORIENTATION = Orientation.Horizontal;
        internal const HorizontalAlignment DEFAULT_HORIZONTAL_ALIGNMENT = HorizontalAlignment.Center;
        internal const VerticalAlignment DEFAULT_VERTICAL_ALIGNMENT = VerticalAlignment.Center;
        internal const Spacing DEFAULT_SPACING = Spacing.Uniform;

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
            float minWidth = DEFAULT_MIN_WIDTH,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float maxHeight = DEFAULT_MAX_HEIGHT,
            Margin margin = null,
            Orientation orientation = DEFAULT_ORIENTATION,
            HorizontalAlignment horizontalAlignment = DEFAULT_HORIZONTAL_ALIGNMENT,
            VerticalAlignment verticalAlignment = DEFAULT_VERTICAL_ALIGNMENT,
            Spacing spacing = DEFAULT_SPACING
        )
            : base(name, minWidth, maxWidth, minHeight, maxHeight, margin)
        {
            Orientation = orientation;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            Spacing = spacing;
        }

        protected override int MaximumChildren
        {
            get
            {
                return int.MaxValue;
            }
        }

        internal override IEnumerable<Layout.Solver.Solution> Solve(float left, float right, float top, float bottom)
        {
            throw new NotImplementedException();
        }

        internal override void Prepare()
        {
            base.Prepare();

            throw new NotImplementedException();
        }

        internal override BaseElementContainer Contain()
        {
            throw new NotImplementedException();
        }
    }
}
