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

        private readonly Orientation _orientation;
        public Orientation Orientation { get { return _orientation; } }

        private readonly uint _repeats;
        public uint Repeats { get { return _repeats; } }

        public Repeat(
            uint count,
            Orientation orientation = DEFAULT_ORIENTATION,
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
            _repeats = count;
            _orientation = orientation;
        }

        internal override IEnumerable<Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom)
        {
            List<Solver.Solution> solutions = new List<Solver.Solution>();

            var self = FillSpace(left, right, top, bottom);
            solutions.Add(self);

            //Early exit for no children case
            var child = Children.SingleOrDefault();
            if (child == null)
                return solutions;

            //Layout children
            solutions.AddRange(LayoutRepeats(Repeats, child, Orientation, self.Left, self.Right, self.Top, self.Bottom));

            return solutions;
        }

        protected override int MaximumChildren
        {
            get
            {
                return 1;
            }
        }

        public static IEnumerable<Solver.Solution> LayoutRepeats(uint count, BaseElement element, Orientation orientation, decimal left, decimal right, decimal top, decimal bottom)
        {
            //Solve the child multiple times, once for each repeat
            if (orientation == Orientation.Horizontal)
            {
                var childWidth = (right - left) / count;
                for (int i = 0; i < count; i++)
                    foreach (var solution in element.Solve(left + childWidth * i, left + childWidth * (i + 1), top, bottom))
                        yield return solution;
            }
            else
            {
                var childHeight = (top - bottom) / count;
                for (int i = 0; i < count; i++)
                    foreach (var solution in element.Solve(left, right, top - childHeight * i, top - childHeight * (i + 1)))
                        yield return solution;
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
        [DefaultValue(Fit.DEFAULT_ORIENTATION)]
        public Orientation Orientation { get; set; }

        public uint Repeats { get; set; }

        public RepeatContainer()
        {
        }

        public RepeatContainer(Repeat repeat)
            : base(repeat)
        {
            Orientation = repeat.Orientation;
            Repeats = repeat.Repeats;
        }

        public override BaseElement Unwrap()
        {
            return UnwrapChildren(new Repeat(
                count: Repeats,
                orientation: Orientation,
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
