
using System;

namespace BeautifulBlueprints
{
    public struct Size
    {
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

            //Default to smallest possible value
            Max = max ?? Math.Max(Math.Max(0, Prefer), Min);
        }

        internal Size? AsNullable()
        {
            if (Min == 0 && Prefer == 0 && Max == 0)
                return null;
            return this;
        }
    }
}
