using BeautifulBlueprints.Layout;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class Fallback
        : BaseContainerElement
    {
        public Fallback(
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
                maxHeight: MaxHeight,
                margin: (Margin ?? new MarginContainer()).Unwrap()
            );

            UnwrapChildren(s);

            return s;
        }
    }
}
