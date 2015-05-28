using System.Linq;
using BeautifulBlueprints.Layout;
using SharpYaml.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace BeautifulBlueprints.Elements
{
    /// <summary>
    /// Abstract base class for all element types
    /// </summary>
    public abstract class BaseElement
        : IEnumerable<BaseElement>
    {
        internal const float DEFAULT_MIN_WIDTH = 0;
        internal const float DEFAULT_PREFERRED_WIDTH = float.NaN;
        internal const float DEFAULT_MAX_WIDTH = 10000;
        internal const float DEFAULT_MIN_HEIGHT = 0;
        internal const float DEFAULT_PREFERRED_HEIGHT = float.NaN;
        internal const float DEFAULT_MAX_HEIGHT = 10000;

        private readonly string _name;
        /// <summary>
        /// The unique name of this element
        /// </summary>
        public string Name { get { return _name; } }

        private readonly float _minWidth;
        /// <summary>
        /// The minimum width this element may be shrunk to
        /// </summary>
        public virtual float MinWidth { get { return _minWidth; } }

        private readonly float _maxWidth;
        /// <summary>
        /// The maximum width this element may be stretched to
        /// </summary>
        public virtual float MaxWidth { get { return _maxWidth; } }

        private float? _preferredWidth;
        /// <summary>
        /// Once all other constraints are satisfied, the width this element should be closest to
        /// </summary>
        public virtual float PreferredWidth
        {
            get
            {
                if (_preferredWidth.HasValue)
                    return _preferredWidth.Value;

                return MinWidth * 0.5f + MaxWidth * 0.5f;
            }
            set
            {
                if (float.IsNaN(value))
                    _preferredWidth = null;
                else
                    _preferredWidth = value;
            }
        }

        private readonly float _minHeight;
        /// <summary>
        /// The minimum height of this element
        /// </summary>
        public virtual float MinHeight { get { return _minHeight; } }

        private readonly float _maxHeight;
        /// <summary>
        /// The maximum height this element may be stretched to
        /// </summary>
        public virtual float MaxHeight { get { return _maxHeight; } }

        private float? _preferredHeight;
        /// <summary>
        /// Once all other constraints are satisfied, the height this element should be closest to
        /// </summary>
        public virtual float PreferredHeight
        {
            get
            {
                if (_preferredHeight.HasValue)
                    return _preferredHeight.Value;

                return MinHeight * 0.5f + MaxHeight * 0.5f;
            }
            set
            {
                if (float.IsNaN(value))
                    _preferredHeight = null;
                else
                    _preferredHeight = value;
            }
        }

        private readonly Margin _margin;
        /// <summary>
        /// The empty space which is always included around this element
        /// </summary>
        public Margin Margin { get { return _margin; } }

        private readonly List<BaseElement> _children = new List<BaseElement>();
        /// <summary>
        /// All child elements of this element
        /// </summary>
        public IEnumerable<BaseElement> Children { get { return _children; } }
        
        protected BaseElement(string name = null,
            float minWidth = DEFAULT_MIN_WIDTH,
            float preferredWidth = DEFAULT_PREFERRED_WIDTH,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float preferredHeight = DEFAULT_PREFERRED_HEIGHT,
            float maxHeight = DEFAULT_MAX_HEIGHT,
            Margin margin = null)
        {
            _name = name ?? Guid.NewGuid().ToString();

            _minWidth = minWidth;
            _preferredWidth = float.IsNaN(preferredWidth) ? (float?)null : preferredWidth;
            _maxWidth = maxWidth;

            _minHeight = minHeight;
            _preferredHeight = float.IsNaN(preferredHeight) ? (float?)null : preferredHeight;
            _maxHeight = maxHeight;

            _margin = margin ?? new Margin();
        }

        [YamlIgnore]
        protected abstract int MaximumChildren { get; }

        public virtual void Add(BaseElement baseElement)
        {
            if (_children.Count == MaximumChildren)
                throw new NotSupportedException(string.Format("{0}({1}) element allows a maximum of {2} children", GetType().Name, Name, MaximumChildren));
            _children.Add(baseElement);
        }

        public IEnumerator<BaseElement> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal abstract IEnumerable<Solver.Solution> Solve(float left, float right, float top, float bottom);

        internal virtual void Prepare()
        {
            foreach (var child in Children)
                child.Prepare();
        }

        protected internal Solver.Solution FillSpace(float left, float right, float top, float bottom, bool checkMinWidth = true, bool checkMaxWidth = true, bool checkMinHeight = true, bool checkMaxHeight = true)
        {
            var width = (right - left) - (Margin.Left + Margin.Right);
            var height = (top - bottom) - (Margin.Top + Margin.Bottom);

            if (checkMinWidth && width < MinWidth)
                throw new LayoutFailureException("available width is < MinWidth", this);
            if (checkMaxWidth && width > MaxWidth)
                throw new LayoutFailureException("available width is > MaxWidth", this);

            if (checkMinHeight && height < MinHeight)
                throw new LayoutFailureException("available height is < MinHeight", this);
            if (checkMaxHeight && height > MaxHeight)
                throw new LayoutFailureException("available height is > MaxHeight", this);

            return new Solver.Solution(this, left + Margin.Left, right - Margin.Right, top - Margin.Top, bottom + Margin.Bottom);
        }

        internal abstract BaseElementContainer Wrap();

        internal abstract class BaseElementContainer
        {
            public string Name { get; set; }

            [DefaultValue(DEFAULT_MIN_WIDTH)]
            public float MinWidth { get; set; }

            [DefaultValue(DEFAULT_MAX_WIDTH)]
            public float MaxWidth { get; set; }

            [DefaultValue(null)]
            public float? PreferredWidth { get; set; }

            [DefaultValue(DEFAULT_MIN_HEIGHT)]
            public float MinHeight { get; set; }

            [DefaultValue(DEFAULT_MAX_HEIGHT)]
            public float MaxHeight { get; set; }

            [DefaultValue(null)]
            public float? PreferredHeight { get; set; }

            [DefaultValue(null)]
            public MarginContainer Margin { get; set; }

            [DefaultValue(null)]
            public List<BaseElementContainer> Children { get; set; }

            public BaseElementContainer()
            {
                MinWidth = DEFAULT_MIN_WIDTH;
                MaxWidth = DEFAULT_MAX_WIDTH;

                MinHeight = DEFAULT_MIN_HEIGHT;
                MaxHeight = DEFAULT_MAX_HEIGHT;
            }

            protected BaseElementContainer(BaseElement element)
            {
                Children = element.Children.Select(a => a.Wrap()).ToList();
                if (Children.Count == 0)
                    Children = null;

                Margin = element.Margin.Wrap();
                MaxHeight = element.MaxHeight;
                MaxWidth = element.MaxWidth;
                MinHeight = element.MinHeight;
                MinWidth = element.MinWidth;
                Name = element.Name;
                PreferredHeight = element._preferredHeight;
                PreferredWidth = element._preferredWidth;
            }

            public abstract BaseElement Unwrap();
        }
    }
}
