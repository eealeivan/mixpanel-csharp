using System;
using System.Collections.Generic;
using Mixpanel.Exceptions;

namespace Mixpanel.Core
{
    internal sealed class PeopleSetMessageBuilder : MessageBuilderBase
    {
        public static readonly Dictionary<string, string> SpecialPropsBindings =
           new Dictionary<string, string>
            {
                {"$token", MixpanelProperty.Token},
                {"token", MixpanelProperty.Token},
                
                {"distinct_id", MixpanelProperty.DistinctId},
                {"distinctid", MixpanelProperty.DistinctId},
                {"$distinct_id", MixpanelProperty.DistinctId},

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

        public PeopleSetMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, object> GetObject(ObjectData objectData)
        {
            var obj = new Dictionary<string, object>();

            // $token
            obj["$token"] = objectData.GetSpecialRequiredProp(MixpanelProperty.Token,
               x =>
               {
                   if (String.IsNullOrWhiteSpace(x.ToString()))
                       throw new MixpanelRequiredPropertyNullOrEmptyException(
                           "'$token' property can't be empty.");
               },
               x => x.ToString());

            // $distinct_id
            obj["$distinct_id"] = objectData.GetSpecialRequiredProp(MixpanelProperty.DistinctId,
               x =>
               {
                   if (String.IsNullOrWhiteSpace(x.ToString()))
                       throw new MixpanelRequiredPropertyNullOrEmptyException(
                           "'$distinct_id' property can't be empty.");
               },
               x => x.ToString());

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