namespace Mixpanel
{
    /// <summary>
    /// There are two different url where Mixpanel messages can be sent:
    /// http://api.mixpanel.com/track and http://api.mixpanel.com/engage
    /// </summary>
    public enum MixpanelMessageEndpoint
    {
        /// <summary>
        /// Used for building http://api.mixpanel.com/track url.
        /// Two types of messages are sent to this url: 'Track' and 'Alias'.
        /// </summary>
        Track,

        /// <summary>
        /// Used for building http://api.mixpanel.com/engage url.
        /// This url is used for all 'People' messages.
        /// </summary>
        Engage
    }
}