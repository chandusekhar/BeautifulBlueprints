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
Children:
  - !Repeat
    Children:
      - !Space {}
  - !Space {}
"));

            Assert.AreEqual(2, ((BaseContainerElement)el).Children.Count());
        }

        [TestMethod]
        public void SerializeElement()
        {
            var el = new Fallback() {
                new Repeat() {
                    new Space()
                },
                new Space()
            };

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(el, new StringWriter(builder));

            Console.WriteLine(builder.ToString());
        }
    }
}
