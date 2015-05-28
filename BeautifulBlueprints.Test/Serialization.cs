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
!Fallback
Margin: {}
Children:
  - !Float
    Children:
      - !Space
        Name: 87e92c86-864f-460b-a651-aebfee9d7a0e
    HorizontalAlignment: Uniform
    Margin: {}
    Name: eefa8f75-d082-4955-b8af-88b4b8b6c6b2
  - !Space
    Children: []
    Margin:
      Bottom: 10.0
    Name: 319ed878-b52f-4181-b0db-be379af9e1b5
"));

            Assert.AreEqual(2, el.Children.Count());
            Assert.AreEqual(10, el.Children.Skip(1).First().Margin.Bottom);
        }

        [TestMethod]
        public void SerializeElement()
        {
            var el = new Fallback() {
                new Float(horizontalAlignment: HorizontalAlignment.Uniform) {
                    new Space()
                },
                new Space(margin: new Margin(bottom: 10))
            };

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(el, new StringWriter(builder));

            Console.WriteLine(builder.ToString());
        }
    }
}
