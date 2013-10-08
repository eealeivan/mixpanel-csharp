using System;
using System.Collections.Generic;
using Mixpanel.Exceptions;

namespace Mixpanel.Builders
{
    internal sealed class MixpanelData
    {
        private readonly IDictionary<string, string> _specialPropsBindings;
        private readonly ValueParser _valueParser;
        private readonly PropertyNameFormatter _nameFormatter;
        private readonly PropertiesDigger _propertiesDigger;

        /// <summary>
        /// Contais Mixpanel special properties like 'token', 'distinct_id' and etc.
        /// </summary>
        public IDictionary<string, object> SpecialProps { get; private set; }

        /// <summary>
        /// Contains user properties
        /// </summary>
        public IDictionary<string, object> Props { get; private set; }

        public MixpanelData(IDictionary<string, string> specialPropsBindings, MixpanelConfig config = null)
        {
            _specialPropsBindings = specialPropsBindings;
            _valueParser = new ValueParser();
            _nameFormatter = new PropertyNameFormatter(config);
            _propertiesDigger = new PropertiesDigger();

            SpecialProps = new Dictionary<string, object>();
            Props = new Dictionary<string, object>();
        }

        public void ParseAndSetProperties(object props)
        {
            if(props == null) return;

            foreach (var pair in _propertiesDigger.Get(props))
            {
                SetProperty(pair.Key, pair.Value);
            }
        }

        public void SetProperty(string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName)) return;

            var parsedValue = _valueParser.Parse(value);
            if (!parsedValue.Item2) return;

            string bindingProp;
            if (_specialPropsBindings.TryGetValue(propertyName.ToLower(), out bindingProp))
            {
                SpecialProps[bindingProp] = parsedValue.Item1;
            }
            else
            {
                Props[_nameFormatter.Format(propertyName)] = parsedValue.Item1;
            }
        }

        /// <summary>
        /// Gets special Mixpanel property and throws <see cref="MixpanelObjectStructureException"/> 
        /// if property is not set or <see cref="MixpanelRequiredPropertyNullOrEmptyException"/> if property
        /// value is null.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="validateFn">Custom validation function. Value will not be null.</param>
        /// <param name="convertFn">Custom value convert function.</param>
        public object GetSpecialRequiredProp(string propName, 
            Action<object> validateFn = null, Func<object, object> convertFn = null)
        {
            object propValue;
            if (!SpecialProps.TryGetValue(propName, out propValue))
                throw new MixpanelObjectStructureException(
                    string.Format("'{0}' property is not set.", propName));

            if (propValue == null)
                throw new MixpanelRequiredPropertyNullOrEmptyException(
                    string.Format("'{0}' property can't be null.", propName));

            if (validateFn != null)
            {
                validateFn(propValue);
            }

            if (convertFn != null)
            {
                propValue = convertFn(propValue);
            }

            return propValue;
        }

        public object GetSpecialProp(string propName, Func<object, object> convertFn = null)
        {
            object val;
            if (SpecialProps.TryGetValue(propName, out val))
            {
                if (convertFn != null)
                {
                    val = convertFn(val);
                }
                return val;
            }
            return null;
        }
    }
}
