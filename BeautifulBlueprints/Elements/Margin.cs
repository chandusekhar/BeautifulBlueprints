using System.ComponentModel;
using BeautifulBlueprints.Layout;
using System;
using System.Collections.Generic;

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
            throw new NotImplementedException();
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
