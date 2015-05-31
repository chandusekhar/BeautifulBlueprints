using BeautifulBlueprints.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BeautifulBlueprints.Layout
{
    public static class Solver
    {
        public static IEnumerable<Solution> Solve(decimal left, decimal right, decimal top, decimal bottom, BaseElement root)
        {
            root.Prepare();
            return root.Solve(left, right, top, bottom).ToArray();
        }

        public struct Solution
        {
            public BaseElement Element { get; private set; }

            public decimal Left { get; private set; }
            public decimal Right { get; private set; }
            public decimal Top { get; private set; }
            public decimal Bottom { get; private set; }

            public Solution(BaseElement element, decimal left, decimal right, decimal top, decimal bottom)
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
