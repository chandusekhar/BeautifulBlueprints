
using System.Linq;
using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class RepeatTest
    {
        [TestMethod]
        public void AssertThat_RepeatElement_ExpandsToFillSpace()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Repeat(minimizeRepeats: false)).ToArray();

            Assert.AreEqual(1, solution.Count());
            Assert.AreEqual(0, solution.Single().Left);
            Assert.AreEqual(100, solution.Single().Right);
            Assert.AreEqual(100, solution.Single().Top);
            Assert.AreEqual(0, solution.Single().Bottom);
        }

        [TestMethod]
        public void AssertThat_RepeatElement_OutputsNoChildren_WhenTooNarrow()
        {
            var solution = Solver.Solve(0, 10, 10, 0, new Repeat(minimizeRepeats: false) {
                new Space(minWidth: 20)
            }).ToArray();

            Assert.AreEqual(1, solution.Count());
        }

        [TestMethod]
        public void AssertThat_RepeatElement_OutputsNoChildren_WhenTooShort()
        {
            var solution = Solver.Solve(0, 10, 10, 0, new Repeat(orientation: Orientation.Vertical, minimizeRepeats: false) {
                new Space(minHeight: 20)
            }).ToArray();

            Assert.AreEqual(1, solution.Count());
        }

        [TestMethod]
        public void AssertThat_RepeatElement_OutputsSingleChild_WhenCorrectWidth()
        {
            var solution = Solver.Solve(0, 10, 10, 0, new Repeat(minimizeRepeats: false) {
                new Space(minWidth: 7.5f)
            }).ToArray();

            Assert.AreEqual(2, solution.Count());
        }

        [TestMethod]
        public void AssertThat_RepeatElement_OutputsSingleChild_WhenCorrectHeight()
        {
            var solution = Solver.Solve(0, 10, 10, 0, new Repeat(orientation: Orientation.Vertical, minimizeRepeats: false) {
                new Space(minHeight: 7.5f)
            }).ToArray();

            Assert.AreEqual(2, solution.Count());
        }

        [TestMethod]
        public void AssertThat_RepeatElement_TwoChildren_WhenCorrectWidth()
        {
            var solution = Solver.Solve(0, 10, 10, 0, new Repeat(minimizeRepeats: false) {
                new Space(minWidth: 4, maxWidth: 11)
            }).ToArray();

            Assert.AreEqual(3, solution.Count());
        }

        [TestMethod]
        public void AssertThat_RepeatElement_SingleChildren_WhenMinimizingRepeats()
        {
            // Min width is 4, and space available is 10 - so if we want we can fit in 2 children here
            // The test immediately above this one (AssertThat_RepeatElement_TwoChildren_WhenCorrectWidth) does exactly this.
            // However, this time we've set minimizeRepeats, which will see if we can reduce the count as much as possible
            // Since max width is eleven we can indeed reduce the count and have just one item 10 wide

            var solution = Solver.Solve(0, 10, 10, 0, new Repeat(minimizeRepeats: true) {
                new Space(minWidth: 4, maxWidth: 11)
            }).ToArray();

            Assert.AreEqual(2, solution.Count());
        }

        [TestMethod]
        public void AssertThat_RepeatElement_TwoChildren_WhenCorrectHeight()
        {
            var solution = Solver.Solve(0, 10, 10, 0, new Repeat(orientation: Orientation.Vertical, minimizeRepeats: false) {
                new Space(minHeight: 4)
            }).ToArray();

            Assert.AreEqual(3, solution.Count());
        }
    }
}
