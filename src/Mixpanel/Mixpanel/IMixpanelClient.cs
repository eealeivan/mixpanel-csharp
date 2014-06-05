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

        #region Alias

        bool Alias(object distinctId, object alias);
        MixpanelMessageTest AliasTest(object distinctId, object alias);

        #endregion Alias

        #region PeopleSet

        bool PeopleSet(object properties);
        bool PeopleSet(object distinctId, object properties);

        MixpanelMessageTest PeopleSetTest(object properties);
        MixpanelMessageTest PeopleSetTest(object distinctId, object properties);

        #endregion PeopleSet

        bool PeopleSetOnce(object properties);
        bool PeopleSetOnce(object distinctId, object properties);

        #region PeopleAdd

        /// <summary>
        /// The property values are added to the existing values of the properties on the profile. 
        /// If the property is not present on the profile, the value will be added to 0. 
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored.
        /// </param>
        bool PeopleAdd(object properties);

        bool PeopleAdd(object distinctId, object properties);

        #endregion PeopleAdd

        #region PeopleAppend

        /// <summary>
        /// Takes an object containing keys and values, and appends each to a list associated with 
        /// the corresponding property name. Appending to a property that doesn't exist will result 
        /// in assigning a list with one element to that property.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        bool PeopleAppend(object properties);

        /// <summary>
        /// Takes an object containing keys and values, and appends each to a list associated with 
        /// the corresponding property name. Appending to a property that doesn't exist will result 
        /// in assigning a list with one element to that property.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        bool PeopleAppend(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleAppend' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleAppendTest(object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleAppend' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleAppendTest(object distinctId, object properties);

        #endregion PeopleAppend

        bool PeopleUnion(object props);
        bool PeopleUnion(object distinctId, object props);

        #region PeopleSet

        /// <summary>
        /// Takes a list of string property names, and permanently removes the properties 
        /// and their values from a profile. Use this method if you have set 'distinct_id'
        /// in super properties.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        bool PeopleUnset(IEnumerable<string> propertyNames);

        /// <summary>
        /// Takes a list of string property names, and permanently removes the properties 
        /// and their values from a profile.
        /// </summary>
        /// <param name="distinctId">User unique identifier. Will be converted to string.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        bool PeopleUnset(object distinctId, IEnumerable<string> propertyNames);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleUnset' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        MixpanelMessageTest PeopleUnsetTest(IEnumerable<string> propertyNames);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleUnset' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="distinctId">User unique identifier. Will be converted to string.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        MixpanelMessageTest PeopleUnsetTest(object distinctId, IEnumerable<string> propertyNames);

        #endregion PeopleSet

        #region PeopleDelete

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        bool PeopleDelete(object distinctId);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleDelete' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        MixpanelMessageTest PeopleDeleteTest(object distinctId);

        #endregion PeopleDelete

        bool PeopleTrackCharge(object distinctId, decimal amount);
        bool PeopleTrackCharge(object distinctId, decimal amount, DateTime time);

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

        MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount);
        MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount, DateTime time);
        MixpanelMessageTest PeopleAddTest(object properties);
        MixpanelMessageTest PeopleAddTest(object distinctId, object properties);
    }
}
