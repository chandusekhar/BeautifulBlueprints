
namespace BeautifulBlueprints
{
    public enum SizeMode
    {
        /// <summary>
        /// This element has a fixed width (specified elsewhere)
        /// </summary>
        Fixed,

        /// <summary>
        /// This element will be automatically sized to contain children
        /// </summary>
        Auto,

        /// <summary>
        /// This element will grow to fill spare space
        /// </summary>
        Grow
    }

    public interface ISizeable
    {
        SizeMode Mode { get; }

        decimal Size { get; }
    }
}
