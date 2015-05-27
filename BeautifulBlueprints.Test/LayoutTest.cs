using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BeautifulBlueprints.Test
{
    [TestClass]
    public class LayoutTest
    {
        [TestMethod]
        public void MostBasicLayout()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Space());

            Assert.AreEqual(1, solution.Count());
            Assert.AreEqual(0, solution.Single().Left);
            Assert.AreEqual(100, solution.Single().Right);
            Assert.AreEqual(100, solution.Single().Top);
            Assert.AreEqual(0, solution.Single().Bottom);
        }
    }
}
