using System;
using System.IO;
using System.Text;
using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using BeautifulBlueprints.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class FloatTest
    {
        [TestMethod]
        public void AssertThat_FloatElement_VerticallyAlignsToCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Float { 
                new Space(maxHeight: 50)
            }).ToArray();

            var fl = solution.Single(a => a.Element is Float);

            //Fills up all width
            Assert.AreEqual(0, fl.Left);
            Assert.AreEqual(100, fl.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, fl.Bottom);
            Assert.AreEqual(75, fl.Top);
        }

        [TestMethod]
        public void AssertThat_FloatElement_HorizontallyAlignsToCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Float { 
                new Space(maxWidth: 50)
            }).ToArray();

            var fl = solution.Single(a => a.Element is Float);

            //Fills up all width
            Assert.AreEqual(25, fl.Left);
            Assert.AreEqual(75, fl.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(0, fl.Bottom);
            Assert.AreEqual(100, fl.Top);
        }

        [TestMethod]
        public void AssertThat_FloatElement_FloatsCorrectly_WithAlignmentCenterCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Float(maxWidth: 50, maxHeight: 50)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to center
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_FloatElement_FloatsCorrectly_WithAlignmentLeftCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Float(maxWidth: 50, maxHeight: 50, horizontalAlignment: HorizontalAlignment.Left)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to left
            Assert.AreEqual(0, sol.Left);
            Assert.AreEqual(50, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_FloatElement_FloatsCorrectly_WithAlignmentRightCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Float(maxWidth: 50, maxHeight: 50, horizontalAlignment: HorizontalAlignment.Right)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to right
            Assert.AreEqual(50, sol.Left);
            Assert.AreEqual(100, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_FloatElement_FloatsCorrectly_WithAlignmentCenterTop()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Float(maxWidth: 50, maxHeight: 50, verticalAlignment: VerticalAlignment.Top)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to center
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to top
            Assert.AreEqual(50, sol.Bottom);
            Assert.AreEqual(100, sol.Top);
        }

        [TestMethod]
        public void AssertThat_FloatElement_FloatsCorrectly_WithAlignmentCenterBottom()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Float(maxWidth: 50, maxHeight: 50, verticalAlignment: VerticalAlignment.Bottom)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to center
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to bottom
            Assert.AreEqual(0, sol.Bottom);
            Assert.AreEqual(50, sol.Top);
        }

        [TestMethod]
        public void AssertThat_FloatElement_PositionsChildElementCorrectly()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Float(maxWidth: 50, maxHeight: 50) {
                new Space()
            }).ToArray();

            Assert.AreEqual(2, solution.Length);

            var ar = solution.Where(a => a.Element is Float).Single();
            Assert.AreEqual(25, ar.Left);
            Assert.AreEqual(75, ar.Right);
            Assert.AreEqual(25, ar.Bottom);
            Assert.AreEqual(75, ar.Top);

            var spc = solution.Where(a => a.Element is Space).Single();
            Assert.AreEqual(25, spc.Left);
            Assert.AreEqual(75, spc.Right);
            Assert.AreEqual(25, spc.Bottom);
            Assert.AreEqual(75, spc.Top);
        }

        [TestMethod]
        public void AssertThat_FloatElement_CanSerializeAndDeserialize_WithDefaults()
        {
            var element = new Float();

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(element, new StringWriter(builder));

            Console.WriteLine(builder.ToString());

            var deserialized = (Float)Yaml.Deserialize(new StringReader(builder.ToString()));

            Assert.AreEqual(element.HorizontalAlignment, deserialized.HorizontalAlignment);
            Assert.AreEqual(element.VerticalAlignment, deserialized.VerticalAlignment);
            Assert.AreEqual(element.MaxHeight, deserialized.MaxHeight);
            Assert.AreEqual(element.MinHeight, deserialized.MinHeight);
            Assert.AreEqual(element.MaxWidth, deserialized.MaxWidth);
            Assert.AreEqual(element.MinWidth, deserialized.MinWidth);
        }
    }
}
