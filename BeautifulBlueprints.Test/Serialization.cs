using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpYaml.Serialization;
using System;
using System.IO;
using System.Linq;
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
            var el = new Fallback {
                new Repeat {
                    new Space()
                },
                new Space()
            };

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(el, new StringWriter(builder));

            Console.WriteLine(builder.ToString());
        }

        [TestMethod]
        public void AreDecimalsBroken_YesTheyAre()
        {
            var serializer = new Serializer(new SerializerSettings
            {
                EmitTags = true,
            });

            var builder = new StringBuilder();
            serializer.Serialize(new StringWriter(builder), new { Value = 1.023m });
        }
    }
}
