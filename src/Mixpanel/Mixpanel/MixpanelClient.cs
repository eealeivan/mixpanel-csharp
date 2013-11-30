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
            try
            {
                var obj = CreateTrackObject(@event, props, distinctId, ip, time);
                var url = string.Format(UrlFormat, EndpointTrack);
                var formData = "data=" + ToBase64(ToJson(obj));
                return Send(url, formData);
            }
            catch (Exception)
            {
                // TODO: Add logging
                return false;
            }
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
            od.SetProperty(MixpanelProperty.DistinctId, distinctId);
            od.SetProperty(MixpanelProperty.Ip, ip);
            od.SetProperty(MixpanelProperty.Time, time);

            return builder.GetObject(od);
        }

        #endregion

        private string ToJson(object obj)
        {
            return ConfigHelper.GetSerializeJsonFn(_config)(obj);
        }

        private string ToBase64(string json)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private bool Send(string url, string formData)
        {
            return ConfigHelper.GetHttpPostFn(_config)(url, formData);
        }
    }
}