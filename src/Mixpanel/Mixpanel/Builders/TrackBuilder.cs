using System;
using System.Collections.Generic;

namespace Mixpanel.Builders
{
    internal class TrackBuilder : BuilderBase
    {
        private static readonly Dictionary<string, string> BindingProps =
            new Dictionary<string, string>
            {
                {"event", MixpanelProperty.Event},

                {"token", MixpanelProperty.Token},

                {"distinct_id", MixpanelProperty.DistinctId},
                {"distinctid", MixpanelProperty.DistinctId},

                {"ip", MixpanelProperty.Ip},

                {"time", MixpanelProperty.Time},
            };

        private readonly IDictionary<string, Tuple<object, int>> _mixpanelProps =
            new Dictionary<string, Tuple<object, int>>();

        private readonly IDictionary<string, object> _otherProps =
            new Dictionary<string, object>();

        public void Add(string propertyName, object value, int weight = 1)
        {
            //TODO: Values parsing (mixpanel supported types)
            string bindingProp;
            if (BindingProps.TryGetValue(propertyName.ToLower(), out bindingProp))
            {
                Tuple<object, int> mpProp;
                if (_mixpanelProps.TryGetValue(bindingProp, out mpProp))
                {
                    if (weight > mpProp.Item2)
                        _mixpanelProps[bindingProp] = Tuple.Create(value, weight);
                }
                else
                {
                    _mixpanelProps.Add(bindingProp, Tuple.Create(value, weight));
                }
            }
            else
            {
                _otherProps[propertyName] = value;
            }
        }

        public override IDictionary<string, object> Object
        {
            get
            {
                var obj = new Dictionary<string, object>();

                // event
                Tuple<object, int> @event;
                if (!_mixpanelProps.TryGetValue(MixpanelProperty.Event, out @event))
                {
                    throw new Exception("'Event' property is not set.");
                }
                obj[MixpanelProperty.Event] = @event.Item1;

                // properties
                var properties = new Dictionary<string, object>();
                obj["properties"] = properties;

                Tuple<object, int> token;
                if (_mixpanelProps.TryGetValue(MixpanelProperty.Token, out token))
                {
                    properties["token"] = token.Item1;
                }

                Tuple<object, int> distinctId;
                if (_mixpanelProps.TryGetValue(MixpanelProperty.DistinctId, out distinctId))
                {
                    properties["distinct_id"] = distinctId.Item1;
                }

                Tuple<object, int> ip;
                if (_mixpanelProps.TryGetValue(MixpanelProperty.Ip, out ip))
                {
                    properties["ip"] = ip.Item1;
                }

                Tuple<object, int> time;
                if (_mixpanelProps.TryGetValue(MixpanelProperty.Time, out time))
                {
                    properties["time"] = time.Item1;
                }

                //TODO: Names changing according to config
                foreach (var otherProp in _otherProps)
                {
                    properties[otherProp.Key] = otherProp.Value;
                }

                return obj;
            }
        }
    }
}
