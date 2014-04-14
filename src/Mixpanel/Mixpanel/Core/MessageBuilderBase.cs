using System.Collections.Generic;

namespace Mixpanel.Core
{
    /// <summary>
    /// Base class for building mixpanel messages that will be serialized to json 
    /// and sent to Mixpanel.
    /// </summary>
    internal abstract class MessageBuilderBase
    {
        protected readonly MixpanelConfig Config;
        protected readonly ValueParser ValueParser;
        protected readonly PropertyNameFormatter PropertyNameFormatter;

        protected MessageBuilderBase(MixpanelConfig config = null)
        {
            Config = config;
            ValueParser = new ValueParser();
            PropertyNameFormatter = new PropertyNameFormatter(config);
        }
        
        public abstract IDictionary<string, object> GetObject(ObjectData objectData);

        protected void SetSpecialProperty(
            IDictionary<string, object> obj, ObjectData objData, string propName, string objPropName)
        {
            var val = objData.GetSpecialProp(propName);
            if (val != null)
            {
                obj[objPropName] = val;
            }
        }

        protected void SetSpecialProperties(
            IDictionary<string, object> obj, ObjectData objData, IDictionary<string, string> propNames)
        {
            foreach (var propName in propNames)
            {
                SetSpecialProperty(obj, objData, propName.Key, propName.Value);
            }
        }
    }
}