namespace Mixpanel
{
    /// <summary>
    /// There are two different URLs where Mixpanel messages can be sent:
    /// https://api.mixpanel.com/track and https://api.mixpanel.com/engage
    /// </summary>
    public enum MixpanelMessageEndpoint
    {
        /// <summary>
        /// Used for building https://api.mixpanel.com/track URL.
        /// Two types of messages are sent to this URL: 'Track' and 'Alias'.
        /// </summary>
        Track,

        /// <summary>
        /// Used for building https://api.mixpanel.com/engage URL.
        /// This URL is used for all 'People' messages.
        /// </summary>
        Engage
    }
}