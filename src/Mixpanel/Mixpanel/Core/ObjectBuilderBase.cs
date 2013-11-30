using System.Collections.Generic;

namespace Mixpanel.Core
{
    /// <summary>
    /// Base class for building mixpanel objects (dictionaries) that will be serialized to json 
    /// and sent to Mixpanel.
    /// </summary>
    internal abstract class ObjectBuilderBase
    {
        protected readonly MixpanelConfig Config;
        protected readonly ValueParser ValueParser;
        protected readonly PropertyNameFormatter PropertyNameFormatter;

        protected ObjectBuilderBase(MixpanelConfig config = null)
        {
            Config = config;
            ValueParser = new ValueParser();
            PropertyNameFormatter = new PropertyNameFormatter(config);
        }
        
        public abstract IDictionary<string, object> GetObject(ObjectData objectData);
    }
}