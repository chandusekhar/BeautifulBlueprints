using BeautifulBlueprints.Elements;
using System;

namespace BeautifulBlueprints.Layout
{
    public static class LayoutHelpers
    {
        public static Solver.Solution LayoutElement(BaseElement el, HorizontalAlignment hAlign, VerticalAlignment vAlign, decimal width, decimal height, decimal left, decimal right, decimal top, decimal bottom)
        {
            var self = el.FillSpace(left, right, top, bottom, checkMaxWidth: false, checkMaxHeight: false);

            //Position the element correctly (Horizontal)
            var spareHSpace = (right - left) - width;
            decimal l, r;
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
            decimal t, b;
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

        public const decimal EPSILON = 0.0001m;

        public static bool IsEqualTo(this decimal a, decimal b)
        {
            return Math.Abs(a - b) < EPSILON;
        }

        public static bool IsGreaterThan(this decimal a, decimal b)
        {
            return a > b + EPSILON;
        }

        public static bool IsLessThan(this decimal a, decimal b)
        {
            return a < b - EPSILON;
        }
    }
}
