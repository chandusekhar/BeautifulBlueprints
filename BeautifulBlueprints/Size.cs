
using System;
using System.ComponentModel;

namespace BeautifulBlueprints
{
    public struct Size
    {
        public decimal Min { get; set; }
        public decimal Prefer { get; set; }
        public decimal Max { get; set; }

        public Size(decimal? min = null, decimal? prefer = null, decimal? max = null)
            : this()
        {
            //Defualt min to zero
            Min = min ?? 0;

            //Default prefer to smallest size possible
            Prefer = prefer ?? Math.Max(0, Min);

            //Default to smallest possible value
            Max = max ?? Math.Max(Math.Max(0, Prefer), Min);
        }

        internal SizeContainer? Wrap()
        {
            if (Min == 0 && Prefer == 0 && Max == 0)
                return null;
            return new SizeContainer(Min, Prefer, Max);
        }
    }

    internal struct SizeContainer
    {
        [DefaultValue(null)]
        public decimal? Min { get; set; }

        [DefaultValue(null)]
        public decimal? Prefer { get; set; }

        [DefaultValue(null)]
        public decimal? Max { get; set; }

        public SizeContainer(decimal min, decimal prefer, decimal max)
            : this()
        {
            Min = min == 0 ? null : (decimal?)min;
            Prefer = prefer == min ? null : (decimal?)min;
            Max = max == prefer? null : (decimal?)max;
        }

        internal Size Unwrap()
        {
            //If the size is specified it might only be *partially* specified
            return new Size(Min, Prefer, Max);
        }
    }

    internal static class SizeContainerExtensions
    {
        public static Size? Unwrap(this SizeContainer? container)
        {
            if (container.HasValue)
                return container.Value.Unwrap();
            return null;
        }
    }
}
