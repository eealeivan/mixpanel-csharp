using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if !(NET40 || NET35)
using System.Threading.Tasks;
#endif
using Mixpanel.Core.Message;
using Mixpanel.Misc;

namespace Mixpanel
{
    public sealed partial class MixpanelClient : IMixpanelClient
    {
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
            :this(config, superProperties)
        {
            _token = token;
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelClient"/>. This constructor is isually used
        /// when you want to call only 'Send' and 'SendAsync' methods, because in this case
        /// token is already specified in each <see cref="MixpanelMessage"/>.
        /// </summary>
        public MixpanelClient(MixpanelConfig config = null, object superProperties = null)
        {
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
            return SendMessageInternal(
                () => CreateTrackMessageObject(@event, distinctId, properties),
                EndpointTrack,
                MessageKind.Track);
        }

#if !(NET40 || NET35)
        /// <summary>
        /// Adds an event to Mixpanel by sending a message to 'http://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public async Task<bool> TrackAsync(string @event, object properties)
        {
            return await TrackAsync(@event, null, properties);
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
        public async Task<bool> TrackAsync(string @event, object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreateTrackMessageObject(@event, distinctId, properties),
                EndpointTrack,
                MessageKind.Track);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'Track' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned. 
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetTrackMessage(string @event, object properties)
        {
            return GetTrackMessage(@event, null, properties);
        }

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'Track' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="event">Name of the event.</param>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetTrackMessage(string @event, object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.Track,
                () => CreateTrackMessageObject(@event, distinctId, properties));
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
                new TrackMessageBuilder(), properties,
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
            return SendMessageInternal(
                () => CreateAliasMessageObject(distinctId, alias),
                EndpointTrack,
                MessageKind.Alias);
        }

#if !(NET40 || NET35)
        /// <summary>
        /// Creates an alias to existing distinct id. 
        /// Message will be sent to 'http://api.mixpanel.com/track/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Original unique user profile identifier to create alias for.</param>
        /// <param name="alias">Alias for original user profile identifier.</param>
        public async Task<bool> AliasAsync(object distinctId, object alias)
        {
            return await SendMessageInternalAsync(
                () => CreateAliasMessageObject(distinctId, alias),
                EndpointTrack,
                MessageKind.Alias);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'Alias'. 
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Original unique user profile identifier to create alias for.</param>
        /// <param name="alias">Alias for original user profile identifier.</param>
        public MixpanelMessage GetAliasMessage(object distinctId, object alias)
        {
            return GetMessage(
                MessageKind.Alias,
                () => CreateAliasMessageObject(distinctId, alias));
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
                new AliasMessageBuilder(), null,
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
            return SendMessageInternal(
                () => CreatePeopleSetMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleSet);
        }

#if !(NET40 || NET35)
        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile. If profile doesn't exists, then new profile
        /// will be created. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public async Task<bool> PeopleSetAsync(object properties)
        {
            return await PeopleSetAsync(null, properties);
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
        public async Task<bool> PeopleSetAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleSetMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleSet);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleSet' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleSetMessage(object properties)
        {
            return GetPeopleSetMessage(null, properties);
        }

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleSet' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleSetMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleSet,
                () => CreatePeopleSetMessageObject(distinctId, properties));
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
                new PeopleSetMessageBuilder(),
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
            return SendMessageInternal(
                () => CreatePeopleSetOnceMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleSetOnce);
        }

#if !(NET40 || NET35)
        /// <summary>
        /// Sets <paramref name="properties"></paramref> for profile without overwriting existing values. 
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public async Task<bool> PeopleSetOnceAsync(object properties)
        {
            return await PeopleSetOnceAsync(null, properties);
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
        public async Task<bool> PeopleSetOnceAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleSetOnceMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleSetOnce);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleSetOnce' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleSetOnceMessage(object properties)
        {
            return GetPeopleSetOnceMessage(null, properties);
        }

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleSetOnce' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleSetOnceMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleSetOnce,
                () => CreatePeopleSetOnceMessageObject(distinctId, properties));
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
                new PeopleSetOnceMessageBuilder(),
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
            return SendMessageInternal(
                () => CreatePeopleAddMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleAdd);
        }

#if !(NET40 || NET35)
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
        public async Task<bool> PeopleAddAsync(object properties)
        {
            return await PeopleAddAsync(null, properties);
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
        public async Task<bool> PeopleAddAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleAddMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleAdd);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleAdd' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleAddMessage(object properties)
        {
            return GetPeopleAddMessage(null, properties);
        }

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleAdd' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties"> 
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleAddMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleAdd,
                () => CreatePeopleAddMessageObject(distinctId, properties));
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
                new PeopleAddMessageBuilder(),
                properties, CreateExtraPropertiesForDistinctId(distinctId));
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
            return SendMessageInternal(
                () => CreatePeopleAppendMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleAppend);
        }

#if !(NET40 || NET35)
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
        public async Task<bool> PeopleAppendAsync(object properties)
        {
            return await PeopleAppendAsync(null, properties);
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
        public async Task<bool> PeopleAppendAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleAppendMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleAppend);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleAppend' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleAppendMessage(object properties)
        {
            return GetPeopleAppendMessage(null, properties);
        }

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleAppend' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed. Check documentation
        /// on project page 'https://github.com/eealeivan/mixpanel-csharp' for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleAppendMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleAppend,
                () => CreatePeopleAppendMessageObject(distinctId, properties));
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
                new PeopleAppendMessageBuilder(),
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
            return SendMessageInternal(
                () => CreatePeopleUnionMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleUnion);
        }

#if !(NET40 || NET35)
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
        public async Task<bool> PeopleUnionAsync(object properties)
        {
            return await PeopleUnionAsync(null, properties);
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
        public async Task<bool> PeopleUnionAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleUnionMessageObject(distinctId, properties),
                EndpointEngage,
                MessageKind.PeopleUnion);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleUnion' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="properties">
        ///  Object containg keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleUnionMessage(object properties)
        {
            return GetPeopleUnionMessage(null, properties);
        }

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleUnion' that contains parsed data from 
        /// <paramref name="properties"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        ///  Object containg keys and values that will be parsed and sent to Mixpanel. All non collection 
        ///  properties except '$distinct_id' will be ignored. Check documentation  on project page 
        ///  https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        public MixpanelMessage GetPeopleUnionMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleUnion,
                () => CreatePeopleUnionMessageObject(distinctId, properties));
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
                new PeopleUnionMessageBuilder(),
                properties, CreateExtraPropertiesForDistinctId(distinctId));
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
            return SendMessageInternal(
                () => CreatePeopleUnsetMessageObject(distinctId, propertyNames),
                EndpointEngage,
                MessageKind.PeopleUnset);
        }

#if !(NET40 || NET35)
        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        public async Task<bool> PeopleUnsetAsync(IEnumerable<string> propertyNames)
        {
            return await PeopleUnsetAsync(null, propertyNames);
        }

        /// <summary>
        /// Properties with names containing in <paramref name="propertyNames"/> will be permanently
        /// removed. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        public async Task<bool> PeopleUnsetAsync(object distinctId, IEnumerable<string> propertyNames)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleUnsetMessageObject(distinctId, propertyNames),
                EndpointEngage,
                MessageKind.PeopleUnset);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleUnset' that contains parsed data from 
        /// <paramref name="propertyNames"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        public MixpanelMessage GetPeopleUnsetMessage(IEnumerable<string> propertyNames)
        {
            return GetPeopleUnsetMessage(null, propertyNames);
        }

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleUnset' that contains parsed data from 
        /// <paramref name="propertyNames"/> parameter. If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">User unique identifier. Will be converted to string.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        public MixpanelMessage GetPeopleUnsetMessage(object distinctId, IEnumerable<string> propertyNames)
        {
            return GetMessage(
                MessageKind.PeopleUnset,
                () => CreatePeopleUnsetMessageObject(distinctId, propertyNames));
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

        private IDictionary<string, object> CreatePeopleUnsetMessageObject(object distinctId,
            IEnumerable<string> propertyNames)
        {
            return GetMessageObject(
                new PeopleUnsetMessageBuilder(),
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
            return SendMessageInternal(
                () => CreatePeopleDeleteObject(distinctId),
                EndpointEngage,
                MessageKind.PeopleDelete);
        }

#if !(NET40 || NET35)
        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties.
        /// Sends a message to 'http://api.mixpanel.com/engage/' endpoint. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        public async Task<bool> PeopleDeleteAsync(object distinctId)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleDeleteObject(distinctId),
                EndpointEngage,
                MessageKind.PeopleDelete);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleDelete'. 
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        public MixpanelMessage GetPeopleDeleteMessage(object distinctId)
        {
            return GetMessage(
                MessageKind.PeopleDelete,
                () => CreatePeopleDeleteObject(distinctId));
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
                new PeopleDeleteMessageBuilder(),
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
            return SendMessageInternal(
                () => CreatePeopleTrackChargeMessageObject(distinctId, amount, time),
                EndpointEngage,
                MessageKind.PeopleTrackCharge);
        }

#if !(NET40 || NET35)
        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        public async Task<bool> PeopleTrackChargeAsync(object distinctId, decimal amount)
        {
            return await PeopleTrackChargeAsync(distinctId, amount, UtcNow());
        }

        /// <summary>
        /// Adds new transaction to profile. Sends a message to 'http://api.mixpanel.com/engage/' endpoint.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        public async Task<bool> PeopleTrackChargeAsync(object distinctId, decimal amount, DateTime time)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleTrackChargeMessageObject(distinctId, amount, time),
                EndpointEngage,
                MessageKind.PeopleTrackCharge);
        }
#endif

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleTrackCharge'. 
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        public MixpanelMessage GetPeopleTrackChargeMessage(object distinctId, decimal amount)
        {
            return GetPeopleTrackChargeMessage(distinctId, amount, UtcNow());
        }

        /// <summary>
        /// Returns a <see cref="MixpanelMessage"/> for 'PeopleTrackCharge'. 
        /// If message can't be created, then null is returned.
        /// No data will be sent to Mixpanel.
        /// You can send returned message using <see cref="IMixpanelClient.Send(Mixpanel.MixpanelMessage[])"/> method.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="amount">Amount of the transaction.</param>
        /// <param name="time">The date transaction was done.</param>
        public MixpanelMessage GetPeopleTrackChargeMessage(object distinctId, decimal amount, DateTime time)
        {
            return GetMessage(
                MessageKind.PeopleTrackCharge,
                () => CreatePeopleTrackChargeMessageObject(distinctId, amount, time));
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

        #region Send

        public SendResult Send(IEnumerable<MixpanelMessage> messages)
        {
            var result = new SendResult { Success = true };
            var batchMessage = new BatchMessageWrapper(messages);

            List<List<MixpanelMessage>> batchTrackMessages = batchMessage.TrackMessages;
            if (batchTrackMessages != null)
            {
                foreach (var trackMessages in batchTrackMessages)
                {
                    var msgs = trackMessages;
                    bool success = SendMessageInternal(
                        () => GetBatchMessageData(msgs), EndpointTrack, MessageKind.Batch);
                    UpdateResult(success, msgs, result);
                }
            }

            List<List<MixpanelMessage>> batchEngageMessages = batchMessage.EngageMessages;
            if (batchEngageMessages != null)
            {
                foreach (var engageMessages in batchEngageMessages)
                {
                    var msgs = engageMessages;
                    bool success = SendMessageInternal(
                        () => GetBatchMessageData(msgs), EndpointEngage, MessageKind.Batch);
                    UpdateResult(success, msgs, result);
                }
            }

            return result;
        }

        public SendResult Send(params MixpanelMessage[] messages)
        {
            return Send(messages as IEnumerable<MixpanelMessage>);
        }

#if !(NET40 || NET35)
        public async Task<SendResult> SendAsync(IEnumerable<MixpanelMessage> messages)
        {
            var result = new SendResult { Success = true };
            var batchMessage = new BatchMessageWrapper(messages);

            List<List<MixpanelMessage>> batchTrackMessages = batchMessage.TrackMessages;
            if (batchTrackMessages != null)
            {
                foreach (var trackMessages in batchTrackMessages)
                {
                    var msgs = trackMessages;
                    bool success = await SendMessageInternalAsync(
                        () => GetBatchMessageData(msgs), EndpointTrack, MessageKind.Batch);
                    UpdateResult(success, msgs, result);
                }
            }

            List<List<MixpanelMessage>> batchEngageMessages = batchMessage.EngageMessages;
            if (batchEngageMessages != null)
            {
                foreach (var engageMessages in batchEngageMessages)
                {
                    var msgs = engageMessages;
                    bool success = await SendMessageInternalAsync(
                        () => GetBatchMessageData(msgs), EndpointEngage, MessageKind.Batch);
                    UpdateResult(success, msgs, result);
                }
            }

            return result;
        }

        public async Task<SendResult> SendAsync(params MixpanelMessage[] messages)
        {
            return await SendAsync(messages as IEnumerable<MixpanelMessage>);
        }
#endif

        public IEnumerable<MixpanelBatchMessageTest> SendTest(IEnumerable<MixpanelMessage> messages)
        {
            var batchMessageWrapper = new BatchMessageWrapper(messages);

            // Concatenate both 'TrackMessages' and 'EngageMessages' in one list
            var batchMessages =
                (batchMessageWrapper.TrackMessages ?? new List<List<MixpanelMessage>>(0))
                .Concat(((batchMessageWrapper.EngageMessages ?? new List<List<MixpanelMessage>>(0))));

            foreach (var batchMessage in batchMessages)
            {
                var testMessage = new MixpanelBatchMessageTest { Data = GetBatchMessageData(batchMessage) };

                try
                {
                    testMessage.Json = ToJson(testMessage);
                    testMessage.Base64 = ToBase64(testMessage.Json);
                }
                catch (Exception e)
                {
                    testMessage.Exception = e;
                }

                yield return testMessage;
            }
        }

        public IEnumerable<MixpanelBatchMessageTest> SendTest(params MixpanelMessage[] messages)
        {
            return SendTest(messages as IEnumerable<MixpanelMessage>);
        }

        private IList<IDictionary<string, object>> GetBatchMessageData(IList<MixpanelMessage> messages)
        {
            Debug.Assert(messages != null);

            return messages
                .Select(msg => msg.Data)
                .ToList();
        }

        private void UpdateResult(bool success, List<MixpanelMessage> mixpanelMessages, SendResult result)
        {
            result.Success &= success;
            if (success)
            {
                if (result.SentBatches == null)
                {
                    result.SentBatches = new List<IList<MixpanelMessage>>();
                }

                result.SentBatches.Add(mixpanelMessages);
            }
            else
            {
                if (result.FailedBatches == null)
                {
                    result.FailedBatches = new List<IList<MixpanelMessage>>();
                }

                result.FailedBatches.Add(mixpanelMessages);
            }
        }

        #endregion Send

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
        private IDictionary<string, object> GetMessageObject(
            MessageBuilderBase builder, object userProperties, object extraProperties)
        {
            var md = new MessageData(
                builder.SpecialPropsBindings,
                builder.DistinctIdPropsBindings,
                builder.MessagePropetiesRules,
                builder.SuperPropertiesRules,
                _config);
            md.SetProperty(MixpanelProperty.Token, _token);
            md.ParseAndSetProperties(userProperties);
            md.ParseAndSetPropertiesIfNotNull(extraProperties);
            md.ParseAndSetSuperProperties(_superProperties);

            return builder.GetMessageObject(md);
        }

        private IDictionary<string, object> CreateExtraPropertiesForDistinctId(object distinctId)
        {
            return new Dictionary<string, object> { { MixpanelProperty.DistinctId, distinctId } };
        }

        private MixpanelMessage GetMessage(
            MessageKind messageKind, Func<IDictionary<string, object>> getMessageDataFn)
        {
            try
            {
                return new MixpanelMessage
                {
                    Kind = messageKind,
                    Data = getMessageDataFn()
                };
            }
            catch (Exception e)
            {
                LogError(string.Format("Error creating '{0}' message.", messageKind), e);
                return null;
            }
        }

        private MixpanelMessageTest TestMessage(Func<IDictionary<string, object>> getMessageDataFn)
        {
            var testMessage = new MixpanelMessageTest();

            try
            {
                testMessage.Data = getMessageDataFn();
                testMessage.Json = ToJson(testMessage.Data);
                testMessage.Base64 = ToBase64(testMessage.Json);
            }
            catch (Exception e)
            {
                testMessage.Exception = e;
                return testMessage;
            }

            return testMessage;
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