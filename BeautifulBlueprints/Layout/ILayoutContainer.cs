using System;
using System.Collections.Generic;
using BeautifulBlueprints.Elements;

namespace BeautifulBlueprints.Layout
{
    public interface ILayoutContainer
        : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Get the tag value for the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string this[string key] { get; }

        /// <summary>
        /// Get the root element of this layout
        /// </summary>
        BaseElement Root { get; }

        /// <summary>
        /// Get the unique ID of this layout
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Get the description of this layout
        /// </summary>
        string Description { get; }
    }
}
