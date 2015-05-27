using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using BeautifulBlueprints.Layout;

namespace BeautifulBlueprints.Elements
{
    public abstract class BaseElement
        : IEnumerable<BaseElement>
    {
        [DefaultValue(0)]
        public float MinWidth { get; set; }

        [DefaultValue(float.PositiveInfinity)]
        public float MaxWidth { get; set; }

        private float? _preferredWidth;

        [DefaultValue(float.PositiveInfinity)]
        public float PreferredWidth
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

        [DefaultValue(0)]
        public float MinHeight { get; set; }

        [DefaultValue(float.PositiveInfinity)]
        public float MaxHeight { get; set; }

        private float? _preferredHeight;

        [DefaultValue(float.PositiveInfinity)]
        public float PreferredHeight
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

        public Margin Margin { get; set; }

        public List<BaseElement> Children { get; set; }

        protected BaseElement(float minWidth = 0, float maxWidth = float.PositiveInfinity, float minHeight = 0, float maxHeight = float.PositiveInfinity, Margin? margin = null)
        {
            Children = new List<BaseElement>();

            MinWidth = minWidth;
            MaxWidth = maxWidth;

            MinHeight = minHeight;
            MaxHeight = maxHeight;

            Margin = margin ?? new Margin();
        }

        protected abstract bool AllowChildren { get; }

        public void Add(BaseElement baseElement)
        {
            if (!AllowChildren)
                throw new NotSupportedException(string.Format("{0} elements do not allow children", GetType().Name));
            Children.Add(baseElement);
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
    }
}
