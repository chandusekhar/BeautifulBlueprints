using BeautifulBlueprints.Elements;
using System;

namespace BeautifulBlueprints.Layout
{
    public static class LayoutHelpers
    {
        public static Solver.Solution FloatElement(BaseElement el, HorizontalAlignment hAlign, VerticalAlignment vAlign, float width, float height, float left, float right, float top, float bottom)
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
    }
}
