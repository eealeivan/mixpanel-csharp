using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if !NET35
using System.Dynamic;
#endif

using System.Runtime.Serialization;
using Mixpanel.Core;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class PropertiesDiggerTests : MixpanelTestsBase
    {
        private const int IntDictionaryKey = 2;

        private PropertiesDigger _digger;

        private ObjectProperty GetProperty(string name, IList<ObjectProperty> properties)
        {
            return properties.Single(x => x.PropertyName == name);
        }

        private void CheckProperties(IList<ObjectProperty> properties)
        {
            Assert.That(properties.Count, Is.EqualTo(3));

            CheckProperty(DecimalPropertyName, PropertyNameSource.Default, DecimalPropertyValue, properties);
            CheckProperty(StringPropertyName, PropertyNameSource.Default, StringPropertyValue, properties);
            CheckProperty(DateTimePropertyName, PropertyNameSource.Default, DateTimePropertyValue, properties);
        }

        private void CheckProperty(
            string name, PropertyNameSource source, object value, IList<ObjectProperty> properties)
        {
            var decimalProperty = GetProperty(name, properties);
            Assert.That(decimalProperty.PropertyNameSource, Is.EqualTo(source));
            Assert.That(decimalProperty.Value, Is.EqualTo(value));
        }

        [SetUp]
        public void SetUp()
        {
            _digger = new PropertiesDigger();
        }

        [Test]
        public void Get_StringKeyObjectValueDictionary_Parsed()
        {
            var dic = new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue},
                {StringPropertyName, StringPropertyValue},
                {DateTimePropertyName, DateTimePropertyValue}
            };

            var properties = _digger.Get(dic);
            CheckProperties(properties);
        }

        [Test]
        public void Get_StringKeyNonObjectValueDictionary_Parsed()
        {
            var dic = new Dictionary<string, decimal>
            {
                {DecimalPropertyName, DecimalPropertyValue},
                {DecimalPropertyName2, DecimalPropertyValue2}
            };

            var properties = _digger.Get(dic);
            Assert.That(properties.Count, Is.EqualTo(2));

            CheckProperty(DecimalPropertyName, PropertyNameSource.Default, DecimalPropertyValue, properties);
            CheckProperty(DecimalPropertyName2, PropertyNameSource.Default, DecimalPropertyValue2, properties);
        }

        [Test]
        public void Get_NonStringKeyDictionary_Parsed()
        {
            var dic = new Dictionary<object, object>
            {
                {IntDictionaryKey, StringPropertyValue},
                {DecimalPropertyName, DecimalPropertyValue},
                {StringPropertyName, StringPropertyValue},
                {DateTimePropertyName, DateTimePropertyValue}
            };

            var properties = _digger.Get(dic);
            CheckProperties(properties);
        }

        [Test]
        public void Get_Hashtable_Parsed()
        {
            var hashtable = new Hashtable
            {
                {IntDictionaryKey, StringPropertyValue},
                {DecimalPropertyName, DecimalPropertyValue},
                {StringPropertyName, StringPropertyValue},
                {DateTimePropertyName, DateTimePropertyValue}
            };

            var properties = _digger.Get(hashtable);
            CheckProperties(properties);
        }

#if !(NET35 || NETSTANDARD16)
        [Test]
        public void Get_ExpandoObject_Parsed()
        {
            dynamic expando = new ExpandoObject();
            expando.DecimalProperty = DecimalPropertyValue;
            expando.StringProperty = StringPropertyValue;
            expando.DateTimeProperty = DateTimePropertyValue;

            var properties = _digger.Get(expando);
            CheckProperties(properties);
        }

        [Test]
        public void Get_Dynamic_Parsed()
        {
            dynamic dyn = new
            {
                DecimalProperty = DecimalPropertyValue,
                StringProperty = StringPropertyValue,
                DateTimeProperty = DateTimePropertyValue
            };

            var properties = _digger.Get(dyn);
            CheckProperties(properties);
        }
#endif

        /// <summary>
        /// All properties should be processed
        /// </summary>
        internal class Class1
        {
            public decimal DecimalProperty { get; set; }
            public string StringProperty { get; set; }
            public DateTime DateTimeProperty { get; set; }
        }

        [Test]
        public void Get_Class1_Parsed()
        {
            var obj = new Class1
            {
                DecimalProperty = DecimalPropertyValue,
                StringProperty = StringPropertyValue,
                DateTimeProperty = DateTimePropertyValue
            };

            var properties = _digger.Get(obj);
            CheckProperties(properties);
        }


        internal class Class2
        {
            /// <summary>
            /// DecimalPropertyName2 should be uased as a name.
            /// </summary>
            [MixpanelName(DecimalPropertyName2)]
            public decimal DecimalProperty { get; set; }

            /// <summary>
            /// StringPropertyName2 should be used as a name.
            /// </summary>
            [MixpanelName(StringPropertyName2)]
            public string StringProperty { get; set; }

            public DateTime DateTimeProperty { get; set; }
        }

        [Test]
        public void Get_Class2_Parsed()
        {
            var test = new Class2
            {
                DecimalProperty = DecimalPropertyValue2,
                StringProperty = StringPropertyValue2,
                DateTimeProperty = DateTimePropertyValue
            };

            var properties = _digger.Get(test);
            Assert.That(properties.Count, Is.EqualTo(3));

            CheckProperty(DecimalPropertyName2, PropertyNameSource.MixpanelName, DecimalPropertyValue2, properties);
            CheckProperty(StringPropertyName2, PropertyNameSource.MixpanelName, StringPropertyValue2, properties);
            CheckProperty(DateTimePropertyName, PropertyNameSource.Default, DateTimePropertyValue, properties);
        }

        [DataContract]
        internal class Class3
        {
            /// <summary>
            /// Should be ignored because there is no DataMember attr.
            /// </summary>
            [MixpanelName(DecimalPropertyName2)]
            public decimal DecimalProperty { get; set; }

            /// <summary>
            /// StringPropertyName2 should be used as name.
            /// </summary>
            [DataMember(Name = StringPropertyName2)]
            public string StringProperty { get; set; }

            /// <summary>
            /// Should be ignored because there is no DataMember attr.
            /// </summary>
            public DateTime DateTimeProperty { get; set; }

            /// <summary>
            /// Should be ignored.
            /// </summary>
            [IgnoreDataMember]
            public string IgnoredProperty { get; set; }
        }

        [Test]
        public void Get_Class3_Parsed()
        {
            var test = new Class3
            {
                DecimalProperty = DecimalPropertyValue2,
                StringProperty = StringPropertyValue2,
                DateTimeProperty = DateTimePropertyValue,
                IgnoredProperty = StringPropertyValue
            };

            var properties = _digger.Get(test);
            Assert.That(properties.Count, Is.EqualTo(1));
            CheckProperty(StringPropertyName2, PropertyNameSource.DataMember, StringPropertyValue2, properties);
        }

        internal class Class4
        {
            /// <summary>
            /// DecimalPropertyName2 should be uased as a name.
            /// </summary>
            [MixpanelName(DecimalPropertyName2)]
            public decimal DecimalProperty { get; set; }

            /// <summary>
            /// StringProperty should be used as a name because there is no DataContract attr.
            /// </summary>
            [DataMember(Name = StringPropertyName2)]
            public string StringProperty { get; set; }

            public DateTime DateTimeProperty { get; set; }

            /// <summary>
            /// Should be ignored.
            /// </summary>
            [IgnoreDataMember]
            public string IgnoredProperty { get; set; }
        }

        [Test]
        public void Get_Class4_Parsed()
        {
            var test = new Class4
            {
                DecimalProperty = DecimalPropertyValue2,
                StringProperty = StringPropertyValue,
                DateTimeProperty = DateTimePropertyValue,
                IgnoredProperty = StringPropertyValue
            };

            var properties = _digger.Get(test);
            Assert.That(properties.Count, Is.EqualTo(3));

            CheckProperty(DecimalPropertyName2, PropertyNameSource.MixpanelName, DecimalPropertyValue2, properties);
            CheckProperty(StringPropertyName, PropertyNameSource.Default, StringPropertyValue, properties);
            CheckProperty(DateTimePropertyName, PropertyNameSource.Default, DateTimePropertyValue, properties);
        }


        [DataContract]
        internal class Class5
        {
            /// <summary>
            /// DecimalPropertyName2 should be used as name because MixpanelName has a higher priority.
            /// </summary>
            [MixpanelName(DecimalPropertyName2)]
            [DataMember(Name = DecimalPropertyName)]
            public decimal DecimalProperty { get; set; }
        }

        [Test]
        public void Get_Class5_Parsed()
        {
            var obj = new Class5
            {
                DecimalProperty = DecimalPropertyValue2,
            };

            var properties = _digger.Get(obj);
            Assert.That(properties.Count, Is.EqualTo(1));
            CheckProperty(DecimalPropertyName2, PropertyNameSource.MixpanelName, DecimalPropertyValue2, properties);
        }


    }
}
