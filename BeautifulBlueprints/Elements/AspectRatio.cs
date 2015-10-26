using BeautifulBlueprints.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BeautifulBlueprints.Elements
{
    public class AspectRatio
        : BaseContainerElement
    {
        internal const HorizontalAlignment DEFAULT_HORIZONTAL_ALIGNMENT = HorizontalAlignment.Center;
        internal const VerticalAlignment DEFAULT_VERTICAL_ALIGNMENT = VerticalAlignment.Center;
        internal const decimal DEFAULT_RATIO = 1;

        private readonly decimal _ratio;
        public decimal Ratio
        {
            get
            {
                return _ratio;
            }
        }

        private readonly decimal _minRatio;
        public decimal MinRatio
        {
            get
            {
                return _minRatio;
            }
        }

        private readonly decimal _maxRatio;
        public decimal MaxRatio
        {
            get
            {
                return _maxRatio;
            }
        }

        private readonly HorizontalAlignment _horizontalAlignment;
        public HorizontalAlignment HorizontalAlignment { get { return _horizontalAlignment; } }

        private readonly VerticalAlignment _verticalAlignment;
        public VerticalAlignment VerticalAlignment { get { return _verticalAlignment; } }

        public AspectRatio(
            string name = null,
            decimal minWidth = DEFAULT_MIN_WIDTH,
            decimal? preferredWidth = null,
            decimal maxWidth = DEFAULT_MAX_WIDTH,
            decimal minHeight = DEFAULT_MIN_HEIGHT,
            decimal? preferredHeight = null,
            decimal maxHeight = DEFAULT_MAX_HEIGHT,
            decimal ratio = DEFAULT_RATIO,
            decimal? minRatio = null,
            decimal? maxRatio = null,
            HorizontalAlignment horizontalAlignment = DEFAULT_HORIZONTAL_ALIGNMENT,
            VerticalAlignment verticalAlignment = DEFAULT_VERTICAL_ALIGNMENT
        )
            : base(name,
                Math.Max(minWidth, minHeight * ratio),
                preferredWidth,
                maxWidth,
                Math.Max(minHeight, minWidth / ratio),
                preferredHeight,
                maxHeight
            )
        {
            _ratio = ratio;
            _minRatio = minRatio ?? ratio;
            _maxRatio = maxRatio ?? ratio;

            _horizontalAlignment = horizontalAlignment;
            _verticalAlignment = verticalAlignment;
        }

        protected override int MaximumChildren
        {
            get
            {
                return 1;
            }
        }

        internal override IEnumerable<Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom)
        {
            //Fill up the available space (no care for aspect ratio)
            var self = FillSpace(left, right, top, bottom, checkMaxWidth: false, checkMaxHeight: false);

            var maxWidth = Math.Min(MaxWidth, self.Right - self.Left);
            var maxHeight = Math.Min(MaxHeight, self.Top - self.Bottom);

            //Correct the aspect ratio
            var width = maxWidth;
            var height = maxHeight;
            if (!(width / height).IsEqualTo(Ratio))
            {
                bool changed;
                do
                {
                    decimal h;
                    decimal w = width;
                    bool hOk = RecalculateHeight(w, maxHeight, out h);
                    bool wOk = RecalculateWidth(h, maxWidth, out w);

// ReSharper disable CompareOfdecimalsByEqualityOperator
                    changed = h != height || w != width;
// ReSharper restore CompareOfdecimalsByEqualityOperator
                    if (changed)
                    {
                        width = w;
                        height = h;
                    }
                    else
                    {
                        //Check if we have failed to satisfy the constraint
                        if (!hOk || !wOk)
                        {
                            if (Ratio >= MinRatio && Ratio <= MaxRatio)
                                break;

                            //Nope, failed to constrain the ratio tightly enough
                            throw new LayoutFailureException(string.Format("Cannot satisfy aspect ratio constraint (best attempt {0} out of {1})", width / height, Ratio), this);
                        }
                    }

                } while (changed);
            }

            var layoutElement = LayoutHelpers.LayoutElement(this, HorizontalAlignment, VerticalAlignment, width, height, left, right, top, bottom);
            yield return layoutElement;

            foreach (var child in Children)
                foreach (var solution in child.Solve(layoutElement.Left, layoutElement.Right, layoutElement.Top, layoutElement.Bottom))
                    yield return solution;
        }

        protected bool RecalculateHeight(decimal width, decimal maxHeight, out decimal height)
        {
            //Given the width, what's the ideal height?
            var h = width / Ratio;

            //Go as far down as we can
            if (h < MinHeight)
            {
                height = MinHeight;
                return false;
            }

            //Go as far up as we can
            if (h > maxHeight)
            {
                height = maxHeight;
                return false;
            }

            //We can do that!
            height = h;
            return true;
        }

        protected bool RecalculateWidth(decimal height, decimal maxWidth, out decimal width)
        {
            //Get the height, what's the ideal width?
            var w = height * Ratio;

            //Go as far down as we can
            if (w < MinWidth)
            {
                width = MinWidth;
                return false;
            }

            //Go as far up as we can
            if (w > maxWidth)
            {
                width = maxWidth;
                return false;
            }

            //We can do that!
            width = w;
            return true;
        }

        internal override BaseElementContainer Wrap()
        {
            return new AspectRatioContainer(this);
        }
    }

    internal class AspectRatioContainer
        : BaseContainerElement.BaseContainerElementContainer
    {
        //[DefaultValue(AspectRatio.DEFAULT_RATIO)]
        public decimal Ratio { get; set; }

        [DefaultValue(null)]
        public decimal? MinRatio { get; set; }

        [DefaultValue(null)]
        public decimal? MaxRatio { get; set; }

        [DefaultValue(AspectRatio.DEFAULT_HORIZONTAL_ALIGNMENT)]
        public HorizontalAlignment HorizontalAlignment { get; set; }

        [DefaultValue(AspectRatio.DEFAULT_VERTICAL_ALIGNMENT)]
        public VerticalAlignment VerticalAlignment { get; set; }

        public AspectRatioContainer()
        {
            Ratio = AspectRatio.DEFAULT_RATIO;
            HorizontalAlignment = AspectRatio.DEFAULT_HORIZONTAL_ALIGNMENT;
            VerticalAlignment = AspectRatio.DEFAULT_VERTICAL_ALIGNMENT;
        }

        public AspectRatioContainer(AspectRatio aspect)
            : base(aspect)
        {
            Ratio = aspect.Ratio;
            MinRatio = aspect.MinRatio == Ratio ? null : (decimal?)aspect.MinRatio;
            MaxRatio = aspect.MaxRatio == Ratio ? null : (decimal?)aspect.MaxRatio;

            HorizontalAlignment = aspect.HorizontalAlignment;
            VerticalAlignment = aspect.VerticalAlignment;
        }

        public override BaseElement Unwrap()
        {
            return UnwrapChildren(new AspectRatio(name: Name,
                minWidth: MinWidth,
                preferredWidth: PreferredWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight,
                maxHeight: MaxHeight,
                ratio: Ratio,
                minRatio: MinRatio,
                maxRatio: MaxRatio,
                horizontalAlignment: HorizontalAlignment,
                verticalAlignment: VerticalAlignment
            ));
        }
    }
}
