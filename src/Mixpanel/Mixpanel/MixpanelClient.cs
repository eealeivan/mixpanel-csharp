using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Mixpanel.Extensibility;
using Mixpanel.MessageBuilders;
using Mixpanel.MessageBuilders.People;
using Mixpanel.MessageBuilders.Track;
using Mixpanel.MessageProperties;

namespace Mixpanel
{
    /// <inheritdoc/>
    public sealed partial class MixpanelClient : IMixpanelClient
    {
        private readonly string token;
        private readonly MixpanelConfig config;

        /// <summary>
        /// Func for getting/setting current utc date. Simplifies testing.
        /// </summary>
        internal Func<DateTime> UtcNow { get; set; }

        /// <summary>
        /// Parsed super properties which are added to every message.
        /// Collection is only initialized in constructor and never changed, so it's safe
        /// to be iterated by multiple threads.
        /// </summary>
        private readonly IList<ObjectProperty> superProperties;
        
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
            : this(config, superProperties)
        {
            this.token = token;
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelClient"/>. This constructor is usually used
        /// when you want to call only 'Send' and 'SendAsync' methods, because in this case
        /// token is already specified in each <see cref="MixpanelMessage"/>.
        /// </summary>
        /// <param name="config">
        /// Configuration for this particular client. Set properties from this class will override global properties.
        /// </param>
        /// <param name="superProperties">
        /// Object with properties that will be attached to every message for the current mixpanel client.
        /// If some of the properties are not valid mixpanel properties they will be ignored. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for valid property types.
        /// </param>
        public MixpanelClient(MixpanelConfig config = null, object superProperties = null)
        {
            this.config = config;
            UtcNow = () => DateTime.UtcNow;

            // Parse super properties only one time
            this.superProperties = PropertiesDigger.Get(superProperties, PropertyOrigin.SuperProperty).ToList();
        }

        #region Track

        /// <inheritdoc/>
        public bool Track(string @event, object properties)
        {
            return Track(@event, null, properties);
        }

        /// <inheritdoc/>
        public bool Track(string @event, object distinctId, object properties)
        {
            return SendMessageInternal(
                () => CreateTrackMessageObject(@event, distinctId, properties),
                MixpanelMessageEndpoint.Track,
                MessageKind.Track);
        }

        /// <inheritdoc/>
        public async Task<bool> TrackAsync(string @event, object properties)
        {
            return await TrackAsync(@event, null, properties).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> TrackAsync(string @event, object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreateTrackMessageObject(@event, distinctId, properties),
                MixpanelMessageEndpoint.Track,
                MessageKind.Track).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetTrackMessage(string @event, object properties)
        {
            return GetTrackMessage(@event, null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetTrackMessage(string @event, object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.Track,
                () => CreateTrackMessageObject(@event, distinctId, properties));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest TrackTest(string @event, object properties)
        {
            return TrackTest(@event, null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest TrackTest(string @event, object distinctId, object properties)
        {
            return TestMessage(() => CreateTrackMessageObject(@event, distinctId, properties));
        }

        private IDictionary<string, object> CreateTrackMessageObject(
            string @event, object distinctId, object properties)
        {
            MessageBuildResult result = TrackMessageBuilder.Build(
                token, @event, superProperties, properties, distinctId, config);
            return GetMessageObject(result);
        }

        #endregion

        #region Alias

        /// <inheritdoc/>
        public bool Alias(object alias)
        {
            return Alias(null, alias);
        }

        /// <inheritdoc/>
        public bool Alias(object distinctId, object alias)
        {
            return SendMessageInternal(
                () => CreateAliasMessageObject(distinctId, alias),
                MixpanelMessageEndpoint.Track,
                MessageKind.Alias);
        }

        /// <inheritdoc/>
        public async Task<bool> AliasAsync(object alias)
        {
            return await AliasAsync(null, alias).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> AliasAsync(object distinctId, object alias)
        {
            return await SendMessageInternalAsync(
                () => CreateAliasMessageObject(distinctId, alias),
                MixpanelMessageEndpoint.Track,
                MessageKind.Alias).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetAliasMessage(object distinctId, object alias)
        {
            return GetMessage(
                MessageKind.Alias,
                () => CreateAliasMessageObject(distinctId, alias));
        }

        /// <inheritdoc/>
        public MixpanelMessage GetAliasMessage(object alias)
        {
            return GetMessage(
                MessageKind.Alias,
                () => CreateAliasMessageObject(null, alias));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest AliasTest(object distinctId, object alias)
        {
            return TestMessage(() => CreateAliasMessageObject(distinctId, alias));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest AliasTest(object alias)
        {
            return TestMessage(() => CreateAliasMessageObject(null, alias));
        }

        private IDictionary<string, object> CreateAliasMessageObject(object distinctId, object alias)
        {
            MessageBuildResult result = AliasMessageBuilder.Build(token, superProperties, distinctId, alias);
            return GetMessageObject(result);
        }

        #endregion Alias

        #region PeopleSet

        /// <inheritdoc/>
        public bool PeopleSet(object properties)
        {
            return PeopleSet(null, properties);
        }

        /// <inheritdoc/>
        public bool PeopleSet(object distinctId, object properties)
        {
            return SendMessageInternal(
                () => CreatePeopleSetMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleSet);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleSetAsync(object properties)
        {
            return await PeopleSetAsync(null, properties).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleSetAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleSetMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleSet).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleSetMessage(object properties)
        {
            return GetPeopleSetMessage(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleSetMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleSet,
                () => CreatePeopleSetMessageObject(distinctId, properties));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleSetTest(object properties)
        {
            return PeopleSetTest(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleSetTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleSetMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleSetMessageObject(object distinctId, object properties)
        {
            MessageBuildResult result = PeopleSetMessageBuilder.BuildSet(token, superProperties, properties, distinctId, config);
            return GetMessageObject(result);
        }

        #endregion PeopleSet

        #region PeopleSetOnce

        /// <inheritdoc/>
        public bool PeopleSetOnce(object properties)
        {
            return PeopleSetOnce(null, properties);
        }

        /// <inheritdoc/>
        public bool PeopleSetOnce(object distinctId, object properties)
        {
            return SendMessageInternal(
                () => CreatePeopleSetOnceMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleSetOnce);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleSetOnceAsync(object properties)
        {
            return await PeopleSetOnceAsync(null, properties).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleSetOnceAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleSetOnceMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleSetOnce).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleSetOnceMessage(object properties)
        {
            return GetPeopleSetOnceMessage(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleSetOnceMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleSetOnce,
                () => CreatePeopleSetOnceMessageObject(distinctId, properties));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleSetOnceTest(object properties)
        {
            return PeopleSetOnceTest(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleSetOnceTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleSetOnceMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleSetOnceMessageObject(object distinctId, object properties)
        {
            MessageBuildResult result = 
                PeopleSetMessageBuilder.BuildSetOnce(token, superProperties, properties, distinctId, config);
            return GetMessageObject(result);
        }

        #endregion PeopleSetOnce

        #region PeopleAdd

        /// <inheritdoc/>
        public bool PeopleAdd(object properties)
        {
            return PeopleAdd(null, properties);
        }

        /// <inheritdoc/>
        public bool PeopleAdd(object distinctId, object properties)
        {
            return SendMessageInternal(
                () => CreatePeopleAddMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleAdd);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleAddAsync(object properties)
        {
            return await PeopleAddAsync(null, properties).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleAddAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleAddMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleAdd).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleAddMessage(object properties)
        {
            return GetPeopleAddMessage(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleAddMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleAdd,
                () => CreatePeopleAddMessageObject(distinctId, properties));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleAddTest(object properties)
        {
            return PeopleAddTest(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleAddTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleAddMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleAddMessageObject(
            object distinctId, object properties)
        {
            MessageBuildResult result = 
                PeopleAddMessageBuilder.Build(token, superProperties, properties, distinctId, config);
            return GetMessageObject(result);
        }

        #endregion PeopleAdd

        #region PeopleAppend

        /// <inheritdoc/>
        public bool PeopleAppend(object properties)
        {
            return PeopleAppend(null, properties);
        }

        /// <inheritdoc/>
        public bool PeopleAppend(object distinctId, object properties)
        {
            return SendMessageInternal(
                () => CreatePeopleAppendMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleAppend);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleAppendAsync(object properties)
        {
            return await PeopleAppendAsync(null, properties).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleAppendAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleAppendMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleAppend).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleAppendMessage(object properties)
        {
            return GetPeopleAppendMessage(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleAppendMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleAppend,
                () => CreatePeopleAppendMessageObject(distinctId, properties));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleAppendTest(object properties)
        {
            return PeopleAppendTest(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleAppendTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleAppendMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleAppendMessageObject(
            object distinctId, object properties)
        {
            MessageBuildResult result =
                PeopleAppendMessageBuilder.Build(token, superProperties, properties, distinctId, config);
            return GetMessageObject(result);
        }

        #endregion PeopleAppend

        #region PeopleUnion

        /// <inheritdoc/>
        public bool PeopleUnion(object properties)
        {
            return PeopleUnion(null, properties);
        }

        /// <inheritdoc/>
        public bool PeopleUnion(object distinctId, object properties)
        {
            return SendMessageInternal(
                () => CreatePeopleUnionMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleUnion);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleUnionAsync(object properties)
        {
            return await PeopleUnionAsync(null, properties).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleUnionAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleUnionMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleUnion).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleUnionMessage(object properties)
        {
            return GetPeopleUnionMessage(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleUnionMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleUnion,
                () => CreatePeopleUnionMessageObject(distinctId, properties));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleUnionTest(object properties)
        {
            return PeopleUnionTest(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleUnionTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleUnionMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleUnionMessageObject(
            object distinctId, object properties)
        {
            MessageBuildResult result =
                PeopleUnionMessageBuilder.Build(token, superProperties, properties, distinctId, config);
            return GetMessageObject(result);
        }

        #endregion PeopleUnion

        #region PeopleRemove

        /// <inheritdoc/>
        public bool PeopleRemove(object properties)
        {
            return PeopleRemove(null, properties);
        }

        /// <inheritdoc/>
        public bool PeopleRemove(object distinctId, object properties)
        {
            return SendMessageInternal(
                () => CreatePeopleRemoveMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleRemove);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleRemoveAsync(object properties)
        {
            return await PeopleRemoveAsync(null, properties).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleRemoveAsync(object distinctId, object properties)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleRemoveMessageObject(distinctId, properties),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleRemove).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleRemoveMessage(object properties)
        {
            return GetPeopleRemoveMessage(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleRemoveMessage(object distinctId, object properties)
        {
            return GetMessage(
                MessageKind.PeopleRemove,
                () => CreatePeopleRemoveMessageObject(distinctId, properties));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleRemoveTest(object properties)
        {
            return PeopleRemoveTest(null, properties);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleRemoveTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleRemoveMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleRemoveMessageObject(
            object distinctId, object properties)
        {
            MessageBuildResult result =
                PeopleRemoveMessageBuilder.Build(token, superProperties, properties, distinctId, config);
            return GetMessageObject(result);
        }

        #endregion PeopleRemove

        #region PeopleUnset

        /// <inheritdoc/>
        public bool PeopleUnset(IEnumerable<string> propertyNames)
        {
            return PeopleUnset(null, propertyNames);
        }

        /// <inheritdoc/>
        public bool PeopleUnset(object distinctId, IEnumerable<string> propertyNames)
        {
            return SendMessageInternal(
                () => CreatePeopleUnsetMessageObject(distinctId, propertyNames),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleUnset);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleUnsetAsync(IEnumerable<string> propertyNames)
        {
            return await PeopleUnsetAsync(null, propertyNames).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleUnsetAsync(object distinctId, IEnumerable<string> propertyNames)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleUnsetMessageObject(distinctId, propertyNames),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleUnset).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleUnsetMessage(IEnumerable<string> propertyNames)
        {
            return GetPeopleUnsetMessage(null, propertyNames);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleUnsetMessage(object distinctId, IEnumerable<string> propertyNames)
        {
            return GetMessage(
                MessageKind.PeopleUnset,
                () => CreatePeopleUnsetMessageObject(distinctId, propertyNames));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleUnsetTest(IEnumerable<string> propertyNames)
        {
            return PeopleUnsetTest(null, propertyNames);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleUnsetTest(object distinctId, IEnumerable<string> propertyNames)
        {
            return TestMessage(() => CreatePeopleUnsetMessageObject(distinctId, propertyNames));
        }

        private IDictionary<string, object> CreatePeopleUnsetMessageObject(
            object distinctId,
            IEnumerable<string> propertyNames)
        {
            MessageBuildResult result =
                PeopleUnsetMessageBuilder.Build(token, superProperties, propertyNames, distinctId, config);
            return GetMessageObject(result);
        }

        #endregion PeopleUnset

        #region PeopleDelete

        /// <inheritdoc/>
        public bool PeopleDelete()
        {
            return PeopleDelete(null);
        }

        /// <inheritdoc/>
        public bool PeopleDelete(object distinctId)
        {
            return SendMessageInternal(
                () => CreatePeopleDeleteObject(distinctId),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleDelete);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleDeleteAsync()
        {
            return await PeopleDeleteAsync(null).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleDeleteAsync(object distinctId)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleDeleteObject(distinctId),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleDelete).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleDeleteMessage()
        {
            return GetPeopleDeleteMessage(null);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleDeleteMessage(object distinctId)
        {
            return GetMessage(
                MessageKind.PeopleDelete,
                () => CreatePeopleDeleteObject(distinctId));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleDeleteTest()
        {
            return PeopleDeleteTest(null);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleDeleteTest(object distinctId)
        {
            return TestMessage(() => CreatePeopleDeleteObject(distinctId));
        }

        private IDictionary<string, object> CreatePeopleDeleteObject(object distinctId)
        {
            MessageBuildResult result =
                PeopleDeleteMessageBuilder.Build(token, superProperties, distinctId, config);
            return GetMessageObject(result);
        }

        #endregion PeopleDelete

        #region PeopleTrackCharge

        /// <inheritdoc/>
        public bool PeopleTrackCharge(decimal amount)
        {
            return PeopleTrackCharge(null, amount);
        }

        /// <inheritdoc/>
        public bool PeopleTrackCharge(object distinctId, decimal amount)
        {
            return PeopleTrackCharge(distinctId, amount, UtcNow());
        }

        /// <inheritdoc/>
        public bool PeopleTrackCharge(decimal amount, DateTime time)
        {
            return PeopleTrackCharge(null, amount, time);
        }

        /// <inheritdoc/>
        public bool PeopleTrackCharge(object distinctId, decimal amount, DateTime time)
        {
            return SendMessageInternal(
                () => CreatePeopleTrackChargeMessageObject(distinctId, amount, time),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleTrackCharge);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleTrackChargeAsync(decimal amount)
        {
            return await PeopleTrackChargeAsync(null, amount).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleTrackChargeAsync(object distinctId, decimal amount)
        {
            return await PeopleTrackChargeAsync(distinctId, amount, UtcNow()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleTrackChargeAsync(decimal amount, DateTime time)
        {
            return await PeopleTrackChargeAsync(null, amount, time).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> PeopleTrackChargeAsync(object distinctId, decimal amount, DateTime time)
        {
            return await SendMessageInternalAsync(
                () => CreatePeopleTrackChargeMessageObject(distinctId, amount, time),
                MixpanelMessageEndpoint.Engage,
                MessageKind.PeopleTrackCharge).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleTrackChargeMessage(decimal amount)
        {
            return GetPeopleTrackChargeMessage(null, amount);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleTrackChargeMessage(object distinctId, decimal amount)
        {
            return GetPeopleTrackChargeMessage(distinctId, amount, UtcNow());
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleTrackChargeMessage(decimal amount, DateTime time)
        {
            return GetPeopleTrackChargeMessage(null, amount, time);
        }

        /// <inheritdoc/>
        public MixpanelMessage GetPeopleTrackChargeMessage(object distinctId, decimal amount, DateTime time)
        {
            return GetMessage(
                MessageKind.PeopleTrackCharge,
                () => CreatePeopleTrackChargeMessageObject(distinctId, amount, time));
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleTrackChargeTest(decimal amount)
        {
            return PeopleTrackChargeTest(null, amount);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount)
        {
            return PeopleTrackChargeTest(distinctId, amount, UtcNow());
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleTrackChargeTest(decimal amount, DateTime time)
        {
            return PeopleTrackChargeTest(null, amount, time);
        }

        /// <inheritdoc/>
        public MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount, DateTime time)
        {
            return TestMessage(() => CreatePeopleTrackChargeMessageObject(distinctId, amount, time));
        }

        private IDictionary<string, object> CreatePeopleTrackChargeMessageObject(
            object distinctId, decimal amount, DateTime time)
        {
            MessageBuildResult result =
                PeopleTrackChargeMessageBuilder.Build(
                    token, 
                    superProperties, 
                    amount,
                    time,
                    distinctId, 
                    config);
            return GetMessageObject(result);
        }

        #endregion

        #region Send

        /// <inheritdoc/>
        public SendResult Send(IEnumerable<MixpanelMessage> messages)
        {
            var resultInternal = new SendResultInternal();
            var batchMessage = new BatchMessageWrapper(messages);

            List<List<MixpanelMessage>> batchTrackMessages = batchMessage.TrackMessages;
            if (batchTrackMessages != null)
            {
                foreach (var trackMessages in batchTrackMessages)
                {
                    var msgs = trackMessages;
                    bool success = SendMessageInternal(
                        () => GetBatchMessageData(msgs), MixpanelMessageEndpoint.Track, MessageKind.Batch);
                    resultInternal.Update(success, msgs);
                }
            }

            List<List<MixpanelMessage>> batchEngageMessages = batchMessage.EngageMessages;
            if (batchEngageMessages != null)
            {
                foreach (var engageMessages in batchEngageMessages)
                {
                    var msgs = engageMessages;
                    bool success = SendMessageInternal(
                        () => GetBatchMessageData(msgs), MixpanelMessageEndpoint.Engage, MessageKind.Batch);
                    resultInternal.Update(success, msgs);
                }
            }

            return resultInternal.ToRealSendResult();
        }

        /// <inheritdoc/>
        public SendResult Send(params MixpanelMessage[] messages)
        {
            return Send(messages as IEnumerable<MixpanelMessage>);
        }

        /// <inheritdoc/>
        public async Task<SendResult> SendAsync(IEnumerable<MixpanelMessage> messages)
        {
            var resultInternal = new SendResultInternal();
            var batchMessage = new BatchMessageWrapper(messages);

            List<List<MixpanelMessage>> batchTrackMessages = batchMessage.TrackMessages;
            if (batchTrackMessages != null)
            {
                foreach (var trackMessages in batchTrackMessages)
                {
                    var msgs = trackMessages;
                    bool success = await SendMessageInternalAsync(
                        () => GetBatchMessageData(msgs), MixpanelMessageEndpoint.Track, MessageKind.Batch).ConfigureAwait(false);
                    resultInternal.Update(success, msgs);
                }
            }

            List<List<MixpanelMessage>> batchEngageMessages = batchMessage.EngageMessages;
            if (batchEngageMessages != null)
            {
                foreach (var engageMessages in batchEngageMessages)
                {
                    var msgs = engageMessages;
                    bool success = await SendMessageInternalAsync(
                        () => GetBatchMessageData(msgs), MixpanelMessageEndpoint.Engage, MessageKind.Batch).ConfigureAwait(false);
                    resultInternal.Update(success, msgs);
                }
            }

            return resultInternal.ToRealSendResult();
        }

        /// <inheritdoc/>
        public async Task<SendResult> SendAsync(params MixpanelMessage[] messages)
        {
            return await SendAsync(messages as IEnumerable<MixpanelMessage>).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<MixpanelBatchMessageTest> SendTest(IEnumerable<MixpanelMessage> messages)
        {
            var batchMessageWrapper = new BatchMessageWrapper(messages);

            // Concatenate both 'TrackMessages' and 'EngageMessages' in one list
            var batchMessages =
                (batchMessageWrapper.TrackMessages ?? new List<List<MixpanelMessage>>(0))
                .Concat(((batchMessageWrapper.EngageMessages ?? new List<List<MixpanelMessage>>(0))))
                .ToList();

            var testMessages = new List<MixpanelBatchMessageTest>(batchMessages.Count);

            foreach (var batchMessage in batchMessages)
            {
                var testMessage = new MixpanelBatchMessageTest { Data = GetBatchMessageData(batchMessage) };

                try
                {
                    testMessage.Json = ToJson(testMessage.Data);
                    testMessage.Base64 = ToBase64(testMessage.Json);
                }
                catch (Exception e)
                {
                    testMessage.Exception = e;
                }

                testMessages.Add(testMessage);
            }

            return testMessages.AsReadOnly();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<MixpanelBatchMessageTest> SendTest(params MixpanelMessage[] messages)
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

        private class SendResultInternal
        {
            private bool success;
            private List<List<MixpanelMessage>> sentBatches;
            private List<List<MixpanelMessage>> failedBatches;

            public SendResultInternal()
            {
                success = true;
            }

            public void Update(bool batchSuccess, List<MixpanelMessage> mixpanelMessages)
            {
                success &= batchSuccess;

                if (batchSuccess)
                {
                    if (sentBatches == null)
                    {
                        sentBatches = new List<List<MixpanelMessage>>();
                    }

                    sentBatches.Add(mixpanelMessages);
                }
                else
                {
                    if (failedBatches == null)
                    {
                        failedBatches = new List<List<MixpanelMessage>>();
                    }

                    failedBatches.Add(mixpanelMessages);
                }
            }

            public SendResult ToRealSendResult()
            {
                var result = new SendResult { Success = success };

                if (sentBatches != null)
                {
                    result.SentBatches = sentBatches.Select(x => x.AsReadOnly()).ToList().AsReadOnly();
                }

                if (failedBatches != null)
                {
                    result.FailedBatches = failedBatches.Select(x => x.AsReadOnly()).ToList().AsReadOnly();
                }

                return result;
            }
        }

        #endregion Send

        #region SendJson

        /// <inheritdoc/>
        public bool SendJson(MixpanelMessageEndpoint endpoint, string messageJson)
        {
            return SendMessageInternal(endpoint, messageJson);
        }

        /// <inheritdoc />
        public async Task<bool> SendJsonAsync(MixpanelMessageEndpoint endpoint, string messageJson)
        {
            return await SendMessageInternalAsync(endpoint, messageJson).ConfigureAwait(false);
        }

        #endregion SendJson
        

        private IDictionary<string, object> GetMessageObject(MessageBuildResult messageBuildResult)
        {
            if (!messageBuildResult.Success)
            {
                throw new Exception(messageBuildResult.Error);
            }

            return messageBuildResult.Message;
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
                LogError($"Error creating '{messageKind}' message.", e);
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
            ConfigHelper.GetErrorLogFn(config)?.Invoke(msg, exception);
        }
    }
}
