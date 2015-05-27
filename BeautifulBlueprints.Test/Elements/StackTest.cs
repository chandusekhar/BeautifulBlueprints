using System;
using System.IO;
using System.Linq;
using System.Text;
using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using BeautifulBlueprints.Serialization;
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

        [TestMethod]
        public void AssertThat_StackElement_CanSerializeAndDeserialize_WithDefaults()
        {
            var element = new Stack();

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(element, new StringWriter(builder));

            Console.WriteLine(builder.ToString());

            var deserialized = (Stack)Yaml.Deserialize(new StringReader(builder.ToString()));

            Assert.AreEqual(element.InlineSpacing, deserialized.InlineSpacing);
            Assert.AreEqual(element.OffsideSpacing, deserialized.OffsideSpacing);
            Assert.AreEqual(element.HorizontalAlignment, deserialized.HorizontalAlignment);
            Assert.AreEqual(element.VerticalAlignment, deserialized.VerticalAlignment);
            Assert.AreEqual(element.Orientation, deserialized.Orientation);
            Assert.AreEqual(element.Margin, deserialized.Margin);
            Assert.AreEqual(element.MaxHeight, deserialized.MaxHeight);
            Assert.AreEqual(element.MinHeight, deserialized.MinHeight);
            Assert.AreEqual(element.MaxWidth, deserialized.MaxWidth);
            Assert.AreEqual(element.MinWidth, deserialized.MinWidth);
        }
    }
}
