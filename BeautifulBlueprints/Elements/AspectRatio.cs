using System;
using System.ComponentModel;
using BeautifulBlueprints.Layout;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class AspectRatio
        : BaseContainerElement
    {
        internal const HorizontalAlignment DEFAULT_HORIZONTAL_ALIGNMENT = HorizontalAlignment.Center;
        internal const VerticalAlignment DEFAULT_VERTICAL_ALIGNMENT = VerticalAlignment.Center;
        internal const float DEFAULT_RATIO = 1;

        private readonly float _ratio;
        public float Ratio
        {
            get
            {
                return _ratio;
            }
        }

        private readonly HorizontalAlignment _horizontalAlignment;
        public HorizontalAlignment HorizontalAlignment { get { return _horizontalAlignment; } }

        private readonly VerticalAlignment _verticalAlignment;
        public VerticalAlignment VerticalAlignment { get { return _verticalAlignment; } }

        public AspectRatio(
            string name = null,
            float minWidth = DEFAULT_MIN_WIDTH,
            float preferredWidth = DEFAULT_PREFERRED_WIDTH,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float preferredHeight = DEFAULT_PREFERRED_HEIGHT,
            float maxHeight = DEFAULT_MAX_HEIGHT,
            Margin margin = null,
            float ratio = DEFAULT_RATIO,
            HorizontalAlignment horizontalAlignment = DEFAULT_HORIZONTAL_ALIGNMENT,
            VerticalAlignment verticalAlignment = DEFAULT_VERTICAL_ALIGNMENT
        )
            : base(name, minWidth, preferredWidth, maxWidth, minHeight, preferredHeight, maxHeight, margin)
        {
            _ratio = ratio;
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

        internal override IEnumerable<Solver.Solution> Solve(float left, float right, float top, float bottom)
        {
            //Fill up the available space (no care for aspect ratio)
            var self = FillSpace(left, right, top, bottom, checkMaxWidth: false, checkMaxHeight: false);

            var maxWidth = Math.Min(MaxWidth, self.Right - self.Left);
            var maxHeight = Math.Min(MaxHeight, self.Top - self.Bottom);

            //Correct the aspect ratio
            var width = maxWidth;
            var height = maxHeight;
            if (Math.Abs((width / height) - Ratio) > float.Epsilon)
            {
                bool changed;
                do
                {
                    float h;
                    float w = width;
                    bool hOk = RecalculateHeight(w, maxHeight, out h);
                    bool wOk = RecalculateWidth(h, maxWidth, out w);

// ReSharper disable CompareOfFloatsByEqualityOperator
                    changed = h != height || w != width;
// ReSharper restore CompareOfFloatsByEqualityOperator
                    if (changed)
                    {
                        width = w;
                        height = h;
                    }
                    else
                    {
                        if (!hOk || !wOk)
                            throw new LayoutFailureException(string.Format("Cannot satisfy aspect ratio constraint (best attempt {0} out of {1})", width / height, Ratio), this);
                    }

                } while (changed);
            }

            var floated = Float.FloatElement(this, HorizontalAlignment, VerticalAlignment, width, height, left, right, top, bottom);
            yield return floated;

            foreach (var child in Children)
                foreach (var solution in child.Solve(floated.Left, floated.Right, floated.Top, floated.Bottom))
                    yield return solution;
        }

        protected bool RecalculateHeight(float width, float maxHeight, out float height)
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

        protected bool RecalculateWidth(float height, float maxWidth, out float width)
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
        [DefaultValue(AspectRatio.DEFAULT_RATIO)]
        public float Ratio { get; set; }

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
            HorizontalAlignment = aspect.HorizontalAlignment;
            VerticalAlignment = aspect.VerticalAlignment;
        }

        public override BaseElement Unwrap()
        {
            var s = new AspectRatio(name: Name,
                minWidth: MinWidth,
                preferredWidth: PreferredWidth ?? BaseElement.DEFAULT_PREFERRED_WIDTH,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight ?? BaseElement.DEFAULT_PREFERRED_HEIGHT,
                maxHeight: MaxHeight,
                margin: (Margin ?? new MarginContainer()).Unwrap(),
                ratio: Ratio,
                horizontalAlignment: HorizontalAlignment,
                verticalAlignment: VerticalAlignment
            );

            UnwrapChildren(s);

            return s;
        }
    }
}
