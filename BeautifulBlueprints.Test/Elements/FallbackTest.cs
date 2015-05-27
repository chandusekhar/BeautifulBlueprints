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
    public class FallbackTest
    {
        [TestMethod]
        public void AssertThat_FallbackElement_ReturnsFirstNonFailingChild()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Fallback() {
                new Space("200", minWidth: 200),
                new Space("100", minWidth: 150),
                new Space("50", minWidth: 50),
            }).ToArray();

            Assert.AreEqual(2, solution.Count());

            var fb = solution.Single(a => a.Element is Fallback);
            Assert.AreEqual(0, fb.Left);
            Assert.AreEqual(100, fb.Right);
            Assert.AreEqual(100, fb.Top);
            Assert.AreEqual(0, fb.Bottom);

            var sp = solution.Single(a => a.Element is Space);
            Assert.AreEqual("50", sp.Element.Name);
            Assert.AreEqual(0, sp.Left);
            Assert.AreEqual(100, sp.Right);
            Assert.AreEqual(100, sp.Top);
            Assert.AreEqual(0, sp.Bottom);
        }

        [TestMethod]
        public void AssertThat_FallbackElement_CanSerializeAndDeserialize_WithDefaults()
        {
            var element = new Fallback();

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(element, new StringWriter(builder));

            Console.WriteLine(builder.ToString());

            var deserialized = (Fallback)Yaml.Deserialize(new StringReader(builder.ToString()));

            Assert.AreEqual(element.Margin, deserialized.Margin);
            Assert.AreEqual(element.MaxHeight, deserialized.MaxHeight);
            Assert.AreEqual(element.MinHeight, deserialized.MinHeight);
            Assert.AreEqual(element.MaxWidth, deserialized.MaxWidth);
            Assert.AreEqual(element.MinWidth, deserialized.MinWidth);
        }
    }
}
