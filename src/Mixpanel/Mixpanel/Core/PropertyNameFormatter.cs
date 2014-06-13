using System.Diagnostics;
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
            Debug.Assert(!string.IsNullOrWhiteSpace(propName));

            var propertyNameFormat = _config != null
                ? _config.MixpanelPropertyNameFormat
                : MixpanelConfig.Global.MixpanelPropertyNameFormat;

            if (propertyNameFormat == MixpanelPropertyNameFormat.None || 
                propertyNameSource != PropertyNameSource.Default)
            {
                return propName;
            }

            bool sentenseCase = propertyNameFormat == MixpanelPropertyNameFormat.SentenceCase;
            bool titleCase = propertyNameFormat == MixpanelPropertyNameFormat.TitleCase;
            bool lowerCase = propertyNameFormat == MixpanelPropertyNameFormat.LowerCase;

            var newName = new StringBuilder(propName.Length + 5);

            var firstLetter = propName[0];
            if ((sentenseCase || titleCase) && !char.IsUpper(firstLetter))
            {
                firstLetter = char.ToUpper(firstLetter);
            }
            else if(lowerCase && !char.IsLower(firstLetter))
            {
                firstLetter = char.ToLower(firstLetter);
            }
            newName.Append(firstLetter);

            for (int i = 1; i < propName.Length; i++)
            {
                var letter = propName[i];
                if (char.IsUpper(letter))
                {
                    // Do not add space if previous letter is space
                    if (propName[i - 1] != ' ')
                    {
                        newName.Append(' ');
                    }

                    if (sentenseCase || lowerCase)
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