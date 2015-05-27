using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class AspectRatioTest
    {
        [TestMethod]
        public void AssertThat_AspectRatioElement_ExpandsToFillSpace_WithCorrectRatio()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 2)).ToArray();

            var sol = solution.Single();

            var width = sol.Right - sol.Left;
            var height = sol.Top - sol.Bottom;

            var ratio = width / height;
            Assert.AreEqual(2, ratio);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_FailsLayout_WithImpossibleHeightConstraint()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 2, minHeight: 90)).ToArray();

            Assert.AreEqual(0, solution.Length);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_FailsLayout_WithImpossibleWidthConstraint()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 0.5f, minWidth: 90)).ToArray();

            Assert.AreEqual(0, solution.Length);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_VerticallyAlignsToCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 2)).ToArray();

            var sol = solution.Single();

            //Fills up all width
            Assert.AreEqual(0, sol.Left);
            Assert.AreEqual(100, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_HorizontallyAlignsToCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 0.5f)).ToArray();

            var sol = solution.Single();

            //Fills up all width
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(0, sol.Bottom);
            Assert.AreEqual(100, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_FloatsCorrectly_WithAlignmentCenterCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50, horizontalAlignment: HorizontalAlignment.Center)).ToArray();

            var sol = solution.Single();

            //Fills up all width
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_FloatsCorrectly_WithAlignmentLeftCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50, horizontalAlignment: HorizontalAlignment.Left)).ToArray();

            var sol = solution.Single();

            //Fills up all width
            Assert.AreEqual(0, sol.Left);
            Assert.AreEqual(50, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_FloatsCorrectly_WithAlignmentRightCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50, horizontalAlignment: HorizontalAlignment.Right)).ToArray();

            var sol = solution.Single();

            //Fills up all width
            Assert.AreEqual(50, sol.Left);
            Assert.AreEqual(100, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_FloatsCorrectly_WithAlignmentCenterTop()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50, verticalAlignment: VerticalAlignment.Top)).ToArray();

            var sol = solution.Single();

            //Fills up all width
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(50, sol.Bottom);
            Assert.AreEqual(100, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_FloatsCorrectly_WithAlignmentCenterBottom()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50, verticalAlignment: VerticalAlignment.Bottom)).ToArray();

            var sol = solution.Single();

            //Fills up all width
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(0, sol.Bottom);
            Assert.AreEqual(50, sol.Top);
        }
    }
}
