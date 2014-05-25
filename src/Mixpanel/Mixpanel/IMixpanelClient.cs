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

        bool PeopleSet(object props);
        bool PeopleSet(object distinctId, object props);

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

        #region Super properties

        /// <summary>
        /// Sets super properties that will be attached to every event for the current mixpanel client.
        /// All previosly set super properties will be removed.
        /// </summary>
        /// <param name="superProperties">
        /// Object with super properties to set.
        /// If some of the properties are not valid mixpanel properties they will be ignored. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for valid property types. If custom 
        /// property name formatting was set in config, then it will be applied to property names.
        /// </param>
        void SetSuperProperties(object superProperties);

        /// <summary>
        /// Sets a super property for the current mixpanel client. If property with given 
        /// <param name="propertyName"></param> alredy exists, the it's value will be rewritten. 
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property. If custom property name formatting was set in config, then it will be 
        /// applied to this.
        /// </param>
        /// <param name="propertyValue">
        /// The value of the property to set. If an invalid value is provided then super property will 
        /// not be set, and if there is already property with given <param name="propertyName"></param> then
        /// it will be removed. Check documentation on project page https://github.com/eealeivan/mixpanel-csharp
        /// for supported property values.
        /// </param>
        void SetSuperProperty(string propertyName, object propertyValue);

        #endregion Super properties
    }
}
