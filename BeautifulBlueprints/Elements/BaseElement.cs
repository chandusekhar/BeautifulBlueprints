using BeautifulBlueprints.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BeautifulBlueprints.Elements
{
    /// <summary>
    /// Abstract base class for all element types
    /// </summary>
    public abstract class BaseElement
    {
        internal const float DEFAULT_MIN_WIDTH = 0;
        internal const float DEFAULT_MAX_WIDTH = 10000;
        internal const float DEFAULT_MIN_HEIGHT = 0;
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

        private readonly float? _preferredWidth;
        /// <summary>
        /// Once all other constraints are satisfied, the width this element should be closest to
        /// </summary>
        public virtual float? PreferredWidth
        {
            get
            {
                return _preferredWidth;
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

        private readonly float? _preferredHeight;
        /// <summary>
        /// Once all other constraints are satisfied, the height this element should be closest to
        /// </summary>
        public virtual float? PreferredHeight
        {
            get
            {
                return _preferredHeight;
            }
        }

        protected BaseElement(string name = null,
            float minWidth = DEFAULT_MIN_WIDTH,
            float? preferredWidth = null,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float? preferredHeight = null,
            float maxHeight = DEFAULT_MAX_HEIGHT)
        {
            _name = name ?? Guid.NewGuid().ToString();

            _minWidth = minWidth;
            _preferredWidth = preferredWidth;
            _maxWidth = maxWidth;

            _minHeight = minHeight;
            _preferredHeight = preferredHeight;
            _maxHeight = maxHeight;
        }

        internal abstract IEnumerable<Solver.Solution> Solve(float left, float right, float top, float bottom);

        internal virtual void Prepare()
        {
        }

        protected internal Solver.Solution FillSpace(float left, float right, float top, float bottom, bool checkMinWidth = true, bool checkMaxWidth = true, bool checkMinHeight = true, bool checkMaxHeight = true)
        {
            var width = (right - left);
            var height = (top - bottom);

            if (checkMinWidth && width < MinWidth)
                throw new LayoutFailureException("available width is < MinWidth", this);
            if (checkMaxWidth && width > MaxWidth)
                throw new LayoutFailureException("available width is > MaxWidth", this);

            if (checkMinHeight && height < MinHeight)
                throw new LayoutFailureException("available height is < MinHeight", this);
            if (checkMaxHeight && height > MaxHeight)
                throw new LayoutFailureException("available height is > MaxHeight", this);

            return new Solver.Solution(this, left, right, top, bottom);
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

            protected BaseElementContainer()
            {
                MinWidth = DEFAULT_MIN_WIDTH;
                MaxWidth = DEFAULT_MAX_WIDTH;

                MinHeight = DEFAULT_MIN_HEIGHT;
                MaxHeight = DEFAULT_MAX_HEIGHT;
            }

            protected BaseElementContainer(BaseElement element)
            {
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
