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

        public int ChildCount
        {
            get
            {
                return _children.Count;
            }
        }

        protected BaseContainerElement(string name = null,
            decimal minWidth = DEFAULT_MIN_WIDTH,
            decimal? preferredWidth = null,
            decimal maxWidth = DEFAULT_MAX_WIDTH,
            decimal minHeight = DEFAULT_MIN_HEIGHT,
            decimal? preferredHeight = null,
            decimal maxHeight = DEFAULT_MAX_HEIGHT
        )
            : base (name, minWidth, preferredWidth, maxWidth, minHeight, preferredHeight, maxHeight)
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

            protected T UnwrapChildren<T>(T element) where T : BaseContainerElement
            {
                if (Children != null)
                    foreach (var child in Children)
                        element.Add(child.Unwrap());

                return element;
            }
        }
    }
}
