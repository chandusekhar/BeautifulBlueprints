using System;
using BeautifulBlueprints.Elements;

namespace BeautifulBlueprints.Layout
{
    public class LayoutFailureException
        : Exception
    {
        public LayoutFailureException(string reason, BaseElement element)
            : base(string.Format(reason + " for element {0}({1})", element.GetType().Name, element.Name))
        {
        }
    }
}
