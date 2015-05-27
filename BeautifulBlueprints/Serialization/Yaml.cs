using BeautifulBlueprints.Elements;
using SharpYaml.Serialization;
using System;
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

        private static void RegisterTags(Serializer serializer)
        {
            serializer.Settings.RegisterTagMapping("AspectRatio", typeof(AspectRatioContainer));
            serializer.Settings.RegisterTagMapping("Fallback", typeof(FallbackContainer));
            serializer.Settings.RegisterTagMapping("Float", typeof(FloatContainer));
            //serializer.Settings.RegisterTagMapping("Flow", typeof(FlowContainer));

            serializer.Settings.RegisterTagMapping("Grid", typeof(GridContainer));
            serializer.Settings.RegisterTagMapping("Row", typeof(GridRowContainer));
            serializer.Settings.RegisterTagMapping("Column", typeof(GridColumnContainer));

            serializer.Settings.RegisterTagMapping("Space", typeof(SpaceContainer));
            serializer.Settings.RegisterTagMapping("Stack", typeof(StackContainer));

            serializer.Settings.RegisterTagMapping("Margin", typeof(MarginContainer));
        }

        private static Serializer CreateSerializer()
        {
            var serializer = new Serializer(new SerializerSettings
            {
                EmitTags = true,
            });

            RegisterTags(serializer);

            return serializer;
        }

        private static Serializer CreateDeserializer()
        {
            var serializer = new Serializer(new SerializerSettings());

            RegisterTags(serializer);

            return serializer;
        }

        public static void Serialize(BaseElement root, TextWriter writer)
        {
            Serializer.Serialize(writer, root.Contain());
        }

        public static BaseElement Deserialize(TextReader reader)
        {
            return Deserializer.Deserialize<BaseElement.BaseElementContainer>(reader).Unwrap();
        }
    }
}
