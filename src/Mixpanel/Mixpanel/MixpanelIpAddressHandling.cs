namespace Mixpanel
{
    /// <summary>
    /// Controls request "ip" query string parameter. 
    /// </summary>
    public enum MixpanelIpAddressHandling
    {
        /// <summary>
        /// "ip" query string parameter will not be added to requests.
        /// </summary>
        None,

        /// <summary>
        /// "ip=1" query string parameter will be added to each request.
        /// </summary>
        UseRequestIp,

        /// <summary>
        /// "ip=0" query string parameter will be added to each request.
        /// </summary>
        IgnoreRequestIp
    }
}