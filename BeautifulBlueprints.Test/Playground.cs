using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace BeautifulBlueprints.Test
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public void WallWithDoorAndWindows()
        {
            var root = new Grid(
                rows: new[] { new GridRow(1, SizeMode.Grow) },
                columns: new[] { new GridColumn(1, SizeMode.Grow), new GridColumn(1, SizeMode.Auto), new GridColumn(1, SizeMode.Auto), new GridColumn(1, SizeMode.Auto), new GridColumn(1, SizeMode.Grow) }
            ) {
                new Space(), //Empty space to the left
                new AspectRatio(ratio: 1, minWidth: 1),
                new AspectRatio(ratio: 2, minWidth: 1),
                new AspectRatio(ratio: 1, minWidth: 1),
                new Space()
            };

            //StringBuilder b = new StringBuilder();
            //Yaml.Serialize(root, new StringWriter(b));

            //Console.WriteLine(b);
            //var d = Yaml.Deserialize(new StringReader(b.ToString()));

            var sol = Layout.Solver.Solve(0, 100, 10, 0, root).ToArray();
            Assert.AreEqual(6, sol.Length);

            foreach (var part in sol.Where(a => !(a.Element is Grid)))
                Console.WriteLine(part.Element.GetType().Name + " " + (part.Right - part.Left));
        }

        [TestMethod]
        public void WallWithDoorAndWindowsVert()
        {
            var root = new Grid(
                rows: new[] { new GridRow(1, SizeMode.Grow), new GridRow(1, SizeMode.Auto), new GridRow(1, SizeMode.Auto), new GridRow(1, SizeMode.Auto), new GridRow(1, SizeMode.Grow) },
                columns: new[] { new GridColumn(1, SizeMode.Grow) }
            ) {
                new Space(), //Empty space to the left
                new Space(minHeight: 1, preferredHeight: 1.3m),
                new Space(minHeight: 1, preferredHeight: 15m),
                new Space(minHeight: 1, preferredHeight: 1.3m),
                new Space()
            };

            //StringBuilder b = new StringBuilder();
            //Yaml.Serialize(root, new StringWriter(b));

            //Console.WriteLine(b);

            var sol = Layout.Solver.Solve(0, 100, 10, 0, root).ToArray();
            Assert.AreEqual(6, sol.Length);

            foreach (var part in sol.Where(a => !(a.Element is Grid)))
                Console.WriteLine(part.Element.GetType().Name + " " + (part.Top - part.Bottom));
        }

        [TestMethod]
        public void MethodName()
        {
            const string LAYOUT = @"
!Grid
Name: Root
Columns:
  - { Mode: Grow, Size: 1 }
  - { Mode: Auto, Size: 1 }
  - { Mode: Auto, Size: 1 }
  - { Mode: Grow, Size: 1 }
Rows:
  - { Mode: Grow, Size: 1 }
Children:
  - !Space
    MinWidth: 20
  - !Fallback
    Children:
    - !AspectRatio
       MinWidth: 40
       MaxWidth: 75
  - !Fallback
    Children:
     - !AspectRatio
       MinWidth: 20
       MaxWidth: 75
  - !Space
    MinWidth: 20


";

            var des = Yaml.Deserialize(new StringReader(LAYOUT));
            var solution = Layout.Solver.Solve(-155, 155, 75, -75, des).ToArray();

            Assert.AreNotEqual(0, solution.Length);

            Assert.IsTrue(solution.Any(a => a.Element is AspectRatio));

            foreach (var part in solution)
                Console.WriteLine(part.Element.GetType().Name + " " + part.Element.Name + " " + part.Left + " " + part.Right + " " + part.Top + " " + part.Bottom);
        }

        [TestMethod]
        public void MethodName2()
        {
            const string LAYOUT = @"
!Repeat
Children:
  - !AspectRatio
    MinWidth: 100
    MaxHeight: 200
    PreferredWidth: 75
    Ratio: 0.75
";

            var des = Yaml.Deserialize(new StringReader(LAYOUT));
            var solution = Layout.Solver.Solve(0, 601, 200, 0, des).ToArray();

            Assert.AreNotEqual(0, solution.Length);

            Assert.IsTrue(solution.Any(a => a.Element is AspectRatio));

            foreach (var part in solution)
                Console.WriteLine(part.Element.GetType().Name + " " + part.Element.Name + " " + part.Left + " " + part.Right + " " + part.Top + " " + part.Bottom);
        }
    }
}
