using System.Linq;
using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class StackTest
    {
        [TestMethod]
        public void AssertThat_StackElement_ExpandsToFillSpace()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Stack()).ToArray();

            Assert.AreEqual(1, solution.Count());
            Assert.AreEqual(0, solution.Single().Left);
            Assert.AreEqual(100, solution.Single().Right);
            Assert.AreEqual(100, solution.Single().Top);
            Assert.AreEqual(0, solution.Single().Bottom);
        }

        [TestMethod]
        public void AssertThat_StackElement_LaysOutChildrenWithMinimumSpace()
        {
            var s = new Stack(inlineSpacing: Spacing.Minimize) {
                new Space(minWidth: 10, maxWidth: 20),
                new Space(minWidth: 10, maxWidth: 20),
                new Space(minWidth: 10, maxWidth: 20),
                new Space(minWidth: 10, maxWidth: 20),
            };

            var solution = Solver.Solve(0, 100, 100, 0, s).ToArray();

            Assert.AreEqual(5, solution.Length);

            Assert.AreEqual(0, solution.Where(a => a.Element == s).Single().Left);
            Assert.AreEqual(100, solution.Where(a => a.Element == s).Single().Right);
            Assert.AreEqual(100, solution.Where(a => a.Element == s).Single().Top);
            Assert.AreEqual(0, solution.Where(a => a.Element == s).Single().Bottom);

            foreach (var el in solution.Where(a => a.Element != s))
                Assert.AreEqual(20, el.Right - el.Left);
        }
    }
}
