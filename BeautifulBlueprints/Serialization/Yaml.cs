using System;
using System.Collections.Generic;
using BeautifulBlueprints.Elements;
using BeautifulBlueprints.Layout;
using SharpYaml.Serialization;
using System.IO;

namespace BeautifulBlueprints.Serialization
{
    public static class Yaml
    {
        private static void RegisterTags(Serializer serializer)
        {
            serializer.Settings.RegisterTagMapping("!Layout", typeof(LayoutContainerInternal));

            serializer.Settings.RegisterTagMapping("AspectRatio", typeof(AspectRatioContainer));
            serializer.Settings.RegisterTagMapping("Fallback", typeof(FallbackContainer));
            //serializer.Settings.RegisterTagMapping("Flow", typeof(FlowContainer));
            serializer.Settings.RegisterTagMapping("Fit", typeof(FitContainer));
            serializer.Settings.RegisterTagMapping("Repeat", typeof(RepeatContainer));
            serializer.Settings.RegisterTagMapping("Overlap", typeof(OverlapContainer));
            serializer.Settings.RegisterTagMapping("Grid", typeof(GridContainer));
            {
                serializer.Settings.RegisterTagMapping("Row", typeof(GridRowContainer));
                serializer.Settings.RegisterTagMapping("Column", typeof(GridColumnContainer));
            }
            serializer.Settings.RegisterTagMapping("Space", typeof(SpaceContainer));
            //serializer.Settings.RegisterTagMapping("Stack", typeof(StackContainer));
            serializer.Settings.RegisterTagMapping("Margin", typeof(MarginContainer));
            serializer.Settings.RegisterTagMapping("Path", typeof(PathContainer));
            serializer.Settings.RegisterTagMapping("Subsection", typeof(SubsectionContainer));
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

        public static void Serialize(Serializer serializer, LayoutContainer root, TextWriter writer)
        {
            serializer.Serialize(writer, root.Wrap());
        }

        public static void Serialize(LayoutContainer root, TextWriter writer, params KeyValuePair<string, Type>[] tagMappings)
        {
            var s = CreateSerializer();
            foreach (var mapping in tagMappings)
                s.Settings.RegisterTagMapping(mapping.Key, mapping.Value);

            Serialize(s, root, writer);
        }

        public static TRoot Deserialize<TRoot>(Serializer serializer, TextReader reader)
        {
            return serializer.Deserialize<TRoot>(reader);
        }

        public static ILayoutContainer Deserialize(TextReader reader, params KeyValuePair<string, Type>[] tagMappings)
        {
            var s = CreateSerializer();
            foreach (var mapping in tagMappings)
                s.Settings.RegisterTagMapping(mapping.Key, mapping.Value);

            return Deserialize<LayoutContainerInternal>(s, reader).Unwrap();
        }
    }
}
