using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using BeautifulBlueprints.Layout.Svg;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class PathTest
    {
        [TestMethod]
        public void AsserThat_PathElement_ExpandsToFillSpace()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Path("m0,0 l0,1 h1 v-1 z")).ToArray();

            Assert.AreEqual(1, solution.Count());
            Assert.AreEqual(0, solution.Single().Left);
            Assert.AreEqual(100, solution.Single().Right);
            Assert.AreEqual(100, solution.Single().Top);
            Assert.AreEqual(0, solution.Single().Bottom);
        }

        [TestMethod]
        public void AsserThat_PathElement_LaysOutBasicRectanglePath()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Path("m0,0 l0,1 h1 v0 z")).ToArray();

            Assert.IsNotNull(solution.Single().Tag);
            var layout = (PathLayout)solution.Single().Tag;

            var points = layout.Points.ToArray();

            Assert.AreEqual(50, points[0].X);
            Assert.AreEqual(50, points[0].Y);
            Assert.IsTrue(points[0].StartOfLine);

            Assert.AreEqual(50, points[1].X);
            Assert.AreEqual(100, points[1].Y);
            Assert.IsFalse(points[1].StartOfLine);

            Assert.AreEqual(100, points[2].X);
            Assert.AreEqual(100, points[2].Y);
            Assert.IsFalse(points[2].StartOfLine);

            Assert.AreEqual(100, points[3].X);
            Assert.AreEqual(50, points[3].Y);
            Assert.IsFalse(points[3].StartOfLine);

            Assert.AreEqual(50, points[4].X);
            Assert.AreEqual(50, points[4].Y);
            Assert.IsFalse(points[4].StartOfLine);
        }
    }
}
