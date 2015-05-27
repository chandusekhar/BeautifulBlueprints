using System;
using BeautifulBlueprints.Layout;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class AspectRatio
        : BaseElement
    {
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
            float minWidth = 0,
            float maxWidth = float.PositiveInfinity,
            float minHeight = 0,
            float maxHeight = float.PositiveInfinity,
            Margin margin = null,
            float ratio = 1,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment verticalAlignment = VerticalAlignment.Center
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

            var maxWidth = Math.Min(MaxWidth, right - left);
            var maxHeight = Math.Min(MaxHeight, top - bottom);

            //Correct the aspect ratio
            var width = maxWidth;
            var height = maxHeight;
            if (Math.Abs((width / height) - Ratio) > float.Epsilon)
            {
                RecalculateHeight(width, maxHeight, ref height);
                RecalculateWidth(height, maxWidth, ref width);
            }

            //Position the element correctly (Horizontal)
            var spareHSpace = (right - left) - width;
            float l, r;
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    l = self.Left;
                    r = l + width;
                    break;
                case HorizontalAlignment.Right:
                    r = self.Right;
                    l = r - width;
                    break;
                case HorizontalAlignment.Center:
                    l = self.Left + spareHSpace / 2;
                    r = l + width;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Position the element correctly (Vertical)
            var spareVSpace = (top - bottom) - height;
            float t, b;
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    t = self.Top;
                    b = t - height;
                    break;
                case VerticalAlignment.Bottom:
                    b = self.Bottom;
                    t = b + height;
                    break;
                case VerticalAlignment.Center:
                    b = self.Bottom + spareVSpace / 2;
                    t = b + height;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return new Solver.Solution(this, l, r, t, b);
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
    }

    internal class AspectRatioContainer
        : BaseElementContainer
    {
        public float Ratio { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

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

            //todo: correctly set defaults

            foreach (var child in Children)
                s.Add(child.Unwrap());

            return s;
        }
    }
}
