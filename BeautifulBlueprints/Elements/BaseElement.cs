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
        internal const decimal DEFAULT_MIN_WIDTH = 0;
        internal const decimal DEFAULT_MAX_WIDTH = 10000;
        internal const decimal DEFAULT_MIN_HEIGHT = 0;
        internal const decimal DEFAULT_MAX_HEIGHT = 10000;

        private readonly string _name;
        /// <summary>
        /// The unique name of this element
        /// </summary>
        public string Name { get { return _name; } }

        private readonly decimal _minWidth;
        /// <summary>
        /// The minimum width this element may be shrunk to
        /// </summary>
        public virtual decimal MinWidth { get { return _minWidth; } }

        private readonly decimal _maxWidth;
        /// <summary>
        /// The maximum width this element may be stretched to
        /// </summary>
        public virtual decimal MaxWidth { get { return _maxWidth; } }

        private readonly decimal? _preferredWidth;
        /// <summary>
        /// Once all other constraints are satisfied, the width this element should be closest to
        /// </summary>
        public virtual decimal PreferredWidth
        {
            get
            {
                return _preferredWidth ?? MinWidth;
            }
        }

        private readonly decimal _minHeight;
        /// <summary>
        /// The minimum height of this element
        /// </summary>
        public virtual decimal MinHeight { get { return _minHeight; } }

        private readonly decimal _maxHeight;
        /// <summary>
        /// The maximum height this element may be stretched to
        /// </summary>
        public virtual decimal MaxHeight { get { return _maxHeight; } }

        private readonly decimal? _preferredHeight;
        /// <summary>
        /// Once all other constraints are satisfied, the height this element should be closest to
        /// </summary>
        public virtual decimal PreferredHeight
        {
            get
            {
                return _preferredHeight ?? MinHeight;
            }
        }

        protected BaseElement(string name = null,
            decimal minWidth = DEFAULT_MIN_WIDTH,
            decimal? preferredWidth = null,
            decimal maxWidth = DEFAULT_MAX_WIDTH,
            decimal minHeight = DEFAULT_MIN_HEIGHT,
            decimal? preferredHeight = null,
            decimal maxHeight = DEFAULT_MAX_HEIGHT)
        {
            _name = name ?? Guid.NewGuid().ToString();

            _minWidth = minWidth;
            _preferredWidth = preferredWidth;
            _maxWidth = maxWidth;

            _minHeight = minHeight;
            _preferredHeight = preferredHeight;
            _maxHeight = maxHeight;
        }

        internal abstract IEnumerable<Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom);

        internal virtual void Prepare(Solver.SolverOptions options)
        {
        }

        protected internal Solver.Solution FillSpace(decimal left, decimal right, decimal top, decimal bottom, bool checkMinWidth = true, bool checkMaxWidth = true, bool checkMinHeight = true, bool checkMaxHeight = true)
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

        public abstract class BaseElementContainer
        {
            public string Name { get; set; }

            //[DefaultValue(DEFAULT_MIN_WIDTH)]
            public decimal MinWidth { get; set; }

            //[DefaultValue(DEFAULT_MAX_WIDTH)]
            public decimal MaxWidth { get; set; }

            [DefaultValue(null)]
            public decimal? PreferredWidth { get; set; }

            //[DefaultValue(DEFAULT_MIN_HEIGHT)]
            public decimal MinHeight { get; set; }

            //[DefaultValue(DEFAULT_MAX_HEIGHT)]
            public decimal MaxHeight { get; set; }

            [DefaultValue(null)]
            public decimal? PreferredHeight { get; set; }

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
