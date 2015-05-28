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
    public class GridTest
    {
        [TestMethod]
        public void AssertThat_GridElement_ExpandsToFillSpace()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Grid(
                rows: new[] { new GridRow(10, SizeMode.Fixed), new GridRow(10, SizeMode.Grow) },
                columns: new[] { new GridColumn(10, SizeMode.Fixed), new GridColumn(10, SizeMode.Grow) }
            ) {
                new Space("tl"), new Space("tr"), new Space("bl"), new Space("br")
            }).ToArray();

            Assert.AreEqual(5, solution.Length);

            //check grid fills entire space
            var grid = solution.Where(a => a.Element is Grid).Single();
            Assert.AreEqual(0, grid.Left);
            Assert.AreEqual(100, grid.Right);
            Assert.AreEqual(100, grid.Top);
            Assert.AreEqual(0, grid.Bottom);

            //We should get something like this:
            //
            // /----|---------------\
            // | TL |       TR      |
            // +----+---------------=
            // |    |               |
            // |    |               |
            // | BL |       BR      |
            // |    |               |
            // |    |               |
            // \----+---------------/
            //
            // Columns are (left to right) 10 and 90
            // Rows are (top to bottom) 10 and 90

            var tl = solution.Where(a => a.Element.Name == "tl").Single();
            var tr = solution.Where(a => a.Element.Name == "tr").Single();
            var bl = solution.Where(a => a.Element.Name == "bl").Single();
            var br = solution.Where(a => a.Element.Name == "br").Single();

            // Check that TL is 10x10 and sits in the top left
            Assert.AreEqual(100, tl.Top);
            Assert.AreEqual(90, tl.Bottom);
            Assert.AreEqual(0, tl.Left);
            Assert.AreEqual(10, tl.Right);

            //Check that TR is 90x10 and sits in the top right
            Assert.AreEqual(100, tr.Top);
            Assert.AreEqual(90, tr.Bottom);
            Assert.AreEqual(10, tr.Left);
            Assert.AreEqual(100, tr.Right);

            //Check that BL is 10x90 and sits in the bottom left
            Assert.AreEqual(90, bl.Top);
            Assert.AreEqual(0, bl.Bottom);
            Assert.AreEqual(0, bl.Left);
            Assert.AreEqual(10, bl.Right);

            //Check that BR is 90x90 and sits in the bottom right
            Assert.AreEqual(90, br.Top);
            Assert.AreEqual(0, br.Bottom);
            Assert.AreEqual(10, br.Left);
            Assert.AreEqual(100, br.Right);
        }

        [TestMethod]
        public void AssertThat_GridElement_FailsLayout_WhenFixedColSizesExceedParentWidth()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Grid(
                rows: new[] { new GridRow(10, SizeMode.Fixed) },
                columns: new[] { new GridColumn(60, SizeMode.Fixed), new GridColumn(60, SizeMode.Fixed) }
            )).ToArray();

            Assert.AreEqual(0, solution.Length);
        }

        [TestMethod]
        public void AssertThat_GridElement_FailsLayout_WhenFixedRowSizesExceedParentHeight()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Grid(
                rows: new[] { new GridRow(60, SizeMode.Fixed), new GridRow(60, SizeMode.Fixed) },
                columns: new[] { new GridColumn(10, SizeMode.Fixed) }
            )).ToArray();

            Assert.AreEqual(0, solution.Length);
        }

        [TestMethod]
        public void AssertThat_GridElement_FailsLayout_WhenFixedColSizesLessThanParentWidth()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Grid(
                rows: new[] { new GridRow(10, SizeMode.Grow) },
                columns: new[] { new GridColumn(10, SizeMode.Fixed) }
            )).ToArray();

            Assert.AreEqual(0, solution.Length);
        }

        [TestMethod]
        public void AssertThat_GridElement_FailsLayout_WhenFixedRowSizesLessThanParentHeight()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Grid(
                rows: new[] { new GridRow(10, SizeMode.Fixed) },
                columns: new[] { new GridColumn(10, SizeMode.Grow) }
            )).ToArray();

            Assert.AreEqual(0, solution.Length);
        }

        [TestMethod]
        public void AssertThat_GridElement_CanSerializeAndDeserialize_WithDefaults()
        {
            var element = new Grid(
                rows: new[] {new GridRow(10, SizeMode.Auto), new GridRow(10, SizeMode.Grow)},
                columns: new[] {new GridColumn(10, SizeMode.Fixed), new GridColumn(10, SizeMode.Grow)}
            );

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(element, new StringWriter(builder));

            Console.WriteLine(builder.ToString());

            var deserialized = (Grid)Yaml.Deserialize(new StringReader(builder.ToString()));

            Assert.AreEqual(element.Margin, deserialized.Margin);
            Assert.AreEqual(element.MaxHeight, deserialized.MaxHeight);
            Assert.AreEqual(element.MinHeight, deserialized.MinHeight);
            Assert.AreEqual(element.MaxWidth, deserialized.MaxWidth);
            Assert.AreEqual(element.MinWidth, deserialized.MinWidth);

            var expectedRows = element.Rows.ToArray();
            var actualRows = deserialized.Rows.ToArray();
            Assert.AreEqual(expectedRows.Length, actualRows.Length);
            for (int i = 0; i < expectedRows.Length; i++)
                Assert.AreEqual(expectedRows[i], actualRows[i]);

            var expectedColumns = element.Columns.ToArray();
            var actualColumns = deserialized.Columns.ToArray();
            Assert.AreEqual(expectedColumns.Length, actualColumns.Length);
            for (int i = 0; i < expectedColumns.Length; i++)
                Assert.AreEqual(expectedColumns[i], actualColumns[i]);
        }

        [TestMethod]
        public void AssertThat_GridElement_LaysOutFixedElements()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Grid(
                rows: new[] { new GridRow(100, SizeMode.Fixed) },
                columns: new[] { new GridColumn(50, SizeMode.Fixed), new GridColumn(50, SizeMode.Fixed) }
            ) {
                new Space("l"), new Space("r")
            }).ToArray();

            Assert.AreEqual(3, solution.Length);

            var l = solution.Single(a => a.Element.Name == "l");
            var r = solution.Single(a => a.Element.Name == "r");

            Assert.AreEqual(0, l.Left);
            Assert.AreEqual(50, l.Right);

            Assert.AreEqual(50, r.Left);
            Assert.AreEqual(100, r.Right);
        }

        [TestMethod]
        public void AssertThat_GridElement_LaysOutAutoElements()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Grid(
                rows: new[] { new GridRow(100, SizeMode.Fixed) },
                columns: new[] { new GridColumn(1, SizeMode.Auto), new GridColumn(1, SizeMode.Auto) }
            ) {
                new Space("l"), new Space("r")
            }).ToArray();

            Assert.AreEqual(3, solution.Length);

            var l = solution.Single(a => a.Element.Name == "l");
            var r = solution.Single(a => a.Element.Name == "r");

            //Check that space is equally distributed between the two auto elements

            Assert.AreEqual(0, l.Left);
            Assert.AreEqual(50, l.Right);

            Assert.AreEqual(50, r.Left);
            Assert.AreEqual(100, r.Right);
        }

        [TestMethod]
        public void AssertThat_GridElement_LaysOutGrowElements()
        {
            var solution = Solver.Solve(0, 100, 100, 0, new Grid(
                rows: new[] { new GridRow(100, SizeMode.Fixed) },
                columns: new[] { new GridColumn(3, SizeMode.Grow), new GridColumn(1, SizeMode.Grow) }
            ) {
                new Space("l"), new Space("r")
            }).ToArray();

            Assert.AreEqual(3, solution.Length);

            var l = solution.Single(a => a.Element.Name == "l");
            var r = solution.Single(a => a.Element.Name == "r");

            //Check that space is distributed between the two auto elements (according to ratio of initial sizes)
            Assert.AreEqual(0, l.Left);
            Assert.AreEqual(75, l.Right);

            Assert.AreEqual(75, r.Left);
            Assert.AreEqual(100, r.Right);
        }
    }
}
