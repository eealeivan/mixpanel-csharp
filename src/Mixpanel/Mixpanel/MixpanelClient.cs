using System;
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

        public bool Track(
            string @event, object props = null, object distinctId = null, 
            string ip = null, DateTime? time = null)
        {
            var builder = new TrackBuilder(_config);
            var md = new MixpanelData(TrackBuilder.SpecialPropsBindings, _config);

            md.ParseAndSetProperties(props);
            md.SetProperty(MixpanelProperty.Event, @event);
            md.SetProperty(MixpanelProperty.Token, _token);
            md.SetProperty(MixpanelProperty.DistinctId, distinctId);
            md.SetProperty(MixpanelProperty.Ip, ip);
            md.SetProperty(MixpanelProperty.Time, time);

            var obj = builder.GetObject(md);
            return Send("track", Encode(ToJson(obj)));
        }

        private string ToJson(object obj)
        {
            return ConfigHelper.GetSerializeJsonFn(_config)(obj);
        }

        private string Encode(string json)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private bool Send(string endpoint, string data)
        {
            return ConfigHelper.GetHttpPostFn(_config)(endpoint, data);
        }
    }
}