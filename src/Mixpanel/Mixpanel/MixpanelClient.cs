using System;
using System.Collections.Generic;
using System.Text;
using Mixpanel.Core.Message;

namespace Mixpanel
{
    public class MixpanelClient : IMixpanelClient
    {
        private const string UrlFormat = "http://api.mixpanel.com/{0}";
        private const string EndpointTrack = "track";
        private const string EndpointEngage = "engage";

        private readonly string _token;
        private readonly MixpanelConfig _config;

        /// <summary>
        /// Func for getting/setting current utc date. Simplifies testing.
        /// </summary>
        internal Func<DateTime> UtcNow { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelClient"/>.
        /// </summary>
        /// <param name="token">
        /// The Mixpanel token associated with your project. You can find your Mixpanel token in the 
        /// project settings dialog in the Mixpanel app. Events without a valid token will be ignored.</param>
        /// <param name="config">
        /// Configuration for this particular client. Set properties from this class will override global properties.
        /// </param>
        /// <param name="superProperties">
        /// Object with properties that will be attached to every message for the current mixpanel client.
        /// If some of the properties are not valid mixpanel properties they will be ignored. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for valid property types.
        /// </param>
        public MixpanelClient(string token, MixpanelConfig config = null, object superProperties = null)
        {
            if (String.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token");

            _token = token;
            _config = config;

            SetSuperProperties(superProperties);
            
            UtcNow = () => DateTime.UtcNow;
        }

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
        public bool Track(string @event, object properties)
        {
            return Track(@event, null, properties);
        }

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
        public bool Track(string @event, object distinctId, object properties)
        {
            return SendMessage(
                CreateTrackMessageObject(@event, distinctId, properties), EndpointTrack, "Track");
        }

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
        public MixpanelMessageTest TrackTest(string @event, object properties)
        {
            return TrackTest(@event, null, properties);
        }

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
        public MixpanelMessageTest TrackTest(string @event, object distinctId, object properties)
        {
            return TestMessage(() => CreateTrackMessageObject(@event, distinctId, properties));
        }

        private IDictionary<string, object> CreateTrackMessageObject(
            string @event, object distinctId, object properties)
        {
            return GetMessageObject(
                new TrackMessageBuilder(_config), properties,
                new Dictionary<string, object>
                {
                    {MixpanelProperty.Event, @event},
                    {MixpanelProperty.DistinctId, distinctId}
                });
        }

        #endregion

        #region Alias

        /// <summary>
        /// Creates an alias to existing distinct id. 
        /// Message will be sent to 'http://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Original unique user profile identifier to create alias for.</param>
        /// <param name="alias">Alias for original user profile identifier.</param>
        public bool Alias(object distinctId, object alias)
        {
            return SendMessage(CreateAliasMessageObject(distinctId, alias), EndpointTrack, "Alias");
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'Alias' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Original unique user profile identifier to create alias for.</param>
        /// <param name="alias">Alias for original user profile identifier.</param>
        public MixpanelMessageTest AliasTest(object distinctId, object alias)
        {
            return TestMessage(() => CreateAliasMessageObject(distinctId, alias));
        }

        private IDictionary<string, object> CreateAliasMessageObject(
            object distinctId, object alias)
        {
            return GetMessageObject(
                new AliasMessageBuilder(_config), null,
                new Dictionary<string, object>
                {
                    {MixpanelProperty.DistinctId, distinctId},
                    {MixpanelProperty.Alias, alias}
                });
        }

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
        public bool PeopleSet(object properties)
        {
            return PeopleSet(null, properties);
        }

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
        public bool PeopleSet(object distinctId, object properties)
        {
            return SendMessage(CreatePeopleSetMessageObject(distinctId, properties), EndpointEngage, "PeopleSet");
        }

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
        public MixpanelMessageTest PeopleSetTest(object properties)
        {
            return PeopleSetTest(null, properties);
        }

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
        public MixpanelMessageTest PeopleSetTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleSetMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleSetMessageObject(object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleSetMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId));
        }

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
        public bool PeopleSetOnce(object properties)
        {
            return PeopleSetOnce(null, properties);
        }

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
        public bool PeopleSetOnce(object distinctId, object properties)
        {
            return SendMessage(
                CreatePeopleSetOnceMessageObject(distinctId, properties), EndpointEngage, "PeopleSetOnce");
        }

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
        public MixpanelMessageTest PeopleSetOnceTest(object properties)
        {
            return PeopleSetOnceTest(null, properties);
        }

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
        public MixpanelMessageTest PeopleSetOnceTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleSetOnceMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleSetOnceMessageObject(object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleSetOnceMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId));
        }

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
        public bool PeopleAdd(object properties)
        {
            return PeopleAdd(null, properties);
        }

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
        public bool PeopleAdd(object distinctId, object properties)
        {
            return SendMessage(
                CreatePeopleAddMessageObject(distinctId, properties), EndpointEngage, "PeopleAdd");
        }

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
        public MixpanelMessageTest PeopleAddTest(object properties)
        {
            return PeopleAddTest(null, properties);
        }

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
        public MixpanelMessageTest PeopleAddTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleAddMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleAddMessageObject(
            object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleAddMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId),
                MessagePropetyRules.NumericsOnly);
        }

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
        public bool PeopleAppend(object properties)
        {
            return PeopleAppend(null, properties);
        }

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
        public bool PeopleAppend(object distinctId, object properties)
        {
            return SendMessage(
                CreatePeopleAppendMessageObject(distinctId, properties), EndpointEngage, "PeopleAppend");
        }

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
        public MixpanelMessageTest PeopleAppendTest(object properties)
        {
            return PeopleAppendTest(null, properties);
        }

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
        public MixpanelMessageTest PeopleAppendTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleAppendMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleAppendMessageObject(
            object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleAppendMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId));
        }

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
        public bool PeopleUnion(object properties)
        {
            return PeopleUnion(null, properties);
        }

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
        public bool PeopleUnion(object distinctId, object properties)
        {
            return SendMessage(
                CreatePeopleUnionMessageObject(distinctId, properties), EndpointEngage, "PeopleUnion");
        }

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
        public MixpanelMessageTest PeopleUnionTest(object properties)
        {
            return PeopleUnionTest(null, properties);
        }

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
        public MixpanelMessageTest PeopleUnionTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleUnionMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleUnionMessageObject(
            object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleUnionMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId),
                MessagePropetyRules.ListsOnly);
        }

        #endregion PeopleUnion

        #region PeopleUnset

        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        public bool PeopleUnset(IEnumerable<string> propertyNames)
        {
            return PeopleUnset(null, propertyNames);
        }

        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        public bool PeopleUnset(object distinctId, IEnumerable<string> propertyNames)
        {
            return SendMessage(
                CreatePeopleUnsetMessageObject(distinctId, propertyNames), EndpointEngage, "PeopleUnset");
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleUnset' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        public MixpanelMessageTest PeopleUnsetTest(IEnumerable<string> propertyNames)
        {
            return PeopleUnsetTest(null, propertyNames);
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleUnset' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">User unique identifier. Will be converted to string.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        public MixpanelMessageTest PeopleUnsetTest(object distinctId, IEnumerable<string> propertyNames)
        {
            return TestMessage(() => CreatePeopleUnsetMessageObject(distinctId, propertyNames));
        }

        private IDictionary<string, object> CreatePeopleUnsetMessageObject(object distinctId, IEnumerable<string> propertyNames)
        {
            return GetMessageObject(
                new PeopleUnsetMessageBuilder(_config),
                null, new Dictionary<string, object>
                {
                    {MixpanelProperty.DistinctId, distinctId},
                    {MixpanelProperty.PeopleUnset, propertyNames},
                });
        }

        #endregion PeopleUnset

        #region PeopleDelete

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties.
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        public bool PeopleDelete(object distinctId)
        {
            return SendMessage(CreatePeopleDeleteObject(distinctId), EndpointEngage, "PeopleDelete");
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleDelete' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        public MixpanelMessageTest PeopleDeleteTest(object distinctId)
        {
            return TestMessage(() => CreatePeopleDeleteObject(distinctId));
        }

        private IDictionary<string, object> CreatePeopleDeleteObject(object distinctId)
        {
            return GetMessageObject(
                new PeopleDeleteMessageBuilder(_config),
                null, CreateExtraPropertiesForDistinctId(distinctId));
        }

        #endregion PeopleDelete

        #region PeopleTrackCharge

        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        public bool PeopleTrackCharge(object distinctId, decimal amount)
        {
            return PeopleTrackCharge(distinctId, amount, UtcNow());
        }

        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        public bool PeopleTrackCharge(object distinctId, decimal amount, DateTime time)
        {
            return SendMessage(
                CreatePeopleTrackChargeMessageObject(distinctId, amount, time),
                EndpointEngage, "PeopleTrackCharge");
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleTrackCharge' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        public MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount)
        {
            return PeopleTrackChargeTest(distinctId, amount, UtcNow());
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleTrackCharge' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// The message will NOT be sent to Mixpanel.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        public MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount, DateTime time)
        {
            return TestMessage(() => CreatePeopleTrackChargeMessageObject(distinctId, amount, time));
        }

        private IDictionary<string, object> CreatePeopleTrackChargeMessageObject(
            object distinctId, decimal amount, DateTime time)
        {
            return GetMessageObject(
                new PeopleTrackChargeMessageBuilder(),
                null, new Dictionary<string, object>
                {
                    {MixpanelProperty.DistinctId, distinctId},
                    {MixpanelProperty.Time, time},
                    {MixpanelProperty.PeopleAmount, amount},
                });
        }

        #endregion

        #region Super properties

        private object _superProperties;

        /// <summary>
        /// Sets super properties that will be attached to every message for the current mixpanel client.
        /// All previosly set super properties will be removed.
        /// </summary>
        /// <param name="superProperties">
        /// Object with super properties to set.
        /// If some of the properties are not valid mixpanel properties they will be ignored. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for valid property types.
        /// </param>
        public void SetSuperProperties(object superProperties)
        {
            _superProperties = superProperties;
        }

        #endregion Super properties

        /// <summary>
        /// Returns dictionary that contains Mixpanel message and is ready to be serialized. 
        /// </summary>
        /// <param name="builder">
        /// An override of <see cref="MessageBuilderBase"/> to use to generate message data.
        /// </param>
        /// <param name="userProperties">Object that contains user defined properties.</param>
        /// <param name="extraProperties">
        /// Object created by calling method. Usually contains properties that are passed to calling method
        /// as arguments.
        /// </param>
        /// <param name="propetyRules">
        /// Additional rules that will be appended to user defined properties.
        /// </param>
        private IDictionary<string, object> GetMessageObject(
            MessageBuilderBase builder, object userProperties, object extraProperties, 
            MessagePropetyRules propetyRules = MessagePropetyRules.None)
        {
            var od = new MessageData(builder.SpecialPropsBindings, propetyRules, _config);
            od.ParseAndSetProperties(userProperties);
            od.SetProperty(MixpanelProperty.Token, _token);
            od.ParseAndSetPropertiesIfNotNull(extraProperties);
            od.ParseAndSetProperties(_superProperties);

            return builder.GetMessageObject(od);
        }

        private IDictionary<string, object> CreateExtraPropertiesForDistinctId(object distinctId)
        {
            return new Dictionary<string, object> { { MixpanelProperty.DistinctId, distinctId } };
        }

        private string ToJson(object obj)
        {
            return ConfigHelper.GetSerializeJsonFn(_config)(obj);
        }

        private string ToBase64(string json)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private string GetFormData(IDictionary<string, object> obj)
        {
            return "data=" + ToBase64(ToJson(obj));
        }

        private bool SendMessage(IDictionary<string, object> obj, string endpoint, string messageType)
        {
            string url, formData;
            try
            {
                url = string.Format(UrlFormat, endpoint);
                formData = GetFormData(obj);
            }
            catch (Exception e)
            {
                LogError(string.Format("Error creating '{0}' object.", messageType), e);
                return false;
            }

            try
            {
                return ConfigHelper.GetHttpPostFn(_config)(url, formData);
            }
            catch (Exception e)
            {
                LogError(string.Format("POST fails to '{0}' with data '{1}'", url, formData), e);
                return false;
            }
        }

        private MixpanelMessageTest TestMessage(Func<IDictionary<string, object>> getMessageDataFn)
        {
            var res = new MixpanelMessageTest();

            try
            {
                res.Data = getMessageDataFn();
            }
            catch (Exception e)
            {
                res.Exception = e;
                return res;
            }

            try
            {
                res.Json = ToJson(res.Data);
            }
            catch (Exception e)
            {
                res.Exception = e;
                return res;
            }

            try
            {
                res.Base64 = ToBase64(res.Json);
            }
            catch (Exception e)
            {
                res.Exception = e;
                return res;
            }

            return res;
        }

        private void LogError(string msg, Exception exception)
        {
            var logFn = ConfigHelper.GetErrorLogFn(_config);
            if (logFn != null)
            {
                logFn(msg, exception);
            }
        }
    }
}