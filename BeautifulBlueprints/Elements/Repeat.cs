using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BeautifulBlueprints.Layout;

namespace BeautifulBlueprints.Elements
{
    public class Repeat
        : BaseContainerElement
    {
        internal const Orientation DEFAULT_ORIENTATION = Orientation.Horizontal;
        internal const bool DEFAULT_MINIMIZE_REPEATS = true;
        internal const bool DEFAULT_ALLOW_ZERO_REPEATS = true;

        private readonly Orientation _orientation;
        public Orientation Orientation { get { return _orientation; } }

        private readonly bool _minimizeRepeats;
        public bool MinimizeRepeats { get { return _minimizeRepeats; } }

        private readonly bool _allowZeroRepeats;
        public bool AllowZeroRepeats { get { return _allowZeroRepeats; } }

        public Repeat(
            bool minimizeRepeats = DEFAULT_MINIMIZE_REPEATS,
            bool allowZeroRepeats = DEFAULT_ALLOW_ZERO_REPEATS,
            Orientation orientation = DEFAULT_ORIENTATION,
            string name = null,
            float minWidth = DEFAULT_MIN_WIDTH,
            float? preferredWidth = null,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float? preferredHeight = null,
            float maxHeight = DEFAULT_MAX_HEIGHT,
            Margin margin = null
        )
            : base(name, minWidth, preferredWidth, maxWidth, minHeight, preferredHeight, maxHeight, margin)

        {
            _minimizeRepeats = minimizeRepeats;
            _allowZeroRepeats = allowZeroRepeats;
            _orientation = orientation;
        }

        internal override IEnumerable<Solver.Solution> Solve(float left, float right, float top, float bottom)
        {
            List<Solver.Solution> solutions = new List<Solver.Solution>();

            var self = FillSpace(left, right, top, bottom);
            solutions.Add(self);

            //Early exit for no children case
            var child = Children.SingleOrDefault();
            if (child == null)
                return solutions;

            

            //How many whole fits can we squeeze in at the minimum extent?
            int repeatCount;

            if (MinimizeRepeats)
            {
                //How few elements can we fit, if we stretch them as much as possible?
                //How many whole elements can we fit, if we squeeze them up as much as possible?
                var maxExtent = (Orientation == Orientation.Horizontal ? child.MaxWidth : child.MaxHeight);
                if (Math.Abs(maxExtent) < float.Epsilon)
                    throw new LayoutFailureException("Repeat cannot contain an element with 0 maximum size", this);

                repeatCount = (int)Math.Ceiling((Orientation == Orientation.Horizontal ? (self.Right - self.Left) : (self.Top - self.Bottom)) / maxExtent);
            }
            else
            {
                //How many whole elements can we fit, if we squeeze them up as much as possible?
                var minExtent = (Orientation == Orientation.Horizontal ? child.MinWidth : child.MinHeight);
                if (Math.Abs(minExtent) < float.Epsilon)
                    throw new LayoutFailureException("Repeat cannot contain an element with 0 minimum size", this);

                repeatCount = (int)Math.Floor((Orientation == Orientation.Horizontal ? (self.Right - self.Left) : (self.Top - self.Bottom)) / minExtent);
            }

            if (repeatCount == 0 && !AllowZeroRepeats)
                throw new LayoutFailureException("Repeat element repeats zero times, but \"AllowZeroRepeats\" is false", this);

            //Solve the child multiple times, once for each repeat
            if (Orientation == Orientation.Horizontal)
            {
                var childWidth = (self.Right - self.Left) / repeatCount;
                for (int i = 0; i < repeatCount; i++)
                    solutions.AddRange(child.Solve(self.Left + childWidth * i, self.Left + childWidth * (i + 1), self.Top, self.Bottom));
            }
            else
            {
                var childHeight = (self.Top - self.Bottom) / repeatCount;
                for (int i = 0; i < repeatCount; i++)
                    solutions.AddRange(child.Solve(self.Left, self.Right, self.Top - childHeight * i, self.Top - childHeight * (i + 1)));
            }

            return solutions;
        }

        protected override int MaximumChildren
        {
            get
            {
                return 1;
            }
        }

        internal override BaseElementContainer Wrap()
        {
            return new RepeatContainer(this);
        }
    }

    internal class RepeatContainer
        : BaseContainerElement.BaseContainerElementContainer
    {
        [DefaultValue(Repeat.DEFAULT_ORIENTATION)]
        public Orientation Orientation { get; set; }

        [DefaultValue(Repeat.DEFAULT_MINIMIZE_REPEATS)]
        public bool MinimizeRepeats { get; set; }

        [DefaultValue(Repeat.DEFAULT_ALLOW_ZERO_REPEATS)]
        public bool AllowZeroRepeats { get; set; }

        public RepeatContainer()
        {
        }

        public RepeatContainer(Repeat split)
            : base(split)
        {
            Orientation = split.Orientation;
            MinimizeRepeats = split.MinimizeRepeats;
            AllowZeroRepeats = split.AllowZeroRepeats;
        }

        public override BaseElement Unwrap()
        {
            var s = new Repeat(
                minimizeRepeats: MinimizeRepeats,
                allowZeroRepeats: AllowZeroRepeats,
                orientation: Orientation,
                name: Name,
                minWidth: MinWidth,
                preferredWidth: PreferredWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight,
                maxHeight: MaxHeight,
                margin: (Margin ?? new MarginContainer()).Unwrap()
            );

            UnwrapChildren(s);

            return s;
        }
    }
}
