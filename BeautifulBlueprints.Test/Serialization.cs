using System.Linq;
using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace BeautifulBlueprints.Test
{
    [TestClass]
    public class Serialization
    {
        [TestMethod]
        public void DeserializeElement()
        {
            var el = Yaml.Deserialize(new StringReader(@"
!Stack
Children:
  - !Stack
    Children: []
    HorizontalAlignment: Left
    InlineSpacing: Uniform
    VerticalAlignment: Top
    MinWidth: 10
  - !Space
    Children: []
    Margin:
      Bottom: 10.0
HorizontalAlignment: Left
InlineSpacing: Maximize
VerticalAlignment: Top
"));

            Assert.AreEqual(2, el.Children.Count());
            Assert.AreEqual(10, el.Children.First().MinWidth);
            Assert.AreEqual(Spacing.Uniform, el.Children.OfType<Stack>().Single().InlineSpacing);
        }

        [TestMethod]
        public void SerializeElement()
        {
            var el = new Stack(inlineSpacing: Spacing.Maximize) {
                new Stack(inlineSpacing: Spacing.Minimize) {
                    new Flow()
                },
                new Space(margin: new Margin(bottom: 10))
            };

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(el, new StringWriter(builder));

            Console.WriteLine(builder.ToString());
        }
    }
}
