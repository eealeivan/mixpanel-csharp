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

        public MixpanelClient(string token, MixpanelConfig config = null)
        {
            if(String.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token");

            _token = token;
            _config = config;
        }

        #region Track

        public bool Track(
            string @event, object props = null, object distinctId = null,
            string ip = null, DateTime? time = null)
        {
            string url, formData;
            try
            {
                var obj = CreateTrackObject(@event, props, distinctId, ip, time);
                url = string.Format(UrlFormat, EndpointTrack);
                formData = GetFormData(obj);
            }
            catch (Exception e)
            {
                LogError("Error creating 'track' object.", e);
                return false;
            }

            return Send(url, formData);
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
            var builder = new TrackBuilder(_config);
            var od = new ObjectData(TrackBuilder.SpecialPropsBindings, _config);

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

        public bool PeopleSet(
            object distinctId = null, object props = null, string ip = null, DateTime? time = null, bool ignoreTime = true)
        {
            string url, formData;
            try
            {
                var obj = CreatePeopleSetObject(distinctId, props, ip, time, ignoreTime);
                url = string.Format(UrlFormat, EndpointEngage);
                formData = GetFormData(obj);
            }
            catch (Exception e)
            {
                LogError("Error creating 'people-set' object.", e);
                return false;
            }

            return Send(url, formData);
        }

        private IDictionary<string, object> CreatePeopleSetObject(
            object distinctId, object props, string ip, DateTime? time, bool ignoreTime)
        {
            var builder = new PeopleSetMessageBuilder(_config);
            var od = new ObjectData(PeopleSetMessageBuilder.SpecialPropsBindings, _config);

            od.ParseAndSetProperties(props);
            od.SetProperty(MixpanelProperty.Token, _token);
            od.SetPropertyIfNotNull(MixpanelProperty.DistinctId, distinctId);
            od.SetPropertyIfNotNull(MixpanelProperty.Ip, ip);
            od.SetPropertyIfNotNull(MixpanelProperty.Time, time);
            od.SetPropertyIfNotNull(MixpanelProperty.IgnoreTime, ignoreTime);

            return builder.GetObject(od);
        }

        #endregion PeopleSet

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

        private bool Send(string url, string formData)
        {
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