using System;
using System.Collections.Generic;
using System.Text;
using Mixpanel.Core;

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
            if(String.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token");

            _token = token;
            _config = config;

            InitializeSuperProperties(superProperties);
        }

        #region Track

        public bool Track(
            string @event, object props = null, object distinctId = null,
            string ip = null, DateTime? time = null)
        {
            return SendMessage(
                CreateTrackObject(@event, props, distinctId, ip, time), EndpointTrack, "Track");
        }

        public MixpanelTest TrackTest(
            string @event, object props = null, object distinctId = null, 
            string ip = null, DateTime? time = null)
        {
            var res = new MixpanelTest();
            try
            {
                res.Data = CreateTrackObject(@event, props, distinctId, ip, time);
            }
            catch (Exception e)
            {
                res.DataException = e;
                return res;
            }

            try
            {
                res.Json = ToJson(res.Data);
            }
            catch (Exception e)
            {
                res.JsonException = e;
                return res;
            }

            try
            {
                res.Base64 = ToBase64(res.Json);
            }
            catch (Exception e)
            {
                res.Base64Exception = e;
                return res;
            }

            res.Success = true;
            return res;
        }

        private IDictionary<string, object> CreateTrackObject(
            string @event, object props, object distinctId, string ip, DateTime? time)
        {
            var builder = new TrackMessageBuilder(_config);
            var od = new ObjectData(TrackMessageBuilder.SpecialPropsBindings, _config);

            od.ParseAndSetProperties(props);
            od.SetProperty(MixpanelProperty.Event, @event);
            od.SetProperty(MixpanelProperty.Token, _token);
            od.SetPropertyIfNotNull(MixpanelProperty.DistinctId, distinctId);
            od.SetPropertyIfNotNull(MixpanelProperty.Ip, ip);
            od.SetPropertyIfNotNull(MixpanelProperty.Time, time);

            return builder.GetObject(od);
        }

        #endregion

        #region PeopleSet

        public bool PeopleSet(object props)
        {
            return SendMessage(CreatePeopleSetObject(null, props), EndpointEngage, "PeopleSet");
        }

        public bool PeopleSet(object distinctId, object props)
        {
            return SendMessage(CreatePeopleSetObject(distinctId, props), EndpointEngage, "PeopleSet");
        }

        private IDictionary<string, object> CreatePeopleSetObject(object distinctId, object props)
        {
            return GetObject(
                new PeopleSetMessageBuilder(_config), PeopleSetMessageBuilder.SpecialPropsBindings,
                distinctId, props);
        }

        #endregion PeopleSet

        #region PeopleSetOnce

        public bool PeopleSetOnce(object props)
        {
            return SendMessage(
                CreatePeopleSetOnceObject(null, props), EndpointEngage, "PeopleSetOnce");
        }

        public bool PeopleSetOnce(object distinctId, object props)
        {
            return SendMessage(
                CreatePeopleSetOnceObject(distinctId, props), EndpointEngage, "PeopleSetOnce");
        }

        private IDictionary<string, object> CreatePeopleSetOnceObject(object distinctId, object props)
        {
            return GetObject(
                new PeopleSetOnceMessageBuilder(_config), PeopleSetOnceMessageBuilder.SpecialPropsBindings,
                distinctId, props);
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

        public bool PeopleDelete(object distinctId)
        {
            throw new NotImplementedException();
        }

        private IDictionary<string, object> CreatePeopleDeleteObject(object distinctId)
        {
            var builder = new PeopleSetMessageBuilder(_config);
            var od = new ObjectData(PeopleSetMessageBuilder.SpecialPropsBindings, _config);

            od.SetProperty(MixpanelProperty.Token, _token);
            od.SetPropertyIfNotNull(MixpanelProperty.DistinctId, distinctId);

            return builder.GetObject(od);
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

        private IDictionary<string, object> GetObject(
            MessageBuilderBase builder, IDictionary<string, string> specialPropsBindings,
            object distinctId, object props)
        {
            var od = new ObjectData(specialPropsBindings, _config);
            od.ParseAndSetProperties(props);
            od.SetProperty(MixpanelProperty.Token, _token);
            od.SetPropertyIfNotNull(MixpanelProperty.DistinctId, distinctId);

            return builder.GetObject(od);
        }

        #region Super properties

        //TODO: Tests. <Aleksandr Ivanov - 23-05-2014>
        //TODO: Add super properties to created objects. <Aleksandr Ivanov - 23-05-2014>
        private ObjectData _superProperties;

        private void InitializeSuperProperties(object superProperties)
        {
            if (_superProperties == null)
            {
                _superProperties = new ObjectData(null, _config);
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