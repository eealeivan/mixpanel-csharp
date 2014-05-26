using System;
using System.Collections.Generic;
using System.Text;
using Mixpanel.Core;
using Mixpanel.Core.Message;

namespace Mixpanel
{
    public class MixpanelClient : IMixpanelClient
    {
        public const string UrlFormat = "http://api.mixpanel.com/{0}";
        public const string EndpointTrack = "track";
        public const string EndpointEngage = "engage";

        private readonly string _token;
        private readonly MixpanelConfig _config;

        public MixpanelClient(string token, MixpanelConfig config = null, object superProperties = null)
        {
            if (String.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token");

            _token = token;
            _config = config;

            InitializeSuperProperties(superProperties);
        }

        #region Track

        public bool Track(string @event, object properties)
        {
            return Track(@event, null, properties);
        }

        public bool Track(string @event, object distinctId, object properties)
        {
            return SendMessage(
                CreateTrackObject(@event, distinctId, properties), EndpointTrack, "Track");
        }

        public MixpanelMessageTest TrackTest(string @event, object properties)
        {
            return TrackTest(@event, null, properties);
        }

        public MixpanelMessageTest TrackTest(string @event, object distinctId, object properties)
        {
            return TestMessage(() => CreateTrackObject(@event, distinctId, properties));
        }

        private IDictionary<string, object> CreateTrackObject(
            string @event, object distinctId, object properties)
        {
            return GetMessageObject(
                new TrackMessageBuilder(_config), TrackMessageBuilder.SpecialPropsBindings, properties,
                new Dictionary<string, object>
                {
                    {MixpanelProperty.Event, @event},
                    {MixpanelProperty.DistinctId, distinctId}
                });
        }

        #endregion

        #region PeopleSet

        public bool PeopleSet(object properties)
        {
            return PeopleSet(null, properties);
        }

        public bool PeopleSet(object distinctId, object properties)
        {
            return SendMessage(CreatePeopleSetObject(distinctId, properties), EndpointEngage, "PeopleSet");
        }

        public MixpanelMessageTest PeopleSetTest(object properties)
        {
            return PeopleSetTest(null, properties);
        }

        public MixpanelMessageTest PeopleSetTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleSetObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleSetObject(object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleSetMessageBuilder(_config), PeopleSetMessageBuilder.SpecialPropsBindings,
                properties, CreateExtraPropertiesForDistinctId(distinctId));
        }

        #endregion PeopleSet

        #region PeopleSetOnce

        public bool PeopleSetOnce(object properties)
        {
            return PeopleSetOnce(null, properties);
        }

        public bool PeopleSetOnce(object distinctId, object properties)
        {
            return SendMessage(
                CreatePeopleSetOnceObject(distinctId, properties), EndpointEngage, "PeopleSetOnce");
        }

        private IDictionary<string, object> CreatePeopleSetOnceObject(object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleSetOnceMessageBuilder(_config), PeopleMessageBuilderBase.CoreSpecialPropsBindings,
                properties, CreateExtraPropertiesForDistinctId(distinctId));
        }

        #endregion PeopleSetOnce

        #region PeopleAdd

        /// <summary>
        /// Sends data to http://api.mixpanel.com/engage/ using '$add' method.
        /// Returns true if call was successful, false otherwise.
        /// </summary>
        /// <param name="props">
        /// Object containg keys and numerical values. Should also contain 'distinct_id'
        /// (if you can't have this property in the object, then use an overload).
        /// </param>
        public bool PeopleAdd(object props)
        {
            throw new NotImplementedException();
        }

        public bool PeopleAdd(object distinctId, object props)
        {
            throw new NotImplementedException();
        }

        #endregion PeopleAdd

        public bool PeopleAppend(object props)
        {
            throw new NotImplementedException();
        }

        public bool PeopleAppend(object distinctId, object props)
        {
            throw new NotImplementedException();
        }

        public bool PeopleUnion(object props)
        {
            throw new NotImplementedException();
        }

        public bool PeopleUnion(object distinctId, object props)
        {
            throw new NotImplementedException();
        }

        public bool PeopleUnset(IEnumerable<string> props)
        {
            throw new NotImplementedException();
        }

        public bool PeopleUnset(object distinctId, IEnumerable<string> props)
        {
            throw new NotImplementedException();
        }

        #region PeopleDelete

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        public bool PeopleDelete(object distinctId)
        {
            return SendMessage(CreatePeopleDeleteObject(distinctId), EndpointEngage, "PeopleDelete");
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (dictionary, JSON,
        /// base64) of building 'PeopleDelete' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        public MixpanelMessageTest PeopleDeleteTest(object distinctId)
        {
            return TestMessage(() => CreatePeopleDeleteObject(distinctId));
        }

        private IDictionary<string, object> CreatePeopleDeleteObject(object distinctId)
        {
            return GetMessageObject(
                new PeopleDeleteMessageBuilder(_config), PeopleMessageBuilderBase.CoreSpecialPropsBindings,
                null, CreateExtraPropertiesForDistinctId(distinctId));
        }

        #endregion PeopleDelete

        public bool Alias(object distinctId, object alias)
        {
            throw new NotImplementedException();
        }

        public bool TrackCharge(object distinctId, decimal amount)
        {
            throw new NotImplementedException();
        }

        public bool TrackCharge(object distinctId, decimal amount, DateTime time)
        {
            throw new NotImplementedException();
        }

        #region Super properties

        //TODO: Tests. <Aleksandr Ivanov - 23-05-2014>
        //TODO: Add super properties to created objects. <Aleksandr Ivanov - 23-05-2014>
        private MessageData _superProperties;

        private void InitializeSuperProperties(object superProperties)
        {
            if (_superProperties == null)
            {
                _superProperties = new MessageData(null, _config);
            }
            else
            {
                _superProperties.Props.Clear();
            }

            _superProperties.ParseAndSetProperties(superProperties);
        }


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
        public void SetSuperProperties(object superProperties)
        {
            InitializeSuperProperties(superProperties);
        }

        /// <summary>
        /// Sets a super property for the current mixpanel client. If property with given 
        /// <param name="propertyName"></param> alredy exists, the it's value will be rewritten. 
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property. If custom property name formatting was set, the it will be 
        /// applied to this.
        /// </param>
        /// <param name="propertyValue">
        /// The value of the property to set. If an invalid value is provided then super property will 
        /// not be set, and if there is already property with given <param name="propertyName"></param> then
        /// it will be removed. Check documentation on project page https://github.com/eealeivan/mixpanel-csharp
        /// for supported property values.
        /// </param>
        public void SetSuperProperty(string propertyName, object propertyValue)
        {
            //TODO: Remove prop if inavlid value was set. <Aleksandr Ivanov - 23-05-2014>
            _superProperties.SetProperty(propertyName, propertyValue);
        }

        #endregion Super properties

        /// <summary>
        /// Returns dictionary that contains Mixpanel message and i sready to be serialized. 
        /// </summary>
        /// <param name="builder">
        /// An override of <see cref="MessageBuilderBase"/> to use to generate message data.
        /// </param>
        /// <param name="specialPropsBindings">
        /// Bindings for special properties.
        /// </param>
        /// <param name="userProperties">Object that contains user defined properties.</param>
        /// <param name="extraProperties">
        /// Object created by calling method. Usually contains properties that are passed to calling method
        /// as arguments.
        /// </param>
        private IDictionary<string, object> GetMessageObject(
            MessageBuilderBase builder, IDictionary<string, string> specialPropsBindings,
            object userProperties, object extraProperties)
        {
            var od = new MessageData(specialPropsBindings, _config);
            od.ParseAndSetProperties(userProperties);
            od.SetProperty(MixpanelProperty.Token, _token);
            od.ParseAndSetPropertiesIfNotNull(extraProperties);

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