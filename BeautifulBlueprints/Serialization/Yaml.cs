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
                    _serializer = CreateSerializer();
                return _serializer;
            }
        }

        [ThreadStatic]
        private static Serializer _deserializer;
        private static Serializer Deserializer
        {
            get
            {
                if (_deserializer == null)
                    _deserializer = CreateDeserializer();
                return _deserializer;
            }
        }

        private static Serializer CreateSerializer()
        {
            var serializer = new Serializer(new SerializerSettings
            {
                EmitTags = true,
            });

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(a => typeof(BaseElement).IsAssignableFrom(a) && !a.IsAbstract))
                serializer.Settings.RegisterTagMapping(type.Name, type);

            return serializer;
        }

        private static Serializer CreateDeserializer()
        {
            var serializer = new Serializer(new SerializerSettings
            {
            });

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(a => typeof(BaseElementContainer).IsAssignableFrom(a) && !a.IsAbstract))
                serializer.Settings.RegisterTagMapping(type.Name.Replace("Container", ""), type);

            return serializer;
        }

        public static void Serialize(BaseElement root, TextWriter writer)
        {
            Serializer.Serialize(writer, root);
        }

        public static BaseElement Deserialize(TextReader reader)
        {
            return Deserializer.Deserialize<BaseElementContainer>(reader).Unwrap();
        }
    }
}
