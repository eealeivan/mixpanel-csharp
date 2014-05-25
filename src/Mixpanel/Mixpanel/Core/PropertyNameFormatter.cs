using System;
using System.Text;

namespace Mixpanel.Core
{
    internal sealed class PropertyNameFormatter
    {
        private readonly MixpanelConfig _config;

        public PropertyNameFormatter(MixpanelConfig config = null)
        {
            _config = config;
        }

        public string Format(string propName, PropertyNameSource propertyNameSource = PropertyNameSource.Default)
        {
            if(string.IsNullOrWhiteSpace(propName))
                throw new ArgumentNullException("propName");

            var propertyNameFormat = _config != null
                ? _config.MixpanelPropertyNameFormat
                : MixpanelConfig.Global.MixpanelPropertyNameFormat;

            if (propertyNameFormat == MixpanelPropertyNameFormat.None || 
                propertyNameSource != PropertyNameSource.Default)
            {
                return propName;
            }

            bool sentenseTitleCase = propertyNameFormat == MixpanelPropertyNameFormat.SentenceTitleCase;
            bool sentenceCapitalized = propertyNameFormat == MixpanelPropertyNameFormat.SentenseCapitilized;
            bool sentenceLowerCase = propertyNameFormat == MixpanelPropertyNameFormat.SentenceLowerCase;

            var newName = new StringBuilder(propName.Length + 5);

            var firstLetter = propName[0];
            if ((sentenseTitleCase || sentenceCapitalized) && !char.IsUpper(firstLetter))
            {
                firstLetter = char.ToUpper(firstLetter);
            }
            else if(sentenceLowerCase && !char.IsLower(firstLetter))
            {
                firstLetter = char.ToLower(firstLetter);
            }
            newName.Append(firstLetter);

            for (int i = 1; i < propName.Length; i++)
            {
                var letter = propName[i];
                if (char.IsUpper(letter))
                {
                    newName.Append(" ");
                    if (sentenceCapitalized || sentenceLowerCase)
                    {
                        letter = char.ToLower(letter);
                    }
                }
                newName.Append(letter);
            }

            return newName.ToString();
        }
    }
}