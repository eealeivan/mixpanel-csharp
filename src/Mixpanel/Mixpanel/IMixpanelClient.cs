using System;
using System.Collections.Generic;

namespace Mixpanel
{
    public interface IMixpanelClient
    {
        #region Track

        //TODO: Documentation. <Aleksandr Ivanov - 26-05-2014>

        /// <summary>
        /// Sends a message to http://api.mixpanel.com/track/ endpoint.
        /// Returns <value>true</value> if call was successful, and <value>false</value> otherwise.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="properties"></param>
        bool Track(string @event, object properties);

        bool Track(string @event, object distinctId, object properties);

        MixpanelMessageTest TrackTest(string @event, object properties);
        MixpanelMessageTest TrackTest(string @event, object distinctId, object properties);

        #endregion Track

        #region PeopleSet

        bool PeopleSet(object properties);
        bool PeopleSet(object distinctId, object properties);

        MixpanelMessageTest PeopleSetTest(object properties);
        MixpanelMessageTest PeopleSetTest(object distinctId, object properties);

        #endregion PeopleSet

        bool PeopleSetOnce(object properties);
        bool PeopleSetOnce(object distinctId, object properties);

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

        #region PeopleDelete

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        bool PeopleDelete(object distinctId);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (dictionary, JSON,
        /// base64) of building 'PeopleDelete' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        MixpanelMessageTest PeopleDeleteTest(object distinctId);

        #endregion PeopleDelete


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
