using BeautifulBlueprints.Layout;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class Space
        : BaseElement
    {
        public Space(
            string name = null,
            float minWidth = 0,
            float maxWidth = float.PositiveInfinity,
            float minHeight = 0,
            float maxHeight = float.PositiveInfinity,
            Margin margin = null
        )
            : base(name, minWidth, maxWidth, minHeight, maxHeight, margin)
        {
        }

        public Space()
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
    }

    internal class SpaceContainer
        : BaseElementContainer
    {
        public override BaseElement Unwrap()
        {
            var s = new Space(name: Name,
                minWidth: MinWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                maxHeight: MaxHeight,
                margin: (Margin ?? new MarginContainer()).Unwrap()
            );

            foreach (var child in Children)
                s.Add(child.Unwrap());

            return s;
        }
    }
}
