using System.Collections.Generic;

namespace Mixpanel.Core
{
    internal sealed class PeopleSetOnceMessageBuilder : PeopleMessageBuilderBase
    {
        public static readonly Dictionary<string, string> SpecialPropsBindings =
            new Dictionary<string, string>();

        static PeopleSetOnceMessageBuilder()
        {
            // Add token and distinct_id bindings
            foreach (var binding in CoreSpecialPropsBindings)
            {
                SpecialPropsBindings.Add(binding.Key, binding.Value);
            }
        }

        public PeopleSetOnceMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, object> GetObject(ObjectData objectData)
        {
            IDictionary<string, object> obj = GetCoreObject(objectData);

            // $set_once
            var setOnce = new Dictionary<string, object>();
            obj["$set_once"] = setOnce;

            foreach (var prop in objectData.Props)
            {
                setOnce[prop.Key] = prop.Value;
            }

            return obj;
        }
    }
}