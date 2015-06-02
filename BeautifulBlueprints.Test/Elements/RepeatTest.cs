using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class RepeatTest
    {
        [TestMethod]
        public void AssertThat_RepeatElement_ExpandsToFillSpace()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Repeat(3)).ToArray();

            Assert.AreEqual(1, solution.Count());
            Assert.AreEqual(0, solution.Single().Left);
            Assert.AreEqual(100, solution.Single().Right);
            Assert.AreEqual(100, solution.Single().Top);
            Assert.AreEqual(0, solution.Single().Bottom);
        }

        [TestMethod]
        public void AssertThat_RepeatElement_RepeatsChildInEqualSpaces()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Repeat(3) {
                new Space()
            }).ToArray();

            Assert.AreEqual(4, solution.Count());

            Assert.AreEqual(0, solution.First().Left);
            Assert.AreEqual(100, solution.First().Right);
            Assert.AreEqual(100, solution.First().Top);
            Assert.AreEqual(0, solution.First().Bottom);

            Assert.AreEqual(0, solution.Skip(1).First().Left);
            Assert.AreEqual(100 / 3m, solution.Skip(1).First().Right);
            Assert.AreEqual(100, solution.Skip(1).First().Top);
            Assert.AreEqual(0, solution.Skip(1).First().Bottom);

            Assert.AreEqual(100 / 3m, solution.Skip(2).First().Left);
            Assert.IsTrue((200 / 3m).IsEqualTo(solution.Skip(2).First().Right));
            Assert.AreEqual(100, solution.Skip(2).First().Top);
            Assert.AreEqual(0, solution.Skip(2).First().Bottom);

            Assert.IsTrue((200 / 3m).IsEqualTo(solution.Skip(3).First().Left));
            Assert.IsTrue((300 / 3m).IsEqualTo(solution.Skip(3).First().Right));
            Assert.AreEqual(100, solution.Skip(3).First().Top);
            Assert.AreEqual(0, solution.Skip(3).First().Bottom);
        }
    }
}
