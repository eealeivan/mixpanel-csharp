using System;
using System.Collections.Generic;
using System.Globalization;
using Mixpanel.Misc;

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
            IDictionary<string, object> obj, ObjectData objData, string propName, string objPropName,
            Func<object, object> convertFn = null)
        {
            var val = objData.GetSpecialProp(propName, convertFn);
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

        protected object ConvertToUnixTime(object val)
        {
            if (val == null)
            {
                return null;
            }

            DateTime dateTime;
            if (DateTime.TryParseExact(val.ToString(), ValueParser.MixpanelDateFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
            {
                return dateTime.ToUnixTime();
            }
            return null;
        }
    }
}