using System.Linq;
using BeautifulBlueprints.Layout;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class Fallback
        : BaseContainerElement
    {
        public override decimal PreferredWidth
        {
            get
            {
                if (ChildCount == 0)
                    return 0;
                return Children.Select(a => a.PreferredWidth).Max();
            }
        }

        public override decimal PreferredHeight
        {
            get
            {
                if (ChildCount == 0)
                    return 0;
                return Children.Select(a => a.PreferredHeight).Max();
            }
        }

        public Fallback(
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

        protected override int MaximumChildren
        {
            get
            {
                return int.MaxValue;
            }
        }

        internal override IEnumerable<Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom)
        {
            List<Solver.Solution> solutions = new List<Solver.Solution>();

            var self = FillSpace(left, right, top, bottom);
            solutions.Add(self);

            foreach (var child in Children)
            {
                try
                {
                    solutions.AddRange(child.Solve(self.Left, self.Right, self.Top, self.Bottom));
                    return solutions;
                }
                catch (LayoutFailureException)
                {
                    //Eat exception, we want to keep falling back until we find something which does not fail
                }
            }

            return new Solver.Solution[0];
        }

        internal override BaseElementContainer Wrap()
        {
            return new FallbackContainer(this);
        }
    }

    internal class FallbackContainer
        : BaseContainerElement.BaseContainerElementContainer
    {
        public FallbackContainer()
        {
        }

        public FallbackContainer(Fallback fallback)
            : base(fallback)
        {
        }

        public override BaseElement Unwrap()
        {
            var s = new Fallback(name: Name,
                minWidth: MinWidth,
                preferredWidth: PreferredWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight,
                maxHeight: MaxHeight
            );

            UnwrapChildren(s);

            return s;
        }
    }
}
