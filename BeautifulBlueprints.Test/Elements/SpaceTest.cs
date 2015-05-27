using System;
using System.Linq;
using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class SpaceTest
    {
        [TestMethod]
        public void AssertThat_SpaceElement_ExpandsToFillSpace()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Space()).ToArray();

            Assert.AreEqual(1, solution.Count());
            Assert.AreEqual(0, solution.Single().Left);
            Assert.AreEqual(100, solution.Single().Right);
            Assert.AreEqual(100, solution.Single().Top);
            Assert.AreEqual(0, solution.Single().Bottom);
        }

        [TestMethod]
        public void AssertThat_SpaceElement_ExpandsToFillSpaceWithinMargin()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Space(margin: new Margin(left: 10, bottom: 10, right: 10, top: 10))).ToArray();

            Assert.AreEqual(1, solution.Count());
            Assert.AreEqual(10, solution.Single().Left);
            Assert.AreEqual(90, solution.Single().Right);
            Assert.AreEqual(90, solution.Single().Top);
            Assert.AreEqual(10, solution.Single().Bottom);
        }

        [TestMethod]
        public void AssertThat_SpaceElement_DoesNot_ExceedMaxWidth()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Space(maxWidth: 50)).ToArray();

            Assert.AreEqual(0, solution.Count());
        }

        [TestMethod]
        public void AssertThat_SpaceElement_DoesNot_ExceedMaxHeight()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Space(maxHeight: 50)).ToArray();

            Assert.AreEqual(0, solution.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AssertThat_SpaceElement_DoesNot_AllowChildren()
        {
            new Space().Add(new Space());
        }
    }
}
