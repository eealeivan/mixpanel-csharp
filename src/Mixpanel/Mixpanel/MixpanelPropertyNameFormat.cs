namespace Mixpanel
{
    /// <summary>
    /// Type of name formatting that will be applied when generating property names.
    /// Works only for classes (not dictionaries).
    /// </summary>
    public enum MixpanelPropertyNameFormat
    {
        /// <summary>
        /// No formatting will be applied. This is default value.
        /// </summary>
        None,

        /// <summary>
        /// Property name will be parsed in sentence with only first word capitalized.
        /// Example: 'MySuperProperty' -> 'My super property'.
        /// </summary>
        SentenceCase,

        /// <summary>
        /// Property name will be parsed in sentence with all words capitalized.
        /// Example: 'MySuperProperty' -> 'My Super Property'.
        /// </summary>
        TitleCase,

        /// <summary>
        /// Property name will be parsed in sentence with no words capitalized.
        /// Example: 'MySuperProperty' -> 'my super property'.
        /// </summary>
        LowerCase
    }
}