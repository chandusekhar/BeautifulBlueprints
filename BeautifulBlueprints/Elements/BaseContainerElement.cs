using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BeautifulBlueprints.Elements
{
    /// <summary>
    /// Abstract base class for all elements which contain other elements
    /// </summary>
    public abstract class BaseContainerElement
        : BaseElement, IEnumerable<BaseElement>
    {
        protected abstract int MaximumChildren { get; }

        private readonly List<BaseElement> _children = new List<BaseElement>();
        /// <summary>
        /// All child elements of this element
        /// </summary>
        public IEnumerable<BaseElement> Children { get { return _children; } }

        protected BaseContainerElement(string name = null,
            float minWidth = DEFAULT_MIN_WIDTH,
            float preferredWidth = DEFAULT_PREFERRED_WIDTH,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float preferredHeight = DEFAULT_PREFERRED_HEIGHT,
            float maxHeight = DEFAULT_MAX_HEIGHT,
            Margin margin = null
        )
            : base (name, minWidth, preferredWidth, maxWidth, minHeight, preferredHeight, maxHeight, margin)
        {
        }

        public virtual void Add(BaseElement baseElement)
        {
            if (_children.Count == MaximumChildren)
                throw new NotSupportedException(string.Format("{0}({1}) element allows a maximum of {2} children", GetType().Name, Name, MaximumChildren));
            _children.Add(baseElement);
        }

        internal override void Prepare()
        {
            base.Prepare();

            foreach (var child in Children)
                child.Prepare();
        }

        public IEnumerator<BaseElement> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_children).GetEnumerator();
        }

        internal abstract class BaseContainerElementContainer
            : BaseElementContainer
        {
            [DefaultValue(null)]
            public List<BaseElementContainer> Children { get; set; }

            protected BaseContainerElementContainer()
            {
            }

            protected BaseContainerElementContainer(BaseContainerElement element)
                : base(element)
            {
                Children = element.Children.Select(a => a.Wrap()).ToList();
                if (Children.Count == 0)
                    Children = null;
            }

            protected void UnwrapChildren(BaseContainerElement element)
            {
                if (Children != null)
                    foreach (var child in Children)
                        element.Add(child.Unwrap());
            }
        }
    }
}
