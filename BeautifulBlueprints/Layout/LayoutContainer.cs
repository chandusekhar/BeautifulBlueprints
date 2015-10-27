using System;
using System.Collections;
using BeautifulBlueprints.Elements;
using System.Collections.Generic;

namespace BeautifulBlueprints.Layout
{
    internal class LayoutContainerInternal
    {
        public IDictionary<string, string> Tags { get; set; }

        public BaseElement.BaseElementContainer Root { get; set; }

        public LayoutContainer Unwrap()
        {
            return new LayoutContainer(this);
        }
    }

    public class LayoutContainer
        : IEnumerable<KeyValuePair<string, string>>, ILayoutContainer
    {
        private readonly IDictionary<string, string> _tags; 
        public string this[string key]
        {
            get
            {
                string val;
                if (_tags.TryGetValue(key, out val))
                    return val;
                return null;
            }
        }

        private readonly BaseElement _root;
        public BaseElement Root
        {
            get
            {
                return _root;
            }
        }

        public Guid Id { get; private set; }

        public string Description { get; private set; }

        public LayoutContainer(BaseElement root, IDictionary<string, string> tags = null, Guid? id = null, string description = "")
        {
            _root = root;
            _tags = tags ?? new Dictionary<string, string>();
            Id = id ?? Guid.NewGuid();
            Description = description ?? "";
        }

        internal LayoutContainer(LayoutContainerInternal layoutContainerInternal)
        {
            _root = layoutContainerInternal.Root.Unwrap();
            _tags = layoutContainerInternal.Tags;
        }

        internal LayoutContainerInternal Wrap()
        {
            return new LayoutContainerInternal {
                Tags = _tags,
                Root = _root.Wrap()
            };
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_tags).GetEnumerator();
        }
    }
}
