using System;
using System.Text;

namespace Mixpanel.Core
{
    internal class PropertyNameFormatter
    {
        private readonly MixpanelConfig _config;

        public PropertyNameFormatter(MixpanelConfig config = null)
        {
            _config = config;
        }

        public string Format(string propName)
        {
            if(string.IsNullOrWhiteSpace(propName))
                throw new ArgumentNullException("propName");

            if ((_config != null && _config.PropertyNameFormat == PropertyNameFormat.None) ||
                MixpanelGlobalConfig.PropertyNameFormat == PropertyNameFormat.None)
            {
                return propName;
            }

            var sentenseTitleCase =
                (_config != null && _config.PropertyNameFormat == PropertyNameFormat.SentenceTitleCase) ||
                MixpanelGlobalConfig.PropertyNameFormat == PropertyNameFormat.SentenceTitleCase;

            var sentenceCapitalized =
                (_config != null && _config.PropertyNameFormat == PropertyNameFormat.SentenseCapitilized) ||
                MixpanelGlobalConfig.PropertyNameFormat == PropertyNameFormat.SentenseCapitilized;

            var sentenceLowerCase =
                (_config != null && _config.PropertyNameFormat == PropertyNameFormat.SentenceLowerCase) ||
                MixpanelGlobalConfig.PropertyNameFormat == PropertyNameFormat.SentenceLowerCase;

            var bPropName = new StringBuilder(propName.Length + 5);

            var firstLetter = propName[0];
            if ((sentenseTitleCase || sentenceCapitalized) && !char.IsUpper(firstLetter))
            {
                firstLetter = char.ToUpper(firstLetter);
            }
            else if(sentenceLowerCase && !char.IsLower(firstLetter))
            {
                firstLetter = char.ToLower(firstLetter);
            }
            bPropName.Append(firstLetter);

            for (int i = 1; i < propName.Length; i++)
            {
                var letter = propName[i];
                if (char.IsUpper(letter))
                {
                    bPropName.Append(" ");
                    if (sentenceCapitalized || sentenceLowerCase)
                    {
                        letter = char.ToLower(letter);
                    }
                }
                bPropName.Append(letter);
            }

            return bPropName.ToString();
        }
    }
}