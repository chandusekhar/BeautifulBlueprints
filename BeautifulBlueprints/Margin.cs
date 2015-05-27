
using System.ComponentModel;

namespace BeautifulBlueprints
{
    public struct Margin
    {
        [DefaultValue(0)]
        public float Left { get; set; }

        [DefaultValue(0)]
        public float Right { get; set; }

        [DefaultValue(0)]
        public float Top { get; set; }

        [DefaultValue(0)]
        public float Bottom { get; set; }
    }
}
