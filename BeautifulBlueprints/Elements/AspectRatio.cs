using System;
using BeautifulBlueprints.Layout;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class AspectRatio
        : BaseElement
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
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float maxHeight = DEFAULT_MAX_HEIGHT,
            Margin margin = null,
            float ratio = DEFAULT_RATIO,
            HorizontalAlignment horizontalAlignment = DEFAULT_HORIZONTAL_ALIGNMENT,
            VerticalAlignment verticalAlignment = DEFAULT_VERTICAL_ALIGNMENT
        )
            : base(name, minWidth, maxWidth, minHeight, maxHeight, margin)
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
                RecalculateHeight(width, maxHeight, ref height);
                RecalculateWidth(height, maxWidth, ref width);
            }

            var floated = Float.FloatElement(this, HorizontalAlignment, VerticalAlignment, width, height, left, right, top, bottom);
            yield return floated;

            foreach (var child in Children)
                foreach (var solution in child.Solve(floated.Left, floated.Right, floated.Top, floated.Bottom))
                    yield return solution;
        }

        protected bool RecalculateHeight(float width, float maxHeight, ref float height)
        {
            var h = width / Ratio;

            if (h < MinHeight)
                throw new LayoutFailureException(string.Format("height is < MinHeight for element {0}({1})", GetType().Name, Name));

            if (h > maxHeight)
                h = maxHeight;

            if (Math.Abs(h - height) < float.Epsilon)
                return false;

            height = h;
            return true;
        }

        protected bool RecalculateWidth(float height, float maxWidth, ref float width)
        {
            var w = height * Ratio;

            if (w < MinWidth)
                throw new LayoutFailureException(string.Format("width is < MinWidth for element {0}({1})", GetType().Name, Name));

            if (w > maxWidth)
                w = maxWidth;

            if (Math.Abs(w - width) < float.Epsilon)
                return false;

            width = w;
            return true;
        }

        internal override BaseElementContainer Contain()
        {
            return new AspectRatioContainer(this);
        }
    }

    internal class AspectRatioContainer
        : BaseElement.BaseElementContainer
    {
        public float Ratio { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

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
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                maxHeight: MaxHeight,
                margin: (Margin ?? new MarginContainer()).Unwrap(),
                ratio: Ratio,
                horizontalAlignment: HorizontalAlignment,
                verticalAlignment: VerticalAlignment
            );

            foreach (var child in Children)
                s.Add(child.Unwrap());

            return s;
        }
    }
}
