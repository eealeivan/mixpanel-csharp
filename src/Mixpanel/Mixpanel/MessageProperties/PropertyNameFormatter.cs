using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Char;
using static System.String;

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

            bool formatNeeded =
                propertyNameFormat != MixpanelPropertyNameFormat.None &&
                propertyNameSource == PropertyNameSource.Default;
            if (!formatNeeded || IsNullOrWhiteSpace(propertyName))
            {
                return propertyName;
            }

            var words = SplitInWords(propertyName);
            var newPropertyName = new StringBuilder(propertyName.Length + 5);

            switch (propertyNameFormat)
            {
                case MixpanelPropertyNameFormat.SentenceCase:
                    ApplySentenceCase(words);
                    break;
                case MixpanelPropertyNameFormat.TitleCase:
                    ApplyTitleCase(words);
                    break;
                case MixpanelPropertyNameFormat.LowerCase:
                    ApplyLowerCase(words);
                    break;
            }

            for (int i = 0; i < words.Length; i++)
            {
                newPropertyName.Append(words[i]);
                if (i != words.Length - 1)
                {
                    newPropertyName.Append(' ');
                }
            }

            return newPropertyName.ToString();
        }

        private static StringBuilder[] SplitInWords(string propertyName)
        {
            var word = new StringBuilder();
            word.Append(propertyName[0]);

            var words = new List<StringBuilder> { word };

            for (int i = 1; i < propertyName.Length; i++)
            {
                char c = propertyName[i];

                bool twoLetterAcronym =
                    IsUpper(c) &&
                    word.Length == 1 &&
                    IsUpper(word[0]);

                if (twoLetterAcronym)
                {
                    word.Append(c);
                    AddNewWord();
                }
                else if (IsUpper(c))
                {
                    AddNewWord();
                    word.Append(c);
                }
                else
                {
                    word.Append(c);
                }
            }

            return words.Where(w => w.Length > 0).ToArray();

            void AddNewWord()
            {
                word = new StringBuilder();
                words.Add(word);
            }
        }

        private static void ApplySentenceCase(StringBuilder[] words)
        {
            ApplyLowerCase(words.Skip(1));
            ApplyTitleCase(words);
        }

        private static void ApplyTitleCase(StringBuilder[] words)
        {
            words[0][0] = ToUpper(words[0][0]);
        }

        private static void ApplyLowerCase(IEnumerable<StringBuilder> words)
        {
            foreach (var word in words)
            {
                for (int i = 0; i < word.Length; i++)
                {
                    word[i] = ToLower(word[i]);
                }
            }
        }
    }
}