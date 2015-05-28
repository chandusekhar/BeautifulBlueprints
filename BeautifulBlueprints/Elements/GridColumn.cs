
using System;

namespace BeautifulBlueprints.Elements
{
    public class GridColumn
        : IEquatable<GridColumn>, ISizeable
    {
        public float Size { get; private set; }

        public SizeMode Mode { get; private set; }

        public GridColumn(float size, SizeMode mode)
        {
            Size = size;
            Mode = mode;
        }

        public bool Equals(GridColumn other)
        {
            return Math.Abs(other.Size - Size) < float.Epsilon
                && other.Mode == Mode;
        }

        public override bool Equals(object obj)
        {
            var col = obj as GridColumn;
            if (col != null)
                return Equals(col);

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Size.GetHashCode() * 397) ^ (int)Mode;
            }
        }

        internal GridColumnContainer Wrap()
        {
            return new GridColumnContainer {
                Size = Size,
                Mode = Mode
            };
        }
    }

    internal class GridColumnContainer
    {
        public float Size { get; set; }

        public SizeMode Mode { get; set; }

        public GridColumn Unwrap()
        {
            return new GridColumn(Size, Mode);
        }
    }
}
