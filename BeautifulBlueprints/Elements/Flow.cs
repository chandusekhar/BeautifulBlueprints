//using System;
//using System.Collections.Generic;
//using System.ComponentModel;

//namespace BeautifulBlueprints.Elements
//{
//    public class Flow
//        : BaseElement
//    {
//        internal const Orientation DEFAULT_ORIENTATION = Orientation.Horizontal;
//        internal const HorizontalAlignment DEFAULT_HORIZONTAL_ALIGNMENT = HorizontalAlignment.Center;
//        internal const VerticalAlignment DEFAULT_VERTICAL_ALIGNMENT = VerticalAlignment.Center;
//        internal const Spacing DEFAULT_SPACING = Spacing.Minimize;

//        public Orientation Orientation { get; set; }

//        public HorizontalAlignment HorizontalAlignment { get; set; }

//        public VerticalAlignment VerticalAlignment { get; set; }

//        public Spacing Spacing { get; set; }

//        public Flow(
//            string name = null,
//            decimal minWidth = DEFAULT_MIN_WIDTH,
//            decimal maxWidth = DEFAULT_MAX_WIDTH,
//            decimal minHeight = DEFAULT_MIN_HEIGHT,
//            decimal maxHeight = DEFAULT_MAX_HEIGHT,
//            Margin margin = null,
//            Orientation orientation = DEFAULT_ORIENTATION,
//            HorizontalAlignment horizontalAlignment = DEFAULT_HORIZONTAL_ALIGNMENT,
//            VerticalAlignment verticalAlignment = DEFAULT_VERTICAL_ALIGNMENT,
//            Spacing spacing = DEFAULT_SPACING
//        )
//            : base(name, minWidth, maxWidth, minHeight, maxHeight, margin)
//        {
//            Orientation = orientation;
//            HorizontalAlignment = horizontalAlignment;
//            VerticalAlignment = verticalAlignment;
//            Spacing = spacing;
//        }

//        protected override int MaximumChildren
//        {
//            get
//            {
//                return int.MaxValue;
//            }
//        }

//        internal override IEnumerable<Layout.Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom)
//        {
//            throw new NotImplementedException();
//        }

//        internal override void Prepare()
//        {
//            base.Prepare();

//            throw new NotImplementedException();
//        }

//        internal override BaseElementContainer Wrap()
//        {
//            return new FlowContainer(this);
//        }
//    }

//    internal class FlowContainer
//        : BaseElement.BaseElementContainer
//    {
//        [DefaultValue(Flow.DEFAULT_ORIENTATION)]
//        public Orientation Orientation { get; set; }

//        [DefaultValue(Flow.DEFAULT_HORIZONTAL_ALIGNMENT)]
//        public HorizontalAlignment HorizontalAlignment { get; set; }

//        [DefaultValue(Flow.DEFAULT_VERTICAL_ALIGNMENT)]
//        public VerticalAlignment VerticalAlignment { get; set; }

//        [DefaultValue(Flow.DEFAULT_SPACING)]
//        public Spacing Spacing { get; set; }

//        public FlowContainer()
//        {
//            Orientation = Flow.DEFAULT_ORIENTATION;
//            HorizontalAlignment = Flow.DEFAULT_HORIZONTAL_ALIGNMENT;
//            VerticalAlignment = Flow.DEFAULT_VERTICAL_ALIGNMENT;
//            Spacing = Flow.DEFAULT_SPACING;
//        }

//        public FlowContainer(Flow flow)
//            : base(flow)
//        {
//            Orientation = flow.Orientation;
//            HorizontalAlignment = flow.HorizontalAlignment;
//            VerticalAlignment = flow.VerticalAlignment;
//            Spacing = flow.Spacing;
//        }

//        public override BaseElement Unwrap()
//        {
//            var s = new Flow(name: Name,
//                minWidth: MinWidth,
//                maxWidth: MaxWidth,
//                minHeight: MinHeight,
//                maxHeight: MaxHeight,
//                margin: (Margin ?? new MarginContainer()).Unwrap(),
//                orientation: Orientation,
//                horizontalAlignment: HorizontalAlignment,
//                verticalAlignment: VerticalAlignment,
//                spacing: Spacing
//            );

//            foreach (var child in Children)
//                s.Add(child.Unwrap());

//            return s;
//        }
//    }
//}
