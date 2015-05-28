//using System.ComponentModel;
//using BeautifulBlueprints.Layout;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace BeautifulBlueprints.Elements
//{
//    public class Stack
//        : BaseElement
//    {
//        internal const Orientation DEFAULT_ORIENTATION = Orientation.Horizontal;
//        internal const HorizontalAlignment DEFAULT_HORIZONTAL_ALIGNMENT = HorizontalAlignment.Center;
//        internal const VerticalAlignment DEFAULT_VERTICAL_ALIGNMENT = VerticalAlignment.Bottom;
//        internal const Spacing DEFAULT_INLINE_SPACING = Spacing.Minimize;
//        internal const Spacing DEFAULT_OFFSIDE_SPACING = Spacing.Minimize;

//        private readonly Orientation _orientation;
//        public Orientation Orientation { get { return _orientation; } }

//        private readonly HorizontalAlignment _horizontalAlignment;
//        public HorizontalAlignment HorizontalAlignment { get { return _horizontalAlignment; } }

//        private readonly VerticalAlignment _verticalAlignment;

//        public VerticalAlignment VerticalAlignment { get { return _verticalAlignment; } }

//        private readonly Spacing _inlineSpacing;
//        public Spacing InlineSpacing { get { return _inlineSpacing; } }

//        private readonly Spacing _offsideSpacing;
//        public Spacing OffsideSpacing { get { return _offsideSpacing; } }

//        private float? _maxWidth;
//        public override float MaxWidth
//        {
//            get
//            {
//                return _maxWidth ?? base.MaxWidth;
//            }
//        }

//        private float? _minWidth;
//        public override float MinWidth
//        {
//            get
//            {
//                return _minWidth ?? base.MinWidth;
//            }
//        }

//        private float? _maxHeight;
//        public override float MaxHeight
//        {
//            get
//            {
//                return _maxHeight ?? base.MaxHeight;
//            }
//        }

//        private float? _minHeight;
//        public override float MinHeight
//        {
//            get
//            {
//                return _minHeight ?? base.MinHeight;
//            }
//        }

//        public Stack(
//            string name = null,
//            float minWidth = DEFAULT_MIN_WIDTH,
//            float maxWidth = DEFAULT_MAX_WIDTH,
//            float minHeight = DEFAULT_MIN_HEIGHT,
//            float maxHeight = DEFAULT_MAX_HEIGHT,
//            Margin margin = null,
//            Orientation orientation = DEFAULT_ORIENTATION,
//            HorizontalAlignment horizontalAlignment = DEFAULT_HORIZONTAL_ALIGNMENT,
//            VerticalAlignment verticalAlignment = DEFAULT_VERTICAL_ALIGNMENT,
//            Spacing inlineSpacing = DEFAULT_INLINE_SPACING,
//            Spacing offsideSpacing = DEFAULT_OFFSIDE_SPACING
//        )
//            : base(name, minWidth, maxWidth, minHeight, maxHeight, margin)
//        {
//            _orientation = orientation;
//            _horizontalAlignment = horizontalAlignment;
//            _verticalAlignment = verticalAlignment;
//            _inlineSpacing = inlineSpacing;
//            _offsideSpacing = offsideSpacing;
//        }

//        protected override int MaximumChildren
//        {
//            get
//            {
//                return int.MaxValue;
//            }
//        }

//        internal override IEnumerable<Solver.Solution> Solve(float left, float right, float top, float bottom)
//        {
//            List<Solver.Solution> solutions = new List<Solver.Solution>();

//            var self = FillSpace(left, right, top, bottom);
//            solutions.Add(self);

//            //Early exit when there are no children
//            var children = Children.ToArray();
//            if (children.Length == 0)
//                return solutions;

//            //Measure min, max and preferred width of child elements
//            float totalMin = 0;
//            float totalMax = 0;
//            float totalPreferred = 0;
//            foreach (var child in children)
//            {
//                if (Orientation == Orientation.Horizontal)
//                {
//                    var margin = child.Margin.Left + child.Margin.Right;
//                    totalMax += child.MaxWidth + margin;
//                    totalMin += child.MinWidth + margin;
//                    totalPreferred += child.PreferredWidth + margin;
//                }
//                else
//                {
//                    var margin = child.Margin.Top + child.Margin.Bottom;
//                    totalMax += child.MaxHeight + margin;
//                    totalMin += child.MinHeight + margin;
//                    totalPreferred += child.PreferredHeight + margin;
//                }
//            }

//            //if the sum of the min extends of all children is more than the current extent of the element, we've failed
//            if (totalMin > (Orientation == Orientation.Horizontal ? (right - left) : (top - bottom)))
//                throw new LayoutFailureException(string.Format("total minimum {0} is > {0} for element {1}({2})", (Orientation == Orientation.Horizontal ? "Width" : "Height"), GetType().Name, Name));

//            //Calculate the spacing of the elements
//            switch (InlineSpacing)
//            {
//                //No spacing is allowed, size all the elements so that they all touch
//                case Spacing.None:
//                    throw new NotImplementedException();

//                //Maximise the spacing, shrink all elements to their minimum allowed extent
//                case Spacing.Maximize:
//                    throw new NotImplementedException();
//                    break;

//                //Minimise the spacing, expand all elements to their maximum allowed extent (so long as that does not exceed parent extent, in which case size in ratios of preferred size)
//                case Spacing.Minimize:
//                    throw new NotImplementedException();

//                default:
//                    throw new ArgumentOutOfRangeException();
//            }

//            //Distribute the calculate space according to the inline alignment
//            throw new NotImplementedException();

//            //Move each element across the axis acording to the offside alignment
//            throw new NotImplementedException();

//            return solutions;
//        }

//        internal override void Prepare()
//        {
//            base.Prepare();

//            if (!Children.Any())
//            {
//                _maxWidth = null;
//                _minWidth = null;

//                _maxHeight = null;
//                _minHeight = null;
//            }
//            else
//            {
//                //In either mode, we can't be any narrower/shorter than the sum of the min of all child elements
//                _minWidth = Children.Select(a => a.MinWidth).Sum();
//                _minHeight = Children.Select(a => a.MinHeight).Sum();

//                if (Orientation == Orientation.Horizontal)
//                {
//                    _maxWidth = MeasureInlineSpacing(InlineSpacing, base.MaxWidth, Children.Select(a => a.MaxWidth).Sum());
//                    _maxHeight = MeasureOffsideSpacing(OffsideSpacing, base.MaxHeight, Children.Select(a => a.MaxHeight).Min());
//                }
//                else
//                {
//                    _maxHeight = MeasureInlineSpacing(InlineSpacing, base.MaxHeight, Children.Select(a => a.MaxHeight).Sum());
//                    _maxWidth = MeasureOffsideSpacing(OffsideSpacing, base.MaxWidth, Children.Select(a => a.MaxWidth).Min());
//                }
//            }
//        }

//        private float? MeasureInlineSpacing(Spacing inlineSpacing, float selfMax, float sumChildMaxSizes)
//        {
//            switch (inlineSpacing)
//            {
//                //In all these modes we're allowed empty space, so the max is the configured max
//                case Spacing.Maximize:
//                case Spacing.Minimize:
//                    return null;

//                //We're not allowed any spacing, therefore the max width is the min(configured max, sum(child max widths))
//                case Spacing.None:
//                    return Math.Min(selfMax, sumChildMaxSizes);

//                default:
//                    throw new NotImplementedException(string.Format("Stack Spacing Mode {0} Not Implemented", InlineSpacing));
//            }
//        }

//        private float? MeasureOffsideSpacing(Spacing offsideSpacing, float selfMax, float minChildMaxSize)
//        {
//            switch (OffsideSpacing)
//            {
//                //In all these modes we're allowed empty space, so the max is the configured max
//                case Spacing.Maximize:
//                case Spacing.Minimize:
//                    return null;

//                //We're not allowed any spacing, therefore the max width is the min(max child widths)
//                case Spacing.None:
//                    return Math.Min(selfMax, minChildMaxSize);

//                default:
//                    throw new NotImplementedException(string.Format("Stack Spacing Mode {0} Not Implemented", InlineSpacing));
//            }
//        }

//        internal override BaseElementContainer Wrap()
//        {
//            return new StackContainer(this);
//        }
//    }

//    internal class StackContainer
//        : BaseElement.BaseElementContainer
//    {
//        [DefaultValue(Stack.DEFAULT_ORIENTATION)]
//        public Orientation Orientation { get; set; }

//        [DefaultValue(Stack.DEFAULT_HORIZONTAL_ALIGNMENT)]
//        public HorizontalAlignment HorizontalAlignment { get; set; }

//        [DefaultValue(Stack.DEFAULT_VERTICAL_ALIGNMENT)]
//        public VerticalAlignment VerticalAlignment { get; set; }

//        [DefaultValue(Stack.DEFAULT_INLINE_SPACING)]
//        public Spacing InlineSpacing { get; set; }

//        [DefaultValue(Stack.DEFAULT_OFFSIDE_SPACING)]
//        public Spacing OffsideSpacing { get; set; }

//        public StackContainer()
//        {
//            Orientation = Stack.DEFAULT_ORIENTATION;
//            HorizontalAlignment = Stack.DEFAULT_HORIZONTAL_ALIGNMENT;
//            VerticalAlignment = Stack.DEFAULT_VERTICAL_ALIGNMENT;
//            InlineSpacing = Stack.DEFAULT_INLINE_SPACING;
//            OffsideSpacing = Stack.DEFAULT_OFFSIDE_SPACING;
//        }

//        public StackContainer(Stack stack)
//            : base(stack)
//        {
//            Orientation = stack.Orientation;
//            HorizontalAlignment = stack.HorizontalAlignment;
//            VerticalAlignment = stack.VerticalAlignment;
//            InlineSpacing = stack.InlineSpacing;
//            OffsideSpacing = stack.OffsideSpacing;
//        }

//        public override BaseElement Unwrap()
//        {
//            var s = new Stack(name: Name,
//                minWidth: MinWidth,
//                maxWidth: MaxWidth,
//                minHeight: MinHeight,
//                maxHeight: MaxHeight,
//                margin: (Margin ?? new MarginContainer()).Unwrap(),
//                orientation: Orientation,
//                horizontalAlignment: HorizontalAlignment,
//                verticalAlignment: VerticalAlignment,
//                inlineSpacing: InlineSpacing,
//                offsideSpacing: OffsideSpacing
//            );

//            foreach (var child in Children)
//                s.Add(child.Unwrap());

//            return s;
//        }
//    }
//}
