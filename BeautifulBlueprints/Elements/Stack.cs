using BeautifulBlueprints.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BeautifulBlueprints.Elements
{
    public class Stack
        : BaseElement
    {
        internal const Orientation DEFAULT_ORIENTATION = Orientation.Horizontal;
        internal const HorizontalAlignment DEFAULT_HORIZONTAL_ALIGNMENT = HorizontalAlignment.Center;
        internal const VerticalAlignment DEFAULT_VERTICAL_ALIGNMENT = VerticalAlignment.Bottom;
        internal const Spacing DEFAULT_INLINE_SPACING = Spacing.Uniform;
        internal const Spacing DEFAULT_OFFSIDE_SPACING = Spacing.Minimize;

        private readonly Orientation _orientation;
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation { get { return _orientation; } }

        private readonly HorizontalAlignment _horizontalAlignment;
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment HorizontalAlignment { get { return _horizontalAlignment; } }

        private readonly VerticalAlignment _verticalAlignment;

        [DefaultValue(VerticalAlignment.Bottom)]
        public VerticalAlignment VerticalAlignment { get { return _verticalAlignment; } }

        private readonly Spacing _inlineSpacing;
        [DefaultValue(Spacing.Uniform)]
        public Spacing InlineSpacing { get { return _inlineSpacing; } }

        private readonly Spacing _offsideSpacing;
        [DefaultValue(Spacing.Minimize)]
        public Spacing OffsideSpacing { get { return _offsideSpacing; } }

        private float? _maxWidth;
        public override float MaxWidth
        {
            get
            {
                return _maxWidth ?? base.MaxWidth;
            }
        }

        private float? _minWidth;
        public override float MinWidth
        {
            get
            {
                return _minWidth ?? base.MinWidth;
            }
        }

        private float? _maxHeight;
        public override float MaxHeight
        {
            get
            {
                return _maxHeight ?? base.MaxHeight;
            }
        }

        private float? _minHeight;
        public override float MinHeight
        {
            get
            {
                return _minHeight ?? base.MinHeight;
            }
        }

        public Stack(
            string name = null,
            float minWidth = DEFAULT_MIN_WIDTH,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float maxHeight = DEFAULT_MAX_HEIGHT,
            Margin margin = null,
            Orientation orientation = DEFAULT_ORIENTATION,
            HorizontalAlignment horizontalAlignment = DEFAULT_HORIZONTAL_ALIGNMENT,
            VerticalAlignment verticalAlignment = DEFAULT_VERTICAL_ALIGNMENT,
            Spacing inlineSpacing = DEFAULT_INLINE_SPACING,
            Spacing offsideSpacing = DEFAULT_OFFSIDE_SPACING
        )
            : base(name, minWidth, maxWidth, minHeight, maxHeight, margin)
        {
            _orientation = orientation;
            _horizontalAlignment = horizontalAlignment;
            _verticalAlignment = verticalAlignment;
            _inlineSpacing = inlineSpacing;
            _offsideSpacing = offsideSpacing;
        }

        protected override int MaximumChildren
        {
            get
            {
                return int.MaxValue;
            }
        }

        internal override IEnumerable<Solver.Solution> Solve(float left, float right, float top, float bottom)
        {
            var self = FillSpace(left, right, top, bottom);
            yield return self;

            throw new NotImplementedException();
        }

        internal override void Prepare()
        {
            base.Prepare();

            if (Orientation == Orientation.Horizontal)
            {
                //In all modes, we can't be any narrower/shorter than the sum of the min of all child elements
                _minWidth = Children.Select(a => a.MinWidth).Sum();
                _minHeight = Children.Select(a => a.MinHeight).Sum();

                switch (InlineSpacing)
                {
                    //In all these modes we're allowed empty space, so the max is the configured max
                    case Spacing.Maximize:
                    case Spacing.Minimize:
                    case Spacing.Uniform:
                        _maxWidth = null;
                        break;

                    //We're not allowed any spacing, therefore the max width is the min(configured max, sum(child max widths))
                    case Spacing.None:
                        _maxWidth = Math.Min(base.MaxWidth, Children.Select(a => a.MaxWidth).Sum());
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Stack Spacing Mode {0} Not Implemented", InlineSpacing));
                }

                switch (OffsideSpacing)
                {
                    //In all these modes we're allowed empty space, so the max is the configured max
                    case Spacing.Maximize:
                    case Spacing.Minimize:
                    case Spacing.Uniform:
                        _maxHeight = null;
                        break;

                    //We're not allowed any spacing, therefore the max height is the min(max child heights)
                    case Spacing.None:
                        _maxWidth = Children.Select(a => a.MaxHeight).Min();
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Stack Spacing Mode {0} Not Implemented", InlineSpacing));
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal override BaseElementContainer Wrap()
        {
            return new StackContainer(this);
        }
    }

    internal class StackContainer
        : BaseElement.BaseElementContainer
    {
        public Orientation Orientation { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public Spacing InlineSpacing { get; set; }

        public Spacing OffsideSpacing { get; set; }

        public StackContainer()
        {
            Orientation = Stack.DEFAULT_ORIENTATION;
            HorizontalAlignment = Stack.DEFAULT_HORIZONTAL_ALIGNMENT;
            VerticalAlignment = Stack.DEFAULT_VERTICAL_ALIGNMENT;
            InlineSpacing = Stack.DEFAULT_INLINE_SPACING;
            OffsideSpacing = Stack.DEFAULT_OFFSIDE_SPACING;
        }

        public StackContainer(Stack stack)
            : base(stack)
        {
            Orientation = stack.Orientation;
            HorizontalAlignment = stack.HorizontalAlignment;
            VerticalAlignment = stack.VerticalAlignment;
            InlineSpacing = stack.InlineSpacing;
            OffsideSpacing = stack.OffsideSpacing;
        }

        public override BaseElement Unwrap()
        {
            var s = new Stack(name: Name,
                minWidth: MinWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                maxHeight: MaxHeight,
                margin: (Margin ?? new MarginContainer()).Unwrap(),
                orientation: Orientation,
                horizontalAlignment: HorizontalAlignment,
                verticalAlignment: VerticalAlignment,
                inlineSpacing: InlineSpacing,
                offsideSpacing: OffsideSpacing
            );

            foreach (var child in Children)
                s.Add(child.Unwrap());

            return s;
        }
    }
}
