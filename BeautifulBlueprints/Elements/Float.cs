using System.ComponentModel;
using BeautifulBlueprints.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeautifulBlueprints.Elements
{
    public class Float
        : BaseContainerElement
    {
        internal const HorizontalAlignment DEFAULT_HORIZONTAL_ALIGNMENT = HorizontalAlignment.Center;
        internal const VerticalAlignment DEFAULT_VERTICAL_ALIGNMENT = VerticalAlignment.Center;

        private readonly HorizontalAlignment _horizontalAlignment;
        public HorizontalAlignment HorizontalAlignment { get { return _horizontalAlignment; } }

        private readonly VerticalAlignment _verticalAlignment;
        public VerticalAlignment VerticalAlignment { get { return _verticalAlignment; } }

        public override float MinWidth
        {
            get
            {
                var c = Children.SingleOrDefault();
                if (c == null)
                    return base.MinWidth;
                return Math.Max(c.MinWidth, base.MinWidth);
            }
        }

        public override float MaxWidth
        {
            get
            {
                var c = Children.SingleOrDefault();
                if (c == null)
                    return base.MaxWidth;
                return Math.Min(c.MaxWidth, base.MaxWidth);
            }
        }

        public override float MinHeight
        {
            get
            {
                var c = Children.SingleOrDefault();
                if (c == null)
                    return base.MinHeight;
                return Math.Max(c.MinHeight, base.MinHeight);
            }
        }

        public override float MaxHeight
        {
            get
            {
                var c = Children.SingleOrDefault();
                if (c == null)
                    return base.MaxHeight;
                return Math.Min(c.MaxHeight, base.MaxHeight);
            }
        }

        public Float(
            string name = null,
            float minWidth = DEFAULT_MIN_WIDTH,
            float preferredWidth = DEFAULT_PREFERRED_WIDTH,
            float maxWidth = DEFAULT_MAX_WIDTH,
            float minHeight = DEFAULT_MIN_HEIGHT,
            float preferredHeight = DEFAULT_PREFERRED_HEIGHT,
            float maxHeight = DEFAULT_MAX_HEIGHT,
            Margin margin = null,
            HorizontalAlignment horizontalAlignment = DEFAULT_HORIZONTAL_ALIGNMENT,
            VerticalAlignment verticalAlignment = DEFAULT_VERTICAL_ALIGNMENT
        )
            : base(name, minWidth, preferredWidth, maxWidth, minHeight, preferredHeight, maxHeight, margin)
        {
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
            var width = Math.Max(Math.Min(MaxWidth, right - left), MinWidth);
            var height = Math.Max(Math.Min(MaxHeight, top - bottom), MinHeight);

            var self = FloatElement(this, HorizontalAlignment, VerticalAlignment, width, height, left, right, top, bottom);
            yield return self;

            foreach (var child in Children)
                foreach (var solution in child.Solve(self.Left, self.Right, self.Top, self.Bottom))
                    yield return solution;
        }

        internal static Solver.Solution FloatElement(BaseElement el, HorizontalAlignment hAlign, VerticalAlignment vAlign, float width, float height, float left, float right, float top, float bottom)
        {
            var self = el.FillSpace(left, right, top, bottom, checkMaxWidth: false, checkMaxHeight: false);

            //Position the element correctly (Horizontal)
            var spareHSpace = (right - left) - width;
            float l, r;
            switch (hAlign)
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
            switch (vAlign)
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

            return new Solver.Solution(el, l, r, t, b);
        }

        internal override BaseElementContainer Wrap()
        {
            return new FloatContainer(this);
        }
    }

    internal class FloatContainer
        : BaseContainerElement.BaseContainerElementContainer
    {
        [DefaultValue(Float.DEFAULT_HORIZONTAL_ALIGNMENT)]
        public HorizontalAlignment HorizontalAlignment { get; set; }

        [DefaultValue(Float.DEFAULT_VERTICAL_ALIGNMENT)]
        public VerticalAlignment VerticalAlignment { get; set; }

        public FloatContainer()
        {
            HorizontalAlignment = Float.DEFAULT_HORIZONTAL_ALIGNMENT;
            VerticalAlignment = Float.DEFAULT_VERTICAL_ALIGNMENT;
        }

        public FloatContainer(Float floatEl)
            : base(floatEl)
        {
            HorizontalAlignment = floatEl.HorizontalAlignment;
            VerticalAlignment = floatEl.VerticalAlignment;
        }

        public override BaseElement Unwrap()
        {
            var s = new Float(name: Name,
                minWidth: MinWidth,
                preferredWidth: PreferredWidth ?? BaseElement.DEFAULT_PREFERRED_WIDTH,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight ?? BaseElement.DEFAULT_PREFERRED_HEIGHT,
                maxHeight: MaxHeight,
                margin: (Margin ?? new MarginContainer()).Unwrap(),
                horizontalAlignment: HorizontalAlignment,
                verticalAlignment: VerticalAlignment
            );

            UnwrapChildren(s);

            return s;
        }
    }
}
