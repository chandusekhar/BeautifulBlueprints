using BeautifulBlueprints.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BeautifulBlueprints.Layout
{
    public static class Solver
    {
        public static IEnumerable<Solution> Solve(float left, float right, float top, float bottom, BaseElement root)
        {
            root.Prepare();

            Solution[] solutions;
            try
            {
                solutions = root.Solve(left, right, top, bottom).ToArray();
            }
            catch (LayoutFailureException)
            {
                yield break;
            }

            foreach (var solution in solutions)
                yield return solution;
        }

        public struct Solution
        {
            public BaseElement Element { get; private set; }

            public float Left { get; private set; }
            public float Right { get; private set; }
            public float Top { get; private set; }
            public float Bottom { get; private set; }

            public Solution(BaseElement element, float left, float right, float top, float bottom)
                : this()
            {
                Element = element;
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
        }
    }
}
