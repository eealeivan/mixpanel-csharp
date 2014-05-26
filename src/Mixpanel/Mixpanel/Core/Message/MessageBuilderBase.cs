using System;
using System.Collections.Generic;
using System.Globalization;
using Mixpanel.Misc;

namespace Mixpanel.Core.Message
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

        public abstract IDictionary<string, object> GetMessageObject(MessageData messageData);

        protected void SetSpecialRequiredProperty(
            IDictionary<string, object> obj, MessageData messageData, string propName,
            Action<object> validateFn = null, Func<object, object> convertFn = null)
        {
            obj[propName] = messageData.GetSpecialRequiredProp(propName, validateFn, convertFn);
        }

        protected void SetSpecialProperty(
            IDictionary<string, object> obj, MessageData messageData, string propName,
            Func<object, object> convertFn = null)
        {
            var val = messageData.GetSpecialProp(propName, convertFn);
            if (val != null)
            {
                obj[propName] = val;
            }
        }

        protected void SetSpecialProperties(
            IDictionary<string, object> obj, MessageData messageData,
            IDictionary<string, Func<object, object>> propNames)
        {
            foreach (var propName in propNames)
            {
                SetSpecialProperty(obj, messageData, propName.Key, propName.Value);
            }
        }

        protected void SetNormalProperties(IDictionary<string, object> obj, MessageData messageData)
        {
            foreach (var prop in messageData.Props)
            {
                obj[prop.Key] = prop.Value;
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