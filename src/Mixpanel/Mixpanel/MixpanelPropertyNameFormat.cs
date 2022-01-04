namespace Mixpanel
{
    /// <summary>
    /// Type of name formatting that will be applied when generating property names.
    /// Formatting will be applied only for properties coming from classes that
    /// do not have an explicit <see cref="MixpanelNameAttribute"/>.
    /// Formatting supports two-letter acronyms as described in
    /// https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/capitalization-conventions.
    /// </summary>
    public enum MixpanelPropertyNameFormat
    {
        /// <summary>
        /// No formatting will be applied. This is a default value.
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