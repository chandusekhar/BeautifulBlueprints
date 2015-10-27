using System.Collections.Generic;
using BeautifulBlueprints.Elements;

namespace BeautifulBlueprints.Layout
{
    public interface ILayoutContainer
        : IEnumerable<KeyValuePair<string, string>>
    {
        string this[string key] { get; }

        BaseElement Root { get; }
    }
}
