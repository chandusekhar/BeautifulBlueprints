
using System;

namespace BeautifulBlueprints
{
    public struct Size
    {
        private const decimal VERY_LARGE_VALUE = 10000;

        public decimal Min { get; private set; }
        public decimal Prefer { get; private set; }
        public decimal Max { get; private set; }

        public Size(decimal? min = null, decimal? prefer = null, decimal? max = null)
            : this()
        {
            //Defualt min to zero
            Min = min ?? 0;

            //Default prefer to smallest size possible
            Prefer = prefer ?? Math.Min(0, Min);

            //Default to a very large value
            Max = max ?? Math.Max(Math.Max(VERY_LARGE_VALUE, Prefer), Min);
        }

        internal Size? AsNullable()
        {
            if (Min == 0 && Prefer == 0 && Max == VERY_LARGE_VALUE)
                return null;
            return this;
        }
    }
}
