
using System;
using System.ComponentModel;

namespace BeautifulBlueprints
{
    public class Margin
        : IEquatable<Margin>
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

        public bool Equals(Margin other)
        {
            return Math.Abs(other.Left - Left) < float.Epsilon
                && Math.Abs(other.Right - Right) < float.Epsilon
                && Math.Abs(other.Top - Top) < float.Epsilon
                && Math.Abs(other.Bottom - Bottom) < float.Epsilon;
        }

        public override bool Equals(object obj)
        {
            var a = obj as Margin;
            if (a != null)
                return Equals(a);

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Left.GetHashCode();
                hashCode = (hashCode * 397) ^ Right.GetHashCode();
                hashCode = (hashCode * 397) ^ Top.GetHashCode();
                hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
                return hashCode;
            }
        }

        internal MarginContainer Wrap()
        {
            if (Math.Abs(Left) < float.Epsilon && Math.Abs(Right) < float.Epsilon && Math.Abs(Top) < float.Epsilon && Math.Abs(Bottom) < float.Epsilon)
                return null;

            return new MarginContainer {
                Left = Left,
                Bottom = Bottom,
                Right = Right,
                Top = Top
            };
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
