using System;
using System.Collections.Generic;
using System.Text;
using Mixpanel.Builders;

namespace Mixpanel
{
    public class MixpanelClient : IMixpanelClient
    {
        private readonly string _token;
        private readonly MixpanelConfig _config;

        public MixpanelClient(string token, MixpanelConfig config = null)
        {
            if(string.IsNullOrWhiteSpace(token))
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
                return Send("track", ToBase64(ToJson(obj)));
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
            var md = new MixpanelData(TrackBuilder.SpecialPropsBindings, _config);

            md.ParseAndSetProperties(props);
            md.SetProperty(MixpanelProperty.Event, @event);
            md.SetProperty(MixpanelProperty.Token, _token);
            md.SetProperty(MixpanelProperty.DistinctId, distinctId);
            md.SetProperty(MixpanelProperty.Ip, ip);
            md.SetProperty(MixpanelProperty.Time, time);

            return builder.GetObject(md);
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

        private bool Send(string endpoint, string data)
        {
            return ConfigHelper.GetHttpPostFn(_config)(endpoint, data);
        }
    }
}