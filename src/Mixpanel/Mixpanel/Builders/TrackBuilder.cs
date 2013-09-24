using System;
using System.Collections.Generic;
using Mixpanel.PropertyTypes;

namespace Mixpanel.Builders
{
    internal class TrackBuilder : BuilderBase
    {
        private static readonly Dictionary<string, string> BindingProps =
            new Dictionary<string, string>
            {
                {"event", MixpanelTrackProperty.Event},

                {"token", MixpanelTrackProperty.Token},

                {"distinct_id", MixpanelTrackProperty.DistinctId},
                {"distinctid", MixpanelTrackProperty.DistinctId},

                {"ip", MixpanelTrackProperty.DistinctId},

                {"time", MixpanelTrackProperty.Time},
            };

        private readonly IDictionary<string, Tuple<object, int>> _mixpanelProps =
            new Dictionary<string, Tuple<object, int>>();

        private readonly IDictionary<string, object> _otherProps =
            new Dictionary<string, object>();

        public void Add(string propertyName, object value, int weight)
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
                if (!_mixpanelProps.TryGetValue(MixpanelTrackProperty.Event, out @event))
                {
                    throw new Exception("'Event' property is not set.");
                }
                obj[MixpanelTrackProperty.Event] = @event.Item1;

                // properties
                var properties = new Dictionary<string, object>();
                obj["properties"] = properties;

                Tuple<object, int> token;
                if (_mixpanelProps.TryGetValue(MixpanelTrackProperty.Token, out token))
                {
                    properties[MixpanelTrackProperty.Token] = token.Item1;
                }

                Tuple<object, int> distinctId;
                if (_mixpanelProps.TryGetValue(MixpanelTrackProperty.DistinctId, out distinctId))
                {
                    properties[MixpanelTrackProperty.DistinctId] = distinctId.Item1;
                }

                Tuple<object, int> ip;
                if (_mixpanelProps.TryGetValue(MixpanelTrackProperty.IpAddress, out ip))
                {
                    properties[MixpanelTrackProperty.IpAddress] = ip.Item1;
                }

                Tuple<object, int> time;
                if (_mixpanelProps.TryGetValue(MixpanelTrackProperty.Time, out time))
                {
                    properties[MixpanelTrackProperty.Time] = time.Item1;
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
