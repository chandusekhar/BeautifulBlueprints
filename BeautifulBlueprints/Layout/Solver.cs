using BeautifulBlueprints.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BeautifulBlueprints.Layout
{
    public static class Solver
    {
        public static IEnumerable<Solution> Solve(decimal left, decimal right, decimal top, decimal bottom, BaseElement root, SolverOptions options = default(SolverOptions))
        {
            root.Prepare(options);
            return root.Solve(left, right, top, bottom).ToArray();
        }

        public delegate BaseElement SubsectionFinder(string name, string[] tags);

        public struct SolverOptions
        {
            public SubsectionFinder SubsectionFinder { get; set; }

            public SolverOptions(SubsectionFinder subsectionFinder = null)
                : this()
            {
                SubsectionFinder = subsectionFinder;
            }
        }

        public struct Solution
        {
            public BaseElement Element { get; private set; }

            public object Tag { get; internal set; }

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
