using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace Mixpanel
{
    /// <summary>
    /// Provides methods to work with Mixpanel.
    /// </summary>
    public interface IMixpanelClient
    {
        #region Track

        /// <summary>
        /// Send a message to 'https://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        Task<bool> TrackAsync(string @event, object properties);

        /// <summary>
        /// Send a message to 'https://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        Task<bool> TrackAsync(string @event, object distinctId, object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> created from provided data.
        /// If message can't be created, then null is returned. 
        /// The message will NOT be sent to Mixpanel.
        /// You can send <see cref="MixpanelMessage"/> using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetTrackMessage(string @event, object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> created from provided data.
        /// If message can't be created, then null is returned. 
        /// The message will NOT be sent to Mixpanel.
        /// You can send <see cref="MixpanelMessage"/> using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetTrackMessage(string @event, object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'Track' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
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
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessageTest TrackTest(string @event, object distinctId, object properties);

        #endregion Track

        #region Alias

        /// <summary>
        /// Creates an alias to 'Distinct ID' that is provided with super properties. 
        /// Message will be sent to 'https://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="alias">Alias for original user profile identifier.</param>
        Task<bool> AliasAsync(object alias);

        /// <summary>
        /// Creates an alias to given <paramref name="distinctId"/>. 
        /// Message will be sent to 'https://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Original unique user profile identifier to create alias for.</param>
        /// <param name="alias">Alias for original user profile identifier.</param>
        Task<bool> AliasAsync(object distinctId, object alias);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'Alias'. 
        /// 'Distinct ID' must ne set with super properties.
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="alias">Alias for original user profile identifier.</param>
        MixpanelMessage GetAliasMessage(object alias);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'Alias'. 
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Original unique user profile identifier to create alias for.</param>
        /// <param name="alias">Alias for original user profile identifier.</param>
        MixpanelMessage GetAliasMessage(object distinctId, object alias);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'Alias' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="alias">Alias for original user profile identifier.</param>
        MixpanelMessageTest AliasTest(object alias);

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
        /// will be created. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool PeopleSet(object properties);

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile. If profile doesn't exists, then new profile
        /// will be created. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool PeopleSet(object distinctId, object properties);

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile. If profile doesn't exists, then new profile
        /// will be created. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        Task<bool> PeopleSetAsync(object properties);

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile. If profile doesn't exists, then new profile
        /// will be created. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        Task<bool> PeopleSetAsync(object distinctId, object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleSet' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleSetMessage(object properties);
        
        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleSet' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleSetMessage(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleSet' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
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
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleSetTest(object distinctId, object properties);

        #endregion PeopleSet

        #region PeopleSetOnce

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile without overwriting existing values. 
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool PeopleSetOnce(object properties);

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile without overwriting existing values. 
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        bool PeopleSetOnce(object distinctId, object properties);

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile without overwriting existing values. 
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        Task<bool> PeopleSetOnceAsync(object properties);

        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile without overwriting existing values. 
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        Task<bool> PeopleSetOnceAsync(object distinctId, object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleSetOnce' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleSetOnceMessage(object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleSetOnce' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleSetOnceMessage(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleSetOnce' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
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
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleSetOnceTest(object distinctId, object properties);

        #endregion PeopleSetOnce

        #region PeopleAdd

        /// <summary>
        /// The property values are added to the existing values of the properties on the profile. 
        /// If the property is not present on the profile, the value will be added to 0. 
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored. Check documentation on project page 'https://github.com/eealeivan/mixpanel-csharp' 
        /// for supported object containers.
        /// </param>
        bool PeopleAdd(object properties);

        /// <summary>
        /// The property values are added to the existing values of the properties on the profile. 
        /// If the property is not present on the profile, the value will be added to 0. 
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored. Check documentation on project page 'https://github.com/eealeivan/mixpanel-csharp' 
        /// for supported object containers.
        /// </param>
        bool PeopleAdd(object distinctId, object properties);

        /// <summary>
        /// The property values are added to the existing values of the properties on the profile. 
        /// If the property is not present on the profile, the value will be added to 0. 
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored. Check documentation on project page 'https://github.com/eealeivan/mixpanel-csharp' 
        /// for supported object containers.
        /// </param>
        Task<bool> PeopleAddAsync(object properties);

        /// <summary>
        /// The property values are added to the existing values of the properties on the profile. 
        /// If the property is not present on the profile, the value will be added to 0. 
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored. Check documentation on project page 'https://github.com/eealeivan/mixpanel-csharp' 
        /// for supported object containers.
        /// </param>
        Task<bool> PeopleAddAsync(object distinctId, object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleAdd' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleAddMessage(object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleAdd' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties"> 
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleAddMessage(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleAdd' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and numeric values. All non numeric properties except '$distinct_id'
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
        /// Object containing keys and numeric values. All non numeric properties except '$distinct_id'
        /// will be ignored. Check documentation on project page 'https://github.com/eealeivan/mixpanel-csharp' 
        /// for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleAddTest(object distinctId, object properties);

        #endregion PeopleAdd

        #region PeopleAppend

        /// <summary>
        /// Appends each property value to list associated with the corresponding property name.
        /// Appending to a property that doesn't exist will result in assigning a list with one element to that property.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        bool PeopleAppend(object properties);

        /// <summary>
        /// Appends each property value to list associated with the corresponding property name.
        /// Appending to a property that doesn't exist will result in assigning a list with one element to that property.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        bool PeopleAppend(object distinctId, object properties);

        /// <summary>
        /// Appends each property value to list associated with the corresponding property name.
        /// Appending to a property that doesn't exist will result in assigning a list with one element to that property.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        Task<bool> PeopleAppendAsync(object properties);

        /// <summary>
        /// Appends each property value to list associated with the corresponding property name.
        /// Appending to a property that doesn't exist will result in assigning a list with one element to that property.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        Task<bool> PeopleAppendAsync(object distinctId, object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleAppend' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleAppendMessage(object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleAppend' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleAppendMessage(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleAppend' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
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
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleAppendTest(object distinctId, object properties);

        #endregion PeopleAppend

        #region PeopleUnion

        /// <summary>
        /// Property list values will be merged with the existing lists on the user profile, ignoring 
        /// duplicate list values. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. All non collection 
        /// properties except '$distinct_id' will be ignored. Check documentation  on project page 
        /// https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        ///</param>
        bool PeopleUnion(object properties);

        ///  <summary>
        ///  Property list values will be merged with the existing lists on the user profile, ignoring 
        ///  duplicate list values. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        ///  Returns true if call was successful, and false otherwise.
        ///  </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        ///  Object containing keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        bool PeopleUnion(object distinctId, object properties);
        
        /// <summary>
        /// Property list values will be merged with the existing lists on the user profile, ignoring 
        /// duplicate list values. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. All non collection 
        /// properties except '$distinct_id' will be ignored. Check documentation  on project page 
        /// https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        ///</param>
        Task<bool> PeopleUnionAsync(object properties);

        ///  <summary>
        ///  Property list values will be merged with the existing lists on the user profile, ignoring 
        ///  duplicate list values. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        ///  Returns true if call was successful, and false otherwise.
        ///  </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        ///  Object containing keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        Task<bool> PeopleUnionAsync(object distinctId, object properties);
        
        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleUnion' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        ///  Object containing keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleUnionMessage(object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleUnion' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        ///  Object containing keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleUnionMessage(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleUnion' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        ///  Object containing keys and values that will be parsed and sent to Mixpanel. All non collection 
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
        ///  Object containing keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleUnionTest(object distinctId, object properties);

        #endregion

        #region PeopleRemove

        /// <summary>
        /// Removes each property value from list associated with the corresponding property name.
        /// If list with the corresponding property name does not exist, no updates are made.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        bool PeopleRemove(object properties);

        /// <summary>
        /// Removes each property value from list associated with the corresponding property name.
        /// If list with the corresponding property name does not exist, no updates are made.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        bool PeopleRemove(object distinctId, object properties);

        /// <summary>
        /// Removes each property value from list associated with the corresponding property name.
        /// If list with the corresponding property name does not exist, no updates are made.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        Task<bool> PeopleRemoveAsync(object properties);

        /// <summary>
        /// Removes each property value from list associated with the corresponding property name.
        /// If list with the corresponding property name does not exist, no updates are made.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        Task<bool> PeopleRemoveAsync(object distinctId, object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleRemove' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleRemoveMessage(object properties);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleRemove' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessage GetPeopleRemoveMessage(object distinctId, object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleRemove' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleRemoveTest(object properties);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleRemove' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containing keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        MixpanelMessageTest PeopleRemoveTest(object distinctId, object properties);

        #endregion

        #region PeopleUnset

        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        bool PeopleUnset(IEnumerable<string> propertyNames);

        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        bool PeopleUnset(object distinctId, IEnumerable<string> propertyNames);

        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        Task<bool> PeopleUnsetAsync(IEnumerable<string> propertyNames);

        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        Task<bool> PeopleUnsetAsync(object distinctId, IEnumerable<string> propertyNames);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleUnset' that contains parsed data from 
        /// <paramref name="propertyNames"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        MixpanelMessage GetPeopleUnsetMessage(IEnumerable<string> propertyNames);
        
        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleUnset' that contains parsed data from 
        /// <paramref name="propertyNames"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">User unique identifier. Will be converted to string.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        MixpanelMessage GetPeopleUnsetMessage(object distinctId, IEnumerable<string> propertyNames);

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
        /// 'Distinct ID' will be taken from super properties.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        bool PeopleDelete();

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        bool PeopleDelete(object distinctId);

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties.
        /// 'Distinct ID' will be taken from super properties.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        Task<bool> PeopleDeleteAsync();

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        Task<bool> PeopleDeleteAsync(object distinctId);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleDelete'. 
        /// 'Distinct ID' will be taken from super properties.
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        MixpanelMessage GetPeopleDeleteMessage();

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleDelete'. 
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        MixpanelMessage GetPeopleDeleteMessage(object distinctId);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleDelete' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        MixpanelMessageTest PeopleDeleteTest();
        
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
        /// Adds new transaction to profile. 'Distinct ID' will be taken from super properties.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="amount">Amount of the transaction.</param>
        bool PeopleTrackCharge(decimal amount);
        
        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        bool PeopleTrackCharge(object distinctId, decimal amount);

        /// <summary>
        /// Adds new transaction to profile. 'Distinct ID' will be taken from super properties.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        bool PeopleTrackCharge(decimal amount, DateTime time);   
        
        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        bool PeopleTrackCharge(object distinctId, decimal amount, DateTime time);

        /// <summary>
        /// Adds new transaction to profile. 'Distinct ID' will be taken from super properties.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="amount">Amount of the transaction.</param>
        Task<bool> PeopleTrackChargeAsync(decimal amount);
        
        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        Task<bool> PeopleTrackChargeAsync(object distinctId, decimal amount);

        /// <summary>
        /// Adds new transaction to profile. 'Distinct ID' will be taken from super properties.
        /// Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        Task<bool> PeopleTrackChargeAsync(decimal amount, DateTime time);
        
        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'https://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        Task<bool> PeopleTrackChargeAsync(object distinctId, decimal amount, DateTime time);

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleTrackCharge'. 
        /// 'Distinct ID' will be taken from super properties.
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="amount">Amount of the transaction.</param>
        MixpanelMessage GetPeopleTrackChargeMessage(decimal amount);    
        
        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleTrackCharge'. 
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        MixpanelMessage GetPeopleTrackChargeMessage(object distinctId, decimal amount);        
        
        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleTrackCharge'. 
        /// 'Distinct ID' will be taken from super properties.
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        MixpanelMessage GetPeopleTrackChargeMessage(decimal amount, DateTime time); 
        
        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleTrackCharge'. 
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        MixpanelMessage GetPeopleTrackChargeMessage(object distinctId, decimal amount, DateTime time);

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleTrackCharge' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="amount">Amount of the transaction.</param>
        MixpanelMessageTest PeopleTrackChargeTest(decimal amount);
        
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
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        MixpanelMessageTest PeopleTrackChargeTest(decimal amount, DateTime time);
        
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

        #region Send

        /// <summary>
        /// Sends messages passed in <paramref name="messages"/> parameter to Mixpanel.
        /// If <paramref name="messages"/> contains both track (Track and Alias) and engage (People*)
        /// messages then they will be divided in 2 batches and will be sent separately. 
        /// If amount of messages of one type exceeds 50,  then messages will be divided in batches
        /// and will be sent separately.
        /// Returns a <see cref="SendResult"/> object that contains lists of success and failed batches. 
        /// </summary>
        /// <param name="messages">List of <see cref="MixpanelMessage"/> to send.</param>
        SendResult Send(params MixpanelMessage[] messages);

        /// <summary>
        /// Sends messages passed in <paramref name="messages"/> parameter to Mixpanel.
        /// If <paramref name="messages"/> contains both track (Track and Alias) and engage (People*)
        /// messages then they will be divided in 2 batches and will be sent separately. 
        /// If amount of messages of one type exceeds 50,  then messages will be divided in batches
        /// and will be sent separately.
        /// Returns a <see cref="SendResult"/> object that contains lists of success and failed batches. 
        /// </summary>
        /// <param name="messages">List of <see cref="MixpanelMessage"/> to send.</param>
        SendResult Send(IEnumerable<MixpanelMessage> messages);

        /// <summary>
        /// Sends messages passed in <paramref name="messages"/> parameter to Mixpanel.
        /// If <paramref name="messages"/> contains both track (Track and Alias) and engage (People*)
        /// messages then they will be divided in 2 batches and will be sent separately. 
        /// If amount of messages of one type exceeds 50,  then messages will be divided in batches
        /// and will be sent separately.
        /// Returns a <see cref="SendResult"/> object that contains lists of success and failed batches. 
        /// </summary>
        /// <param name="messages">List of <see cref="MixpanelMessage"/> to send.</param>
        Task<SendResult> SendAsync(params MixpanelMessage[] messages);

        /// <summary>
        /// Sends messages passed in <paramref name="messages"/> parameter to Mixpanel.
        /// If <paramref name="messages"/> contains both track (Track and Alias) and engage (People*)
        /// messages then they will be divided in 2 batches and will be sent separately. 
        /// If amount of messages of one type exceeds 50,  then messages will be divided in batches
        /// and will be sent separately.
        /// Returns a <see cref="SendResult"/> object that contains lists of success and failed batches. 
        /// </summary>
        /// <param name="messages">List of <see cref="MixpanelMessage"/> to send.</param>
        Task<SendResult> SendAsync(IEnumerable<MixpanelMessage> messages);

        /// <summary>
        /// Returns a collection of <see cref="MixpanelBatchMessageTest"/>. Each item represents a
        /// batch that contains all steps (message data, JSON, base64) of building a batch message. 
        /// If some error occurs during the process of creating a batch it can be found in 
        /// <see cref="MixpanelMessageTest.Exception"/> property.
        /// The messages will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="messages">List of <see cref="MixpanelMessage"/> to test.</param>
        ReadOnlyCollection<MixpanelBatchMessageTest> SendTest(IEnumerable<MixpanelMessage> messages);

        /// <summary>
        /// Returns a collection of <see cref="MixpanelBatchMessageTest"/>. Each item represents a
        /// batch that contains all steps (message data, JSON, base64) of building a batch message. 
        /// If some error occurs during the process of creating a batch it can be found in 
        /// <see cref="MixpanelMessageTest.Exception"/> property.
        /// The messages will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="messages">List of <see cref="MixpanelMessage"/> to test.</param>
        ReadOnlyCollection<MixpanelBatchMessageTest> SendTest(params MixpanelMessage[] messages);

        #endregion Send

        #region SendJson

        /// <summary>
        /// Sends <paramref name="messageJson"/> to given <paramref name="endpoint"/>.
        /// This method gives you total control of what message will be sent to Mixpanel.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="endpoint">Endpoint where message will be sent.</param>
        /// <param name="messageJson">
        /// Raw JSON without any encoding.
        /// </param>
        bool SendJson(MixpanelMessageEndpoint endpoint, string messageJson);

        /// <summary>
        /// Sends <paramref name="messageJson"/> to given <paramref name="endpoint"/>.
        /// This method gives you total control of what message will be sent to Mixpanel.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="endpoint">Endpoint where message will be sent.</param>
        /// <param name="messageJson">
        /// Raw JSON without any encoding.
        /// </param>
        Task<bool> SendJsonAsync(MixpanelMessageEndpoint endpoint, string messageJson);

        #endregion SendJson
    }
}
