using BeautifulBlueprints.Layout;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class Fallback
        : BaseElement
    {
        public Fallback(
            string name = null,
            float minWidth = DEFAULT_MIN_WIDTH,
            float preferredWidth = DEFAULT_PREFERRED_WIDTH,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float preferredHeight = DEFAULT_PREFERRED_HEIGHT,
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
        : BaseElement.BaseElementContainer
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
                preferredWidth: PreferredWidth ?? BaseElement.DEFAULT_PREFERRED_WIDTH,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight ?? BaseElement.DEFAULT_PREFERRED_HEIGHT,
                maxHeight: MaxHeight,
                margin: (Margin ?? new MarginContainer()).Unwrap()
            );

            if (Children != null)
            {
                foreach (var child in Children)
                    s.Add(child.Unwrap());
            }

            return s;
        }
    }
}
