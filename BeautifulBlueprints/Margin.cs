
using System.ComponentModel;

namespace BeautifulBlueprints
{
    public class Margin
    {
        [DefaultValue(0)]
        public float Left { get; private set; }

        [DefaultValue(0)]
        public float Right { get; private set; }

        [DefaultValue(0)]
        public float Top { get; private set; }

        [DefaultValue(0)]
        public float Bottom { get; private set; }

        public Margin(float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
        }
    }

    internal class MarginContainer
    {
        [DefaultValue(0)]
        public float Left { get; set; }

        [DefaultValue(0)]
        public float Right { get; set; }

        [DefaultValue(0)]
        public float Top { get; set; }

        [DefaultValue(0)]
        public float Bottom { get; set; }

        public Margin Unwrap()
        {
            return new Margin(left: Left, right: Right, top: Top, bottom: Bottom);
        }
    }
}
