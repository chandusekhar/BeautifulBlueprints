using System;
using System.Linq;
using System.Reflection;
using BeautifulBlueprints.Elements;
using SharpYaml.Serialization;
using System.IO;

namespace BeautifulBlueprints.Serialization
{
    public static class Yaml
    {
        [ThreadStatic] private static Serializer _serializer;
        private static Serializer Serializer
        {
            get
            {
                if (_serializer == null)
                    _serializer = Create();
                return _serializer;
            }
        }

        private static Serializer Create()
        {
            var serializer = new Serializer(new SerializerSettings
            {
                EmitTags = true,
                EmitShortTypeName = true
            });

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(a => typeof(BaseElement).IsAssignableFrom(a) && !a.IsAbstract))
                serializer.Settings.RegisterTagMapping(type.Name, type);

            return serializer;
        }

        public static void Serialize(BaseElement root, TextWriter writer)
        {
            Serializer.Serialize(writer, root);
        }

        public static BaseElement Deserialize(TextReader reader)
        {
            return Serializer.Deserialize<BaseElement>(reader);
        }
    }
}
