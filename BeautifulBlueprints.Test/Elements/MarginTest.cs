using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class MarginTest
    {
        [TestMethod]
        public void AssertThat_MarginElement_ExpandsToFillSpace()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Margin());

            var sol = solution.Single();

            var width = sol.Right - sol.Left;
            var height = sol.Top - sol.Bottom;

            var ratio = width / height;
            Assert.AreEqual(1, ratio);
        }

        [TestMethod]
        public void AssertThat_MarginElement_ExpandsToFillSpace_WithMargins()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Margin(
                left: new Size(1, 2, 3),
                right: new Size(2, 3, 4),
                top: new Size(3, 4, 5)
            ) {
                new Space()
            }).ToArray();

            Assert.AreEqual(2, solution.Length);

            var sol = solution[1];

            var marginLeft = sol.Left;
            Assert.IsTrue(marginLeft >= 1 && marginLeft <= 2);

            var marginRight = 100 - sol.Right;
            Assert.IsTrue(marginRight >= 2 && marginRight <= 4);

            var marginTop = 100 - sol.Top;
            Assert.IsTrue(marginTop >= 3 && marginTop <= 5);

            var marginBottom = sol.Bottom;
            Assert.IsTrue(marginBottom == 0);
        }

        [TestMethod]
        public void AssertThat_MarginElement_MakesChildAsLargeAsPossible_WhenSizeIsLessThanPreferred()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Margin(
                left: new Size(1, 2, 3),
                right: new Size(2, 3, 4),
                top: new Size(3, 4, 5)
            ) {
                new Space(preferredWidth: 200, preferredHeight: 200)
            }).ToArray();

            Assert.AreEqual(2, solution.Length);

            var sol = solution[1];
            Assert.AreEqual(1, sol.Left);
            Assert.AreEqual(98, sol.Right);
            Assert.AreEqual(97, sol.Top);
            Assert.AreEqual(0, sol.Bottom);
        }

        [TestMethod]
        public void AssertThat_MarginELement_StretchesMarginsOverPreferred_WhenChildMaxIsSmall()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Margin(
                left: new Size(1, 1, 40),
                right: new Size(1, 1, 40),
                top: new Size(1, 1, 40),
                bottom: new Size(1, 1, 40)
            ) {
                new Space(maxWidth: 50, maxHeight: 40)
            }).ToArray();

            Assert.AreEqual(2, solution.Length);

            var sol = solution[1];
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(25, 100 - sol.Right);
            Assert.AreEqual(30, 100 - sol.Top);
            Assert.AreEqual(30, sol.Bottom);
        }
    }
}
