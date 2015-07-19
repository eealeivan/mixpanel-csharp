using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Mixpanel.Exceptions;
using Mixpanel.Misc;

namespace Mixpanel.Core.Message
{
    /// <summary>
    /// Base class for building mixpanel messages that will be serialized to json 
    /// and sent to Mixpanel.
    /// </summary>
    internal abstract class MessageBuilderBase
    {
        /// <summary>
        /// Dictionary which keys are used to find special properties.
        /// Example: { "$first_name", "$first_name" }, { "firstname", "$first_name" }, that means both
        /// "$first_name" and "firstname" will be interpreted as "$first_name" special property.
        /// Also contains bindings for "distinct_id" property. If you need only "distinct_id" bindings
        /// then use <see cref="DistinctIdPropsBindings"/>.
        /// </summary>
        public abstract IDictionary<string, string> SpecialPropsBindings { get; }

        /// <summary>
        /// Dictionary which keys are used to find "distinct_id" special properties.
        /// Needed when parsing super properties because in some cases only "distinct_id" should be taken
        /// and all other properties ignored.
        /// </summary>
        public abstract IDictionary<string, string> DistinctIdPropsBindings { get; } 

        public abstract IDictionary<string, object> GetMessageObject(MessageData messageData);
        
        public virtual SuperPropertiesRules SuperPropertiesRules
        {
            get { return SuperPropertiesRules.DistinctIdOnly; }
        }

        public virtual MessagePropetiesRules MessagePropetiesRules
        {
            get { return MessagePropetiesRules.None; }
        }

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

        protected void ThrowIfPropertyIsNullOrEmpty(object val, string propertyName)
        {
            Debug.Assert(val != null);
            Debug.Assert(propertyName != null);

            if (val.ToString().IsNullOrWhiteSpace())
                throw new MixpanelRequiredPropertyNullOrEmptyException(
                    string.Format("'{0}' property can't be empty.", propertyName));
        }
    }
}