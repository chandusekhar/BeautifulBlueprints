﻿using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using BeautifulBlueprints.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class AspectRatioTest
    {
        [TestMethod]
        public void AssertThat_AspectRatioElement_ExpandsToFillSpace_WithCorrectRatio()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 2)).ToArray();

            var sol = solution.Single();

            var width = sol.Right - sol.Left;
            var height = sol.Top - sol.Bottom;

            var ratio = width / height;
            Assert.AreEqual(2, ratio);
        }

        [TestMethod]
        [ExpectedException(typeof(LayoutFailureException))]
        public void AssertThat_AspectRatioElement_FailsLayout_WithImpossibleHeightConstraint()
        {
            Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 2, minHeight: 90));
        }

        [TestMethod]
        [ExpectedException(typeof(LayoutFailureException))]
        public void AssertThat_AspectRatioElement_FailsLayout_WithImpossibleWidthConstraint()
        {
            Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 0.5m, minWidth: 90));
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_VerticallyAlignsToCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 2)).ToArray();

            var sol = solution.Single();

            //Fills up all width
            Assert.AreEqual(0, sol.Left);
            Assert.AreEqual(100, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_HorizontallyAlignsToCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 0.5m)).ToArray();

            var sol = solution.Single();

            //Fills up all width
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(0, sol.Bottom);
            Assert.AreEqual(100, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_decimalsCorrectly_WithAlignmentCenterCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to center
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_decimalsCorrectly_WithAlignmentLeftCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50, horizontalAlignment: HorizontalAlignment.Left)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to left
            Assert.AreEqual(0, sol.Left);
            Assert.AreEqual(50, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_decimalsCorrectly_WithAlignmentRightCenter()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50, horizontalAlignment: HorizontalAlignment.Right)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to right
            Assert.AreEqual(50, sol.Left);
            Assert.AreEqual(100, sol.Right);

            //Uses half of height, and aligns to center
            Assert.AreEqual(25, sol.Bottom);
            Assert.AreEqual(75, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_decimalsCorrectly_WithAlignmentCenterTop()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50, verticalAlignment: VerticalAlignment.Top)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to center
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to top
            Assert.AreEqual(50, sol.Bottom);
            Assert.AreEqual(100, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_decimalsCorrectly_WithAlignmentCenterBottom()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50, verticalAlignment: VerticalAlignment.Bottom)).ToArray();

            var sol = solution.Single();

            //Uses half of width, and aligns to center
            Assert.AreEqual(25, sol.Left);
            Assert.AreEqual(75, sol.Right);

            //Uses half of height, and aligns to bottom
            Assert.AreEqual(0, sol.Bottom);
            Assert.AreEqual(50, sol.Top);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_PositionsChildElementCorrectly()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new AspectRatio(ratio: 1, maxWidth: 50, maxHeight: 50) {
                new Space()
            }).ToArray();

            Assert.AreEqual(2, solution.Length);

            var ar = solution.Where(a => a.Element is AspectRatio).Single();
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
        public void AssertThat_AspectRatioElement_CanSerializeAndDeserialize_WithDefaults()
        {
            var element = new AspectRatio();

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(new LayoutContainer(element), new StringWriter(builder));
            Console.WriteLine(builder.ToString());

            var deserialized = (AspectRatio)Yaml.Deserialize(new StringReader(builder.ToString())).Root;

            Assert.AreEqual(element.HorizontalAlignment, deserialized.HorizontalAlignment);
            Assert.AreEqual(element.VerticalAlignment, deserialized.VerticalAlignment);
            Assert.AreEqual(element.MaxHeight, deserialized.MaxHeight);
            Assert.AreEqual(element.MinHeight, deserialized.MinHeight);
            Assert.AreEqual(element.MaxWidth, deserialized.MaxWidth);
            Assert.AreEqual(element.MinWidth, deserialized.MinWidth);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_ModifiesMinWidth_WithRespectToMinHeight()
        {
            var element = new AspectRatio(minWidth: 1, minHeight: 10, ratio: 0.5m);

            Assert.AreEqual(5, element.MinWidth);
        }

        [TestMethod]
        public void AssertThat_AspectRatioElement_ModifiesMinHeight_WithRespectToMinWidth()
        {
            var element = new AspectRatio(minWidth: 10, minHeight: 1, ratio: 0.5m);

            Assert.AreEqual(20, element.MinHeight);
        }
    }
}
