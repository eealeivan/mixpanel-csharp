using System;
using System.Collections.Generic;

namespace Mixpanel
{
    public interface IMixpanelClient
    {
        #region Track

        /// <summary>
        /// Adds an event to Mixpanel by sending a message to 'http://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool Track(string @event, object properties);

        /// <summary>
        /// Adds an event to Mixpanel by sending a message to 'http://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool Track(string @event, object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'Track' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessageTest TrackTest(string @event, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'Track' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessageTest TrackTest(string @event, object distinctId, object properties);

        #endregion Track

        #region Alias

        /// <summary>
        /// Creates an alias to existing distinct id. 
        /// Message will be sent to 'http://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Original unique user profile identifier to create alias for.</param>
        /// <param name="alias">Alias for original user profile identifier.</param>
        bool Alias(object distinctId, object alias);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'Alias' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Original unique user profile identifier to create alias for.</param>
        /// <param name="alias">Alias for original user profile identifier.</param>
        MixpanelMessageTest AliasTest(object distinctId, object alias);

        #endregion Alias

        #region PeopleSet

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile. If profile doesn't exists, then new profile
        /// will be created. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool PeopleSet(object properties);

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile. If profile doesn't exists, then new profile
        /// will be created. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool PeopleSet(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleSet' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleSetTest(object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleSet' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleSetTest(object distinctId, object properties);

        #endregion PeopleSet

        #region PeopleSetOnce

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile without overwriting existing values. 
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool PeopleSetOnce(object properties);

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile without overwriting existing values. 
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool PeopleSetOnce(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleSetOnce' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleSetOnceTest(object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleSetOnce' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleSetOnceTest(object distinctId, object properties);

        #endregion PeopleSetOnce

        #region PeopleAdd

        /// <summary>
        /// The property values are added to the existing values of the properties on the profile. 
        /// If the property is not present on the profile, the value will be added to 0. 
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored. Check documentation on project page 'https://github.com/eealeivan/mixpanel-csharp' 
        /// for supported object containers.
        /// </param>
        bool PeopleAdd(object properties);

        /// <summary>
        /// The property values are added to the existing values of the properties on the profile. 
        /// If the property is not present on the profile, the value will be added to 0. 
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored. Check documentation on project page 'https://github.com/eealeivan/mixpanel-csharp' 
        /// for supported object containers.
        /// </param>
        bool PeopleAdd(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleAdd' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored. Check documentation on project page 'https://github.com/eealeivan/mixpanel-csharp' 
        /// for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleAddTest(object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleAdd' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored. Check documentation on project page 'https://github.com/eealeivan/mixpanel-csharp' 
        /// for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleAddTest(object distinctId, object properties);

        #endregion PeopleAdd

        #region PeopleAppend

        /// <summary>
        /// Appends each property value to list associated with the corresponding property name.
        /// Appending to a property that doesn't exist will result in assigning a list with one element to that property.
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        bool PeopleAppend(object properties);

        /// <summary>
        /// Appends each property value to list associated with the corresponding property name.
        /// Appending to a property that doesn't exist will result in assigning a list with one element to that property.
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
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
        /// The message will NOT be sent to Mixpanel.
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
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleAppendTest(object distinctId, object properties);

        #endregion PeopleAppend

        #region PeopleUnion

        /// <summary>
        /// Property list values will be merged with the existing lists on the user profile, ignoring 
        /// duplicate list values. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. All non collection 
        /// properties except '$distinct_id' will be ignored. Check documentation  on project page 
        /// https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        ///</param>
        bool PeopleUnion(object properties);

        ///  <summary>
        ///  Property list values will be merged with the existing lists on the user profile, ignoring 
        ///  duplicate list values. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        ///  Returns true if call was successful, and false otherwise.
        ///  </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        ///  Object containg keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        bool PeopleUnion(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleUnion' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        ///  Object containg keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleUnionTest(object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleUnion' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        ///  Object containg keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleUnionTest(object distinctId, object properties);

        #endregion

        #region PeopleUnset

        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        bool PeopleUnset(IEnumerable<string> propertyNames);

        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        bool PeopleUnset(object distinctId, IEnumerable<string> propertyNames);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleUnset' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        MixpanelMessageTest PeopleUnsetTest(IEnumerable<string> propertyNames);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleUnset' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">User unique identifier. Will be converted to string.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        MixpanelMessageTest PeopleUnsetTest(object distinctId, IEnumerable<string> propertyNames);

        #endregion PeopleUnset

        #region PeopleDelete

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties.
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        bool PeopleDelete(object distinctId);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleDelete' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        MixpanelMessageTest PeopleDeleteTest(object distinctId);

        #endregion PeopleDelete

        #region PeopleTrackCharge

        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        bool PeopleTrackCharge(object distinctId, decimal amount);

        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        bool PeopleTrackCharge(object distinctId, decimal amount, DateTime time);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleTrackCharge' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleTrackCharge' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount, DateTime time);

        #endregion PeopleTrackCharge

        #region Super properties

        /// <summary>
        /// Sets super properties that will be attached to every message for the current mixpanel client.
        /// All previosly set super properties will be removed.
        /// </summary>
        /// <param name="superProperties">
        /// Object with super properties to set.
        /// If some of the properties are not valid mixpanel properties they will be ignored. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for valid property types.
        /// </param>
        void SetSuperProperties(object superProperties);

        #endregion Super properties
    }
}
