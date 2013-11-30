using System;

namespace Mixpanel
{
    public interface IMixpanelClient
    {
        /// <summary>
        /// Sends an event to http://api.mixpanel.com/track/ endpoint.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="props"></param>
        /// <param name="distinctId"></param>
        /// <param name="ip"></param>
        /// <param name="time"></param>
        /// <returns><value>true</value> if call was successful, and <value>false</value> otherwise.</returns>
        bool Track(
            string @event, object props = null, object distinctId = null,
            string ip = null, DateTime? time = null);
    }
}
