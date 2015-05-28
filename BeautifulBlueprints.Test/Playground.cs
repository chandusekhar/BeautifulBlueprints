using System;
using System.IO;
using System.Linq;
using System.Text;
using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            StringBuilder b = new StringBuilder();
            Yaml.Serialize(root, new StringWriter(b));

            Console.WriteLine(b);

            var sol = Layout.Solver.Solve(0, 100, 10, 0, root).ToArray();
        }
    }
}
