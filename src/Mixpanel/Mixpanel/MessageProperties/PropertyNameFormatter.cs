using System.Text;

namespace Mixpanel.MessageProperties
{
    internal static class PropertyNameFormatter
    {
        public static string Format(
            ObjectProperty objectProperty, 
            MixpanelConfig config = null)
        {
            return Format(objectProperty.PropertyName, objectProperty.PropertyNameSource, config);
        }

        public static string Format(
            string propertyName, 
            PropertyNameSource propertyNameSource, 
            MixpanelConfig config = null)
        {
            MixpanelPropertyNameFormat propertyNameFormat =
                ConfigHelper.GetMixpanelPropertyNameFormat(config);

            if (propertyNameFormat == MixpanelPropertyNameFormat.None ||
                propertyNameSource != PropertyNameSource.Default)
            {
                return propertyName;
            }

            bool sentenceCase = propertyNameFormat == MixpanelPropertyNameFormat.SentenceCase;
            bool titleCase = propertyNameFormat == MixpanelPropertyNameFormat.TitleCase;
            bool lowerCase = propertyNameFormat == MixpanelPropertyNameFormat.LowerCase;

            var newName = new StringBuilder(propertyName.Length + 5);

            var firstLetter = propertyName[0];
            if ((sentenceCase || titleCase) && !char.IsUpper(firstLetter))
            {
                firstLetter = char.ToUpper(firstLetter);
            }
            else if (lowerCase && !char.IsLower(firstLetter))
            {
                firstLetter = char.ToLower(firstLetter);
            }
            newName.Append(firstLetter);

            for (int i = 1; i < propertyName.Length; i++)
            {
                var letter = propertyName[i];
                if (char.IsUpper(letter))
                {
                    // Do not add space if previous letter is space
                    // Or the current word is "ID" and we're in title case mode
                    if (propertyName[i - 1] != ' '
                        && !(titleCase
                        && char.ToUpper(propertyName[i - 1]).Equals('I')
                        && char.ToUpper(letter).Equals('D')))
                    {
                        newName.Append(' ');
                    }

                    if (sentenceCase || lowerCase)
                    {
                        letter = char.ToLower(letter);
                    }
                }
                else if (titleCase && char.ToUpper(letter).Equals('D') && propertyName[i - 1].Equals('I'))
                {
                    letter = char.ToUpper(letter);
                }

                newName.Append(letter);
            }

            return newName.ToString();
        }
    }
}