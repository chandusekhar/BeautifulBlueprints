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
        public void Layout1()
        {
            const string LAYOUT = @"
!Layout
Root:
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

            var des = Yaml.Deserialize(new StringReader(LAYOUT)).Root;
            var solution = Layout.Solver.Solve(-155, 155, 75, -75, des).ToArray();

            Assert.AreNotEqual(0, solution.Length);

            Assert.IsTrue(solution.Any(a => a.Element is AspectRatio));

            foreach (var part in solution)
                Console.WriteLine(part.Element.GetType().Name + " " + part.Element.Name + " " + part.Left + " " + part.Right + " " + part.Top + " " + part.Bottom);
        }

        [TestMethod]
        public void Layout2()
        {
            const string LAYOUT = @"
!Layout
Root:
    !Fit
    Children:
      - !AspectRatio
        MinWidth: 100
        MaxHeight: 200
        PreferredWidth: 75
        Ratio: 0.75
";

            var des = Yaml.Deserialize(new StringReader(LAYOUT)).Root;
            var solution = Layout.Solver.Solve(0, 601, 200, 0, des).ToArray();

            Assert.AreNotEqual(0, solution.Length);

            Assert.IsTrue(solution.Any(a => a.Element is AspectRatio));

            foreach (var part in solution)
                Console.WriteLine(part.Element.GetType().Name + " " + part.Element.Name + " " + part.Left + " " + part.Right + " " + part.Top + " " + part.Bottom);
        }

        [TestMethod]
        public void Layout3()
        {
            const string LAYOUT = @"
!Layout
Root:
    !AspectRatio
    Name: Window
    Ratio: 1.2
    MinRatio: 1
    MaxRatio: 1.5
    MinWidth: 40
    MaxWidth: 200
    MinHeight: 40
    MaxHeight: 200
    Children:
      - !Grid
        Name: grid
        Rows:
          - { Mode: Grow, Size: 1 }
          - { Mode: Grow, Size: 1 }
        Columns:
          - { Mode: Grow, Size: 1 }
          - { Mode: Grow, Size: 1 }
        Children:
          - !Margin
            Name: margin
            Left: { Min: 2 }
            Right: { Min: 2 }
            Top: { Min: 2 }
            Bottom: { Min: 2 }
            Children:
            - !Path
              Name: path
              Path: M-1,-1 v2 h2 v-2 z
              StartDepth: 0
              Thickness: 1
          - !Path
            Name: path
            Path: M-1,-1 v2 h2 v-2 z
            StartDepth: 0
            Thickness: 1
          - !Path
            Name: path
            Path: M-1,-1 v2 h2 v-2 z
            StartDepth: 0
            Thickness: 1
          - !Path
            Name: path
            Path: M-1,-1 v2 h2 v-2 z
            StartDepth: 0
            Thickness: 1
";

            var des = Yaml.Deserialize(new StringReader(LAYOUT)).Root;
            var solution = Layout.Solver.Solve(0, 601, 200, 0, des).ToArray();

            Assert.AreEqual(7, solution.Length);
        }

        [TestMethod]
        public void Layout4()
        {
            const string LAYOUT = @"
!Layout
Root:
    !AspectRatio
    Name: Window
    Ratio: 1.2
    MinRatio: 1
    MaxRatio: 1.5
    MinWidth: 40
    MaxWidth: 200
    MinHeight: 40
    MaxHeight: 200
    Children:
      - !Fit
        Orientation: Vertical
        MinimizeRepeats: true
        Children:
          - !Fit
            Orientation: Horizontal
            MinimizeRepeats: true
            MinWidth: 1
            MinHeight: 1
            Children:
              - !Margin
                MinHeight: 1
                MinWidth: 1
                Left: { Min: 2 }
                Right: { Min: 2 }
                Top: { Min: 2 }
                Bottom: { Min: 2 }
                Children:
                  - !Path
                    Path: M-1,-1 v2 h2 v-2 z
                    StartDepth: 0
                    Thickness: 1
";

            var des = Yaml.Deserialize(new StringReader(LAYOUT)).Root;
            var solution = Layout.Solver.Solve(0, 601, 200, 0, des).ToArray();

            Assert.AreEqual(5, solution.Length);
        }

        [TestMethod]
        public void Layout5()
        {
            const string LAYOUT = @"
!Layout
Tags:
  Type: Hollow
  Style: None
  Id: 6b6d766c-6e99-488e-bed0-8eb244c0cea4
Root:
  !Fallback
  Children:
    - !Margin
      Left: { Min: 20, Max: 40 }
      Right: { Min: 20, Max: 40 }
      Top: { Min: 20, Max: 40 }
      Bottom: { Min: 20, Max: 40 }
      Children:
        - !Path
          Path: M-1,-1 v2 h2 v-2 z
          StartDepth: -1
          Thickness: 3
          Brush: Wood
";

            var des = Yaml.Deserialize(new StringReader(LAYOUT)).Root;
            var solution = Layout.Solver.Solve(0, 40, 100, 0, des).ToArray();

            Assert.AreEqual(3, solution.Length);
        }
    }
}
