//using System;
//using System.IO;
//using System.Linq;
//using System.Text;
//using BeautifulBlueprints.Elements;
//using BeautifulBlueprints.Layout;
//using BeautifulBlueprints.Serialization;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace BeautifulBlueprints.Test.Elements
//{
//    [TestClass]
//    public class FlowTest
//    {
//        [TestMethod]
//        public void AssertThat_FlowElement_ExpandsToFillSpace()
//        {
//            var solution = Solver.Solve(0, 100, 100, 0, new Flow()).ToArray();

//            Assert.AreEqual(1, solution.Count());
//            Assert.AreEqual(0, solution.Single().Left);
//            Assert.AreEqual(100, solution.Single().Right);
//            Assert.AreEqual(100, solution.Single().Top);
//            Assert.AreEqual(0, solution.Single().Bottom);
//        }

//        [TestMethod]
//        public void AssertThat_FlowElement_CanSerializeAndDeserialize_WithDefaults()

//        {
//            var element = new Flow();

//            StringBuilder builder = new StringBuilder();
//            Yaml.Serialize(element, new StringWriter(builder));

//            Console.WriteLine(builder.ToString());

//            var deserialized = (Flow)Yaml.Deserialize(new StringReader(builder.ToString()));

//            Assert.AreEqual(element.Spacing, deserialized.Spacing);
//            Assert.AreEqual(element.HorizontalAlignment, deserialized.HorizontalAlignment);
//            Assert.AreEqual(element.VerticalAlignment, deserialized.VerticalAlignment);
//            Assert.AreEqual(element.Orientation, deserialized.Orientation);
//            Assert.AreEqual(element.Margin, deserialized.Margin);
//            Assert.AreEqual(element.MaxHeight, deserialized.MaxHeight);
//            Assert.AreEqual(element.MinHeight, deserialized.MinHeight);
//            Assert.AreEqual(element.MaxWidth, deserialized.MaxWidth);
//            Assert.AreEqual(element.MinWidth, deserialized.MinWidth);
//        }
//    }
//}
