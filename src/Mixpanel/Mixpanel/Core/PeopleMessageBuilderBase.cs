using System;
using System.Collections.Generic;
using Mixpanel.Exceptions;

namespace Mixpanel.Core
{
    internal abstract class PeopleMessageBuilderBase : MessageBuilderBase
    {
        protected PeopleMessageBuilderBase(MixpanelConfig config = null)
            : base(config)
        {
        }

        protected static readonly Dictionary<string, string> CoreSpecialPropsBindings =
            new Dictionary<string, string>
            {
                {"$token", MixpanelProperty.Token},
                {"token", MixpanelProperty.Token},

                {"distinct_id", MixpanelProperty.DistinctId},
                {"distinctid", MixpanelProperty.DistinctId},
                {"$distinct_id", MixpanelProperty.DistinctId},
            };

        protected IDictionary<string, object> GetCoreObject(ObjectData objectData)
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

            return obj;
        }
    }
}