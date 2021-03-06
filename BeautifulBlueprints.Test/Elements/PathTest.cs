﻿using System;
using System.Text;
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
            var solution = Solver.Solve(0, 100, 100, 0, new Path("M0,0 L0,1 H1 V-1 Z")).ToArray();

            Assert.AreEqual(1, solution.Count());
            Assert.AreEqual(0, solution.Single().Left);
            Assert.AreEqual(100, solution.Single().Right);
            Assert.AreEqual(100, solution.Single().Top);
            Assert.AreEqual(0, solution.Single().Bottom);
        }

        [TestMethod]
        public void AsserThat_PathElement_LaysOutBasicRectanglePath()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Path("M0,0 L0,1 H1 V0 Z")).ToArray();

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

        [TestMethod]
        public void AssertThat_BezierCurve_IsContainedWithinParent()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Path("M-1 -1 C -10 1, 1 1, 1 -1")).ToArray();

            Assert.IsNotNull(solution.Single().Tag);
            var layout = (PathLayout)solution.Single().Tag;

            StringBuilder builder = new StringBuilder();
            foreach (var point in layout.Points)
            {
                //Assert that the points all lie within the boundary
                Assert.IsTrue(point.X >= 0 && point.X <= 100);
                Assert.IsTrue(point.Y >= 0 && point.Y <= 100);

                if (point.StartOfLine)
                    builder.Append(string.Format("M{0},{1} ", point.X, point.Y));
                else
                    builder.Append(string.Format("L{0},{1} ", point.X, point.Y));
            }
            Console.WriteLine(builder.ToString());
        }

        [TestMethod]
        public void AssertThat_QuadraticCurve_IsContainedWithinParent()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Path("M-1 -1 Q 0,1 1,-1")).ToArray();

            Assert.IsNotNull(solution.Single().Tag);
            var layout = (PathLayout)solution.Single().Tag;

            StringBuilder builder = new StringBuilder();
            foreach (var point in layout.Points)
            {
                //Assert that the points all lie within the boundary
                Assert.IsTrue(point.X >= 0 && point.X <= 100);
                Assert.IsTrue(point.Y >= 0 && point.Y <= 100);

                if (point.StartOfLine)
                    builder.Append(string.Format("M{0},{1} ", point.X, point.Y));
                else
                    builder.Append(string.Format("L{0},{1} ", point.X, point.Y));
            }
            Console.WriteLine(builder.ToString());
        }
    }
}
