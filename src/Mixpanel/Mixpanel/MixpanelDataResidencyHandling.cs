namespace Mixpanel
{
    /// <summary>
    /// Controls which API host to route data to.
    /// </summary>
    public enum MixpanelDataResidencyHandling
    {
        /// <summary>
        /// Route data to Mixpanel's US servers.
        /// </summary>
        Default,

        /// <summary>
        /// Route data to Mixpanel's EU servers.
        /// </summary>
        EU,
    }
}