using System.Linq;
using System.Runtime.Remoting;
using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeautifulBlueprints.Test.Elements
{
    [TestClass]
    public class SubsectionTest
    {
        [TestMethod]
        public void AssertThat_SubsectionElement_CanSerializeAndDeserialize_WithDefaults()
        {
            var element = new Subsection(new Dictionary<string, string> {
                { "a", "b" }
            });

            StringBuilder builder = new StringBuilder();
            Yaml.Serialize(element, new StringWriter(builder));

            Console.WriteLine(builder.ToString());

            var deserialized = (Fallback)Yaml.Deserialize(new StringReader(builder.ToString()));

            Assert.AreEqual(element.MaxHeight, deserialized.MaxHeight);
            Assert.AreEqual(element.MinHeight, deserialized.MinHeight);
            Assert.AreEqual(element.MaxWidth, deserialized.MaxWidth);
            Assert.AreEqual(element.MinWidth, deserialized.MinWidth);
        }

        [TestMethod]
        public void AssertThat_SubsectionElement_CanDeserialize()
        {
            var yaml = @"
!Subsection
Parameters:
    a: b
    b: c
";
            var deserialized = (Subsection)Yaml.Deserialize(new StringReader(yaml));

            Assert.IsTrue(deserialized.SearchParameters.Single(a => a.Key == "a").Value == "b");
            Assert.IsTrue(deserialized.SearchParameters.Single(a => a.Key == "b").Value == "c");
        }
    }
}
