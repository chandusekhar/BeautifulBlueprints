using System;

namespace BeautifulBlueprints.Layout
{
    public class LayoutFailureException
        : Exception
    {
        public LayoutFailureException(string reason)
            : base(reason)
        {
        }
    }
}
