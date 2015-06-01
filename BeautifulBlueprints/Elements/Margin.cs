using BeautifulBlueprints.Layout;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BeautifulBlueprints.Elements
{
    public class Margin
        : BaseContainerElement
    {
        public Size Left { get; private set; }
        public Size Top { get; private set; }
        public Size Right { get; private set; }
        public Size Bottom { get; private set; }

        public Margin(
            Size? left = null,
            Size? top = null,
            Size? right = null,
            Size? bottom = null,
            string name = null,
            decimal minWidth = DEFAULT_MIN_WIDTH,
            decimal? preferredWidth = null,
            decimal maxWidth = DEFAULT_MAX_WIDTH,
            decimal minHeight = DEFAULT_MIN_HEIGHT,
            decimal? preferredHeight = null,
            decimal maxHeight = DEFAULT_MAX_HEIGHT
        )
            : base(name, minWidth, preferredWidth, maxWidth, minHeight, preferredHeight, maxHeight)

        {
            Left = left ?? new Size();
            Top = top ?? new Size();
            Right = right ?? new Size();
            Bottom = bottom ?? new Size();
        }

        internal override IEnumerable<Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom)
        {
            var solutions = new List<Solver.Solution>();

            var self = FillSpace(left, right, top, bottom);
            solutions.Add(self);

            if (ChildCount == 0)
                return solutions;

            var child = Children.Single();

            //Calculate the sizes of the 4 margins
            decimal l, r, t, b;
            Resize(out l, out r, Left, Right, self.Right - self.Left, child.MinWidth, child.PreferredWidth, child.MaxWidth);
            Resize(out t, out b, Top, Bottom, self.Top - self.Bottom, child.MinHeight, child.PreferredHeight, child.MaxHeight);

            //Size the child within the calculated margins
            solutions.AddRange(child.Solve(left + l, right - r, top - t, bottom + b));
            return solutions;
        }

        /// <summary>
        /// Calculate the size of the margins. The space in the middle (with min/max/preferred size) must fit between the margins
        /// </summary>
        /// <param name="margin1"></param>
        /// <param name="margin2"></param>
        /// <param name="m1Size"></param>
        /// <param name="m2Size"></param>
        /// <param name="extent"></param>
        /// <param name="childMin"></param>
        /// <param name="childPrefer"></param>
        /// <param name="childMax"></param>
        private void Resize(out decimal margin1, out decimal margin2, Size m1Size, Size m2Size, decimal extent, decimal childMin, decimal childPrefer, decimal childMax)
        {
            margin1 = m1Size.Min;
            margin2 = m2Size.Min;
            var child = childMin;

            //spare amount of space to distribute to things
            var spareSize = extent - (m1Size.Min + m2Size.Min + childMin);
            if (spareSize < 0)
                throw new LayoutFailureException("Space available is too small for the minimum possible size of the margins and the child element", this);

            //Bail out if we can't possibly expand up to the size available
            if (childMax + m1Size.Max + m2Size.Max < extent)
                throw new LayoutFailureException("Space available is too big for the maximum possible size of the margins and the child element", this);

            //Try to expand the child to it's preferred size
            var spaceToExpandChildToPreferred = childPrefer - child;
            if (spareSize - spaceToExpandChildToPreferred < 0)
            {
                //Consume all spare space expanding the child up towards it's preferred size
                child += spareSize;
                return;
            }

            //We have the space, so expand the child all the way up to it's preferred size
            child += spaceToExpandChildToPreferred;
            spareSize -= spaceToExpandChildToPreferred;

            //Now expand the margins up to their preferred size
            var spaceToExpandMarginsToPreferred = (m1Size.Prefer + m2Size.Prefer) - (margin1 + margin2);
            var prefRatio = m1Size.Prefer / (m1Size.Prefer + m2Size.Prefer);
            if (spareSize - spaceToExpandMarginsToPreferred < 0)
            {
                //Consume all spare space expanding margins up towards preferred (in ratio of preferences)
                margin1 += spareSize * prefRatio;
                margin2 += spareSize * (1 - prefRatio);
                return;
            }

            //Expand margins to their preferred size
            spareSize -= (m1Size.Prefer - margin1);
            margin1 = m1Size.Prefer;
            spareSize -= (m2Size.Prefer - margin2);
            margin2 = m2Size.Prefer;

            //If the space is less than the max of the child we're ok!
            if (spareSize < childMax)
                return;
            else
                spareSize -= (childMax - child);

            //Expand margins (with remaining space, in ratio of preferred sizes
            var s1 = spareSize * prefRatio;
            var s2 = spareSize * (1 - prefRatio);
            spareSize -= ExpandMargin(ref margin1, m1Size, s1);
            spareSize -= ExpandMargin(ref margin2, m2Size, s2);

            //If there's *still* spare space, one of the margins must have hit it's max size, add all the space to the other one
            if (margin1 == m1Size.Max)
                spareSize -= ExpandMargin(ref margin2, m2Size, spareSize);
            else
                spareSize -= ExpandMargin(ref margin1, m1Size, spareSize);

            //if there's *still* spare size we're doomed
            if (spareSize > 0)
                throw new LayoutFailureException("Still spare size to allocate even after margins have reached maximum size", this);
        }

        private static decimal ExpandMargin(ref decimal margin, Size size, decimal amount)
        {
            if (margin + amount > size.Max)
            {
                var used = (size.Max - margin);
                margin = size.Max;
                return used;
            }
            else
            {
                margin += amount;
                return amount;
            }
        }

        internal override BaseElementContainer Wrap()
        {
            return new MarginContainer(this);
        }

        protected override int MaximumChildren
        {
            get
            {
                return 1;
            }
        }
    }

    internal class MarginContainer
        : BaseContainerElement.BaseContainerElementContainer
    {
        [DefaultValue(null)]
        public Size? Left { get; private set; }

        [DefaultValue(null)]
        public Size? Top { get; private set; }

        [DefaultValue(null)]
        public Size? Right { get; private set; }

        [DefaultValue(null)]
        public Size? Bottom { get; private set; }

        public MarginContainer()
        {
        }

        public MarginContainer(Margin margin)
            : base(margin)
        {
            Left = margin.Left.AsNullable();
            Top = margin.Top.AsNullable();
            Right = margin.Right.AsNullable();
            Bottom = margin.Bottom.AsNullable();
        }

        public override BaseElement Unwrap()
        {
            return new Margin(Left, Top, Right, Bottom,
                name: Name,
                minWidth: MinWidth,
                preferredWidth: PreferredWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight,
                maxHeight: MaxHeight
            );
        }
    }
}
