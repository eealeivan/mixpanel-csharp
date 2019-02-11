using System;
using System.Collections.Generic;
using Mixpanel.MessageProperties;

namespace Mixpanel.MessageBuilders
{
    internal sealed class MessageCandidate
    {
        private readonly MixpanelConfig config;
        private readonly Func<string, string> mapRawNameToSpecialPropertyFn;

        public Dictionary<string, ObjectProperty> SpecialProperties { get; }
        public Dictionary<string, ObjectProperty> UserProperties { get; }

        public bool HasSpecialProperty(string name) => 
            SpecialProperties.ContainsKey(name);

        public ObjectProperty GetSpecialProperty(string name) =>
            SpecialProperties.TryGetValue(name, out var objectProperty) ? objectProperty : null;

        public MessageCandidate(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            object rawProperties,
            object distinctId,
            MixpanelConfig config,
            Func<string, string> mapRawNameToSpecialPropertyFn)
        {
            this.config = config;
            this.mapRawNameToSpecialPropertyFn = mapRawNameToSpecialPropertyFn;

            SpecialProperties = new Dictionary<string, ObjectProperty>();
            UserProperties = new Dictionary<string, ObjectProperty>();

            ProcessSuperProperties(superProperties);
            ProcessRawProperties(rawProperties);
            ProcessToken(token);
            ProcessDistinctId(distinctId);
        }

        private void ProcessSuperProperties(IEnumerable<ObjectProperty> superProperties)
        {
            if (superProperties == null)
            {
                return;
            }

            foreach (ObjectProperty superProperty in superProperties)
            {
                ProcessObjectProperty(superProperty);
            }
        }

        private void ProcessRawProperties(object rawProperties)
        {
            if (rawProperties == null)
            {
                return;
            }

            foreach (ObjectProperty objectProperty in PropertiesDigger.Get(rawProperties, PropertyOrigin.RawProperty))
            {
                ProcessObjectProperty(objectProperty);
            }
        }

        private void ProcessToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            SpecialProperties[mapRawNameToSpecialPropertyFn("Token")] = 
                new ObjectProperty("Token", PropertyNameSource.Default, PropertyOrigin.Parameter, token);
        }

        private void ProcessDistinctId(object distinctId)
        {
            if (distinctId == null)
            {
                return;
            }

            SpecialProperties[mapRawNameToSpecialPropertyFn("DistinctId")] = 
                new ObjectProperty("DistinctId", PropertyNameSource.Default, PropertyOrigin.Parameter, distinctId);
        }

        private void ProcessObjectProperty(ObjectProperty objectProperty)
        {
            string specialProperty = mapRawNameToSpecialPropertyFn(objectProperty.PropertyName);
            if (specialProperty != null)
            {
                SpecialProperties[specialProperty] = objectProperty;
            }
            else
            {
                string formattedPropertyName = PropertyNameFormatter.Format(objectProperty, config);
                UserProperties[formattedPropertyName] = objectProperty;
            }
        }
    }
}