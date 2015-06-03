using System;
using System.Collections.Generic;
using BeautifulBlueprints.Layout;

namespace BeautifulBlueprints.Elements
{
    public class Overlap
        : BaseContainerElement
    {
        public Overlap(
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
        }

        internal override IEnumerable<Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom)
        {
            var solutions = new List<Solver.Solution>();

            var self = FillSpace(left, right, top, bottom);
            solutions.Add(self);

            foreach (var child in Children)
                solutions.AddRange(child.Solve(self.Left, self.Right, self.Top, self.Bottom));

            return solutions;
        }

        internal override BaseElementContainer Wrap()
        {
            return new OverlapContainer(this);
        }

        protected override int MaximumChildren
        {
            get
            {
                return int.MaxValue;
            }
        }
    }

    internal class OverlapContainer
        : BaseContainerElement.BaseContainerElementContainer
    {
        public OverlapContainer(Overlap overlap)
            : base(overlap)
        {
        }

        public override BaseElement Unwrap()
        {
            return UnwrapChildren(new Overlap(
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
