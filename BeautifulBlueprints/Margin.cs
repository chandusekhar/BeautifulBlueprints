
using System;
using System.ComponentModel;
using BeautifulBlueprints.Layout;

namespace BeautifulBlueprints
{
    public class Margin
        : IEquatable<Margin>
    {
        [DefaultValue(0)]
        public decimal Left { get; private set; }

        [DefaultValue(0)]
        public decimal Right { get; private set; }

        [DefaultValue(0)]
        public decimal Top { get; private set; }

        [DefaultValue(0)]
        public decimal Bottom { get; private set; }

        public Margin(decimal left = 0, decimal right = 0, decimal top = 0, decimal bottom = 0)
        {
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
        }

        public bool Equals(Margin other)
        {
            return other.Left.IsEqualTo(Left)
                && other.Right.IsEqualTo(Right)
                && other.Top.IsEqualTo(Top)
                && other.Bottom.IsEqualTo(Bottom);
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
            if (Left.IsEqualTo(0) && Right.IsEqualTo(0) && Top.IsEqualTo(0) && Bottom.IsEqualTo(0))
                return null;

            return new MarginContainer {
                Left = Left,
                Bottom = Bottom,
                Right = Right,
                Top = Top
            };
        }
    }

    public static class MarginExtensions
    {
        public static decimal Width(this Margin margin)
        {
            if (margin == null)
                return 0;
            return margin.Left + margin.Right;
        }

        public static decimal Height(this Margin margin)
        {
            if (margin == null)
                return 0;
            return margin.Top + margin.Bottom;
        }
    }

    internal class MarginContainer
    {
        [DefaultValue(0)]
        public decimal Left { get; set; }

        [DefaultValue(0)]
        public decimal Right { get; set; }

        [DefaultValue(0)]
        public decimal Top { get; set; }

        [DefaultValue(0)]
        public decimal Bottom { get; set; }

        public Margin Unwrap()
        {
            return new Margin(left: Left, right: Right, top: Top, bottom: Bottom);
        }
    }
}
