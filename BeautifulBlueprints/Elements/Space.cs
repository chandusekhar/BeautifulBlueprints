
using System.Collections.Generic;
using BeautifulBlueprints.Layout;

namespace BeautifulBlueprints.Elements
{
    public class Space
        : BaseElement
    {
        public Space(
            float minWidth = 0,
            float maxWidth = float.PositiveInfinity,
            float minHeight = 0,
            float maxHeight = float.PositiveInfinity,
            Margin? margin = null
        )
            : base(minWidth, maxWidth, minHeight, maxHeight, margin)
        {
        }

        public Space()
        {
        }

        protected override bool AllowChildren
        {
            get
            {
                return false;
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
            var width = (right - left) - (Margin.Left + Margin.Right);
            var height = (top - bottom) - (Margin.Top + Margin.Bottom);

            if (width < MinWidth || width > MaxWidth || height < MinHeight || height > MaxHeight)
                yield break;

            yield return new Solver.Solution(this, left + Margin.Left, right - Margin.Right, top - Margin.Top, bottom + Margin.Bottom);
        }
    }
}
