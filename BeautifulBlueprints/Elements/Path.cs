using BeautifulBlueprints.Layout;
using BeautifulBlueprints.Layout.Svg;
using System.Collections.Generic;

namespace BeautifulBlueprints.Elements
{
    public class Path
        : BaseElement
    {
        public string SvgPath { get; private set; }

        public bool Additive { get; private set; }

        public decimal Thickness { get; private set; }

        public decimal StartDepth { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">SVG path to place into this space. Coordinates are relative (-1,-1 indicates bottom left, 1,1 indicates top right)</param>
        /// <param name="additive"></param>
        /// <param name="name"></param>
        /// <param name="minWidth"></param>
        /// <param name="preferredWidth"></param>
        /// <param name="maxWidth"></param>
        /// <param name="minHeight"></param>
        /// <param name="preferredHeight"></param>
        /// <param name="maxHeight"></param>
        /// <param name="startDepth"></param>
        /// <param name="thickness"></param>
        public Path(
            string path,
            decimal startDepth = 0,
            decimal thickness = 1,
            bool additive = false,
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
            SvgPath = path;
            StartDepth = startDepth;
            Thickness = thickness;
            Additive = additive;
        }

        internal override IEnumerable<Solver.Solution> Solve(decimal left, decimal right, decimal top, decimal bottom)
        {
            var self = FillSpace(left, right, top, bottom);

            self.Tag = new PathLayout(SvgPath, self.Left, self.Right, self.Top, self.Bottom).Layout();

            yield return self;
        }

        internal override BaseElementContainer Wrap()
        {
            return new PathContainer(this);
        }
    }

    internal class PathContainer
        : BaseElement.BaseElementContainer
    {
        public string Path { get; set; }

        public bool Additive { get; set; }

        public decimal Thickness { get; set; }

        public decimal StartDepth { get; set; }

        public PathContainer()
        {
        }

        public PathContainer(Path path)
            : base(path)
        {
            Path = path.SvgPath;
            Additive = path.Additive;
            Thickness = path.Thickness;
            StartDepth = path.StartDepth;
        }

        public override BaseElement Unwrap()
        {
            return new Path(
                path: Path,
                name: Name,
                minWidth: MinWidth,
                preferredWidth: PreferredWidth,
                maxWidth: MaxWidth,
                minHeight: MinHeight,
                preferredHeight: PreferredHeight,
                maxHeight: MaxHeight,
                startDepth: StartDepth,
                thickness: Thickness,
                additive: Additive
            );
        }
    }
}
