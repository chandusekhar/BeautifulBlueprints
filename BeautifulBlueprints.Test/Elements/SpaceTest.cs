﻿using System;
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
        [ExpectedException(typeof(LayoutFailureException))]
        public void AssertThat_SpaceElement_DoesNot_ExceedMaxWidth()
        {
            Solver.Solve(0, 100, 100, 0, new Space(maxWidth: 50));
        }

        [TestMethod]
        [ExpectedException(typeof(LayoutFailureException))]
        public void AssertThat_SpaceElement_DoesNot_ExceedMaxHeight()
        {
            Solver.Solve(0, 100, 100, 0, new Space(maxHeight: 50));
        }

        [TestMethod]
        public void AssertThat_SpaceElement_CanSerializeAndDeserialize_WithDefaults()
        {
            var element = new Space();

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(new LayoutContainer(element), new StringWriter(builder));

            Console.WriteLine(builder.ToString());

            var deserialized = (Space)Yaml.Deserialize(new StringReader(builder.ToString())).Root;

            Assert.AreEqual(element.MaxHeight, deserialized.MaxHeight);
            Assert.AreEqual(element.MinHeight, deserialized.MinHeight);
            Assert.AreEqual(element.MaxWidth, deserialized.MaxWidth);
            Assert.AreEqual(element.MinWidth, deserialized.MinWidth);
        }
    }
}
