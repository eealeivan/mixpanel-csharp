using System;
using System.Collections.Generic;
using Mixpanel.Exceptions;

namespace Mixpanel.Core
{
    internal sealed class PeopleSetMessageBuilder : PeopleMessageBuilderBase
    {
        public static readonly Dictionary<string, string> SpecialPropsBindings =
           new Dictionary<string, string>
            {
                {"$ip", MixpanelProperty.Ip},
                {"ip", MixpanelProperty.Ip},

                {"$time", MixpanelProperty.Time},
                {"time", MixpanelProperty.Time},
                
                {"$ignore_time", MixpanelProperty.IgnoreTime},
                {"ignore_time", MixpanelProperty.IgnoreTime},
                {"ignoretime", MixpanelProperty.IgnoreTime},

                {"$first_name", MixpanelProperty.FirstName},
                {"first_name", MixpanelProperty.FirstName},
                {"firstname", MixpanelProperty.FirstName}, 
                
                {"$last_name", MixpanelProperty.LastName},
                {"last_name", MixpanelProperty.LastName},
                {"lastname", MixpanelProperty.LastName},

                {"$name", MixpanelProperty.Name},
                {"name", MixpanelProperty.Name},

                {"$created", MixpanelProperty.Created},
                {"created", MixpanelProperty.Created},

                {"$email", MixpanelProperty.Email},
                {"email", MixpanelProperty.Email},
                {"e-mail", MixpanelProperty.Email},

                {"$phone", MixpanelProperty.Phone},
                {"phone", MixpanelProperty.Phone},
            };

        static PeopleSetMessageBuilder()
        {
            // Add token and distinct_id bindings
            foreach (var binding in CoreSpecialPropsBindings)
            {
                SpecialPropsBindings.Add(binding.Key, binding.Value);
            }
        }

        public PeopleSetMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, object> GetObject(ObjectData objectData)
        {
            IDictionary<string, object> obj = GetCoreObject(objectData);

            // $ip
            object ip = objectData.GetSpecialProp(MixpanelProperty.Ip, x => x.ToString());
            if (ip != null)
            {
                obj["$ip"] = ip;
            }

            // $time
            SetSpecialProperty(obj, objectData, MixpanelProperty.Time, "$time", ConvertToUnixTime);
            SetSpecialProperty(obj, objectData, MixpanelProperty.IgnoreTime, "$ignore_time");

            // $set
            var set = new Dictionary<string, object>();
            obj["$set"] = set;

            SetSpecialProperties(set, objectData, new Dictionary<string, string>
            {
                { MixpanelProperty.FirstName, "$first_name" },
                { MixpanelProperty.LastName, "$last_name" },
                { MixpanelProperty.Name, "$name" },
                { MixpanelProperty.Created, "$created" },
                { MixpanelProperty.Email, "$email" },
                { MixpanelProperty.Phone, "$phone" },
            });

            // Other properties
            foreach (var prop in objectData.Props)
            {
                set[prop.Key] = prop.Value;
            }

            return obj;
        }
    }
}