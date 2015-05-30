using BeautifulBlueprints.Layout;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class Space
        : BaseElement
    {
        public Space(
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

        /// <summary>
        /// Fill up as much of the available space as possible
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        internal override IEnumerable<Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom)
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
                preferredWidth: PreferredWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight,
                maxHeight: MaxHeight
            );

            return s;
        }
    }
}
