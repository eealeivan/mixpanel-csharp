using System;
using System.Collections.Generic;
using Mixpanel.Exceptions;
using Mixpanel.Misc;

namespace Mixpanel.Core.Message
{
    internal sealed class MessageData
    {
        private readonly IDictionary<string, string> _specialPropsBindings;
        private readonly IDictionary<string, string> _distinctIdPropsBindings;
        private readonly MessagePropetiesRules _messagePropetiesRules;
        private readonly SuperPropertiesRules _superPropertiesRules;
        private readonly ValueParser _valueParser;
        private readonly PropertyNameFormatter _nameFormatter;
        private readonly PropertiesDigger _propertiesDigger;

        /// <summary>
        /// Contais parsed Mixpanel special properties like 'token', 'distinct_id' etc.
        /// </summary>
        public IDictionary<string, object> SpecialProps { get; private set; }

        /// <summary>
        /// Contains parsed user properties.
        /// </summary>
        public IDictionary<string, object> Props { get; private set; }

        public MessageData(
            IDictionary<string, string> specialPropsBindings,
            IDictionary<string, string> distinctIdPropsBindings,
            MessagePropetiesRules messagePropetiesRules,
            SuperPropertiesRules superPropertiesRules,
            MixpanelConfig config = null)
        {
            _specialPropsBindings = specialPropsBindings ?? new Dictionary<string, string>();
            _distinctIdPropsBindings = distinctIdPropsBindings ?? new Dictionary<string, string>();
            _messagePropetiesRules = messagePropetiesRules;
            _superPropertiesRules = superPropertiesRules;
            _valueParser = new ValueParser();
            _nameFormatter = new PropertyNameFormatter(config);
            _propertiesDigger = new PropertiesDigger();

            SpecialProps = new Dictionary<string, object>();
            Props = new Dictionary<string, object>();
        }

        public void ParseAndSetProperties(object props)
        {
            if (props == null)
            {
                return;
            }

            foreach (var property in _propertiesDigger.Get(props))
            {
                SetProperty(property.PropertyName, property.Value, property.PropertyNameSource);
            }
        }

        public void ParseAndSetPropertiesIfNotNull(object props)
        {
            if (props == null)
            {
                return;
            }

            foreach (var property in _propertiesDigger.Get(props))
            {
                SetPropertyIfNotNull(property.PropertyName, property.Value, property.PropertyNameSource);
            }
        }

        public void ParseAndSetSuperProperties(object superProperties)
        {
            if (superProperties == null)
            {
                return;
            }

            foreach (var property in _propertiesDigger.Get(superProperties))
            {
                switch (_superPropertiesRules)
                {
                    case SuperPropertiesRules.All:
                        SetProperty(property.PropertyName, property.Value, property.PropertyNameSource);
                        break;
                    case SuperPropertiesRules.DistinctIdOnly:
                        string distinctIdBindingProp;
                        bool isDistinctId = _distinctIdPropsBindings.TryGetValue(
                            property.PropertyName.ToLower(), out distinctIdBindingProp);

                        if (isDistinctId)
                        {
                            SetProperty(property.PropertyName, property.Value, property.PropertyNameSource);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool SetProperty(string propertyName, object value,
            PropertyNameSource propertyNameSource = PropertyNameSource.Default)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return false;
            }

            var parsedProperty = _valueParser.Parse(value);
            if (!parsedProperty.IsValid)
            {
                return false;
            }

            string bindingProp;
            if (_specialPropsBindings.TryGetValue(propertyName.ToLower(), out bindingProp))
            {
                SpecialProps[bindingProp] = parsedProperty.Value;
            }
            else
            {
                switch (_messagePropetiesRules)
                {
                    case MessagePropetiesRules.None:
                        break;
                    case MessagePropetiesRules.NumericsOnly:
                        if (!_valueParser.IsNumeric(parsedProperty.Value))
                        {
                            return false;
                        }
                        break;
                    case MessagePropetiesRules.ListsOnly:
                        if (!_valueParser.IsEnumerable(parsedProperty.Value))
                        {
                            return false;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Props[_nameFormatter.Format(propertyName, propertyNameSource)] = parsedProperty.Value;
            }

            return true;
        }

        public bool SetPropertyIfNotNull(string propertyName, object value,
            PropertyNameSource propertyNameSource = PropertyNameSource.Default)
        {
            if (value == null)
            {
                return false;
            }

            return SetProperty(propertyName, value, propertyNameSource);
        }

        public void RemoveProperty(string propertyName,
            PropertyNameSource propertyNameSource = PropertyNameSource.Default)
        {
            if (propertyName.IsNullOrWhiteSpace())
            {
                return;
            }

            if (!SpecialProps.Remove(propertyName))
            {
                Props.Remove(_nameFormatter.Format(propertyName, propertyNameSource));
            }
        }

        public void ClearAllProperties()
        {
            SpecialProps.Clear();
            Props.Clear();
        }

        /// <summary>
        /// Gets special Mixpanel property and throws <see cref="MixpanelObjectStructureException"/> 
        /// if property is not set or <see cref="MixpanelRequiredPropertyNullOrEmptyException"/> if property
        /// value is null.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="validateFn">
        /// Custom validation function. Value will not be null. If it was null an exception
        /// will be thrown before.
        /// </param>
        /// <param name="convertFn">Custom value convert function.</param>
        public object GetSpecialRequiredProp(string propName,
            Action<object> validateFn = null, Func<object, object> convertFn = null)
        {
            object val;
            if (!SpecialProps.TryGetValue(propName, out val))
                throw new MixpanelObjectStructureException(
                    string.Format("'{0}' property is not set.", propName));

            if (val == null)
                throw new MixpanelRequiredPropertyNullOrEmptyException(
                    string.Format("'{0}' property can't be null.", propName));

            if (validateFn != null)
            {
                validateFn(val);
            }

            if (convertFn != null)
            {
                val = convertFn(val);
            }

            return val;
        }

        public object GetSpecialProp(string propName, Func<object, object> convertFn = null)
        {
            object val;
            if (!SpecialProps.TryGetValue(propName, out val))
            {
                return null;
            }

            if (val != null && convertFn != null)
            {
                val = convertFn(val);
            }
            return val;
        }
    }
}
