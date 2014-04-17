using System;
using System.Collections;
using System.Collections.Generic;

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

        MixpanelTest TrackTest(
            string @event, object props = null, object distinctId = null,
            string ip = null, DateTime? time = null);

        bool PeopleSet(
            object distinctId = null, object props = null, string ip = null, DateTime? time = null,
            bool ignoreTime = true);

        bool PeopleSetOnce(object props);
        bool PeopleSetOnce(object distinctId, object props);

        /// <summary>
        /// Sends data to http://api.mixpanel.com/engage/ using '$add' method.
        /// Returns true if call was successful, false otherwise.
        /// </summary>
        /// <param name="props">
        /// Object containg keys and numerical values. Should also contain 'distinct_id'
        /// (if you can't have this property in the object, then use an overload).
        /// </param>
        bool PeopleAdd(object props);

        bool PeopleAdd(object distinctId, object props);

        bool PeopleAppend(object props);
        bool PeopleAppend(object distinctId, object props);

        bool PeopleUnion(object props);
        bool PeopleUnion(object distinctId, object props);

        bool PeopleUnset(IEnumerable<string> props); 
        bool PeopleUnset(object distinctId, IEnumerable<string> props);

        bool PeopleDelete(object distinctId);

        bool Alias(object distinctId, object alias);

        bool TrackCharge(object distinctId, decimal amount);
        bool TrackCharge(object distinctId, decimal amount, DateTime time);
    }
}
