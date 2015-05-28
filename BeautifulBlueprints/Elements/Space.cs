using BeautifulBlueprints.Layout;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class Space
        : BaseElement
    {
        public Space(
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
                return 0;
            }
        }

        /// <summary>
        /// Fill up as much of the available space as possible
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        internal override IEnumerable<Solver.Solution> Solve(float left, float right, float top, float bottom)
        {
            yield return FillSpace(left, right, top, bottom);
        }

        internal override BaseElementContainer Wrap()
        {
            return new SpaceContainer(this);
        }
    }

    internal class SpaceContainer
        : BaseElement.BaseElementContainer
    {
        public SpaceContainer()
        {
        }

        public SpaceContainer(Space space)
            : base(space)
        {
        }

        public override BaseElement Unwrap()
        {
            var s = new Space(name: Name,
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
