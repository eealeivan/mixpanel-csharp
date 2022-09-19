using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using Mixpanel.MessageProperties;
using NUnit.Framework;

namespace Mixpanel.Tests.Unit.MessageProperties
{
    [TestFixture]
    public class PropertiesDiggerTests : MixpanelTestsBase
    {
        [Test]
        public void Given_StringObjectDic_When_ValidInput_Then_AllParsed()
        {
            var dic = new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue},
                {StringPropertyName, StringPropertyValue},
                {DateTimePropertyName, DateTimePropertyValue}
            };

            var properties = PropertiesDigger.Get(dic, PropertyOrigin.RawProperty).ToArray();
            CheckProperties(properties);
        }

        [Test]
        public void Given_StringDecimalDic_When_ValidInput_Then_AllParsed()
        {
            var dic = new Dictionary<string, decimal>
            {
                {DecimalPropertyName, DecimalPropertyValue},
                {DecimalPropertyName2, DecimalPropertyValue2}
            };

            var properties = PropertiesDigger.Get(dic, PropertyOrigin.RawProperty).ToArray();
            Assert.That(properties.Count, Is.EqualTo(2));

            CheckProperty(DecimalPropertyName, PropertyNameSource.Default, DecimalPropertyValue, properties);
            CheckProperty(DecimalPropertyName2, PropertyNameSource.Default, DecimalPropertyValue2, properties);
        }

        [Test]
        public void Given_ObjectObjectDic_When_MixedInput_Then_OnlyValidParsed()
        {
            var dic = new Dictionary<object, object>
            {
                {1, IntPropertyValue},
                {DecimalPropertyName, DecimalPropertyValue},
                {StringPropertyName, StringPropertyValue},
                {DateTimePropertyName, DateTimePropertyValue}
            };

            var properties = PropertiesDigger.Get(dic, PropertyOrigin.RawProperty).ToArray();
            CheckProperties(properties);
        }

        [Test]
        public void Given_Hashtable_When_MixedInput_Then_OnlyValidParsed()
        {
            var hashtable = new Hashtable
            {
                {1, IntPropertyValue},
                {DecimalPropertyName, DecimalPropertyValue},
                {StringPropertyName, StringPropertyValue},
                {DateTimePropertyName, DateTimePropertyValue}
            };

            var properties = PropertiesDigger.Get(hashtable, PropertyOrigin.RawProperty).ToArray();
            CheckProperties(properties);
        }

        [Test]
        public void Given_ExpandoObject_When_ValidInput_Then_AllParsed()
        {
            dynamic expando = new ExpandoObject();
            expando.DecimalProperty = DecimalPropertyValue;
            expando.StringProperty = StringPropertyValue;
            expando.DateTimeProperty = DateTimePropertyValue;

            var properties =
                ((IEnumerable<ObjectProperty>)PropertiesDigger.Get(expando, PropertyOrigin.RawProperty))
                .ToArray();
            CheckProperties(properties);
        }


        [Test]
        public void Given_Dynamic_When_ValidInput_Then_AllParsed()
        {
            dynamic dyn = new
            {
                DecimalProperty = DecimalPropertyValue,
                StringProperty = StringPropertyValue,
                DateTimeProperty = DateTimePropertyValue
            };

            var properties =
                ((IEnumerable<ObjectProperty>)PropertiesDigger.Get(dyn, PropertyOrigin.RawProperty))
                .ToArray();
            CheckProperties(properties);
        }

        [Test]
        public void Given_AnonymousClass_When_ValidInput_Then_AllParsed()
        {
            var obj = new
            {
                DecimalProperty = DecimalPropertyValue,
                StringProperty = StringPropertyValue,
                DateTimeProperty = DateTimePropertyValue
            };

            var properties = PropertiesDigger.Get(obj, PropertyOrigin.RawProperty).ToArray();
            CheckProperties(properties);
        }

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
        public void Given_Class_When_ValidInput_Then_AllParsed()
        {
            var obj = new Class1
            {
                DecimalProperty = DecimalPropertyValue,
                StringProperty = StringPropertyValue,
                DateTimeProperty = DateTimePropertyValue
            };

            var properties = PropertiesDigger.Get(obj, PropertyOrigin.RawProperty).ToArray();
            CheckProperties(properties);
        }

        internal class Class2
        {
            /// <summary>
            /// DecimalPropertyName2 should be used as a name.
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
        public void Given_Class_When_MixpanelNameAttribute_Then_MixpanelNameUsed()
        {
            var obj = new Class2
            {
                DecimalProperty = DecimalPropertyValue2,
                StringProperty = StringPropertyValue2,
                DateTimeProperty = DateTimePropertyValue
            };

            var properties = PropertiesDigger.Get(obj, PropertyOrigin.RawProperty).ToArray();
            Assert.That(properties.Count, Is.EqualTo(3));

            CheckProperty(
                DecimalPropertyName2, PropertyNameSource.MixpanelName, DecimalPropertyValue2, properties);
            CheckProperty(
                StringPropertyName2, PropertyNameSource.MixpanelName, StringPropertyValue2, properties);
            CheckProperty(
                DateTimePropertyName, PropertyNameSource.Default, DateTimePropertyValue, properties);
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
        public void Given_Class_When_DataContractAttribute_Then_DataMemberParsed()
        {
            var obj = new Class3
            {
                DecimalProperty = DecimalPropertyValue2,
                StringProperty = StringPropertyValue2,
                DateTimeProperty = DateTimePropertyValue,
                IgnoredProperty = StringPropertyValue
            };

            var properties = PropertiesDigger.Get(obj, PropertyOrigin.RawProperty).ToArray();
            Assert.That(properties.Count, Is.EqualTo(1));
            CheckProperty(StringPropertyName2, PropertyNameSource.DataMember, StringPropertyValue2, properties);
        }

        internal class Class4
        {
            /// <summary>
            /// DecimalPropertyName2 should be used as a name.
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
        public void Given_Class_When_DifferentAttributes_Then_Parsed()
        {
            var obj = new Class4
            {
                DecimalProperty = DecimalPropertyValue2,
                StringProperty = StringPropertyValue,
                DateTimeProperty = DateTimePropertyValue,
                IgnoredProperty = StringPropertyValue
            };

            var properties = PropertiesDigger.Get(obj, PropertyOrigin.RawProperty).ToArray();
            Assert.That(properties.Count, Is.EqualTo(3));

            CheckProperty(
                DecimalPropertyName2, PropertyNameSource.MixpanelName, DecimalPropertyValue2, properties);
            CheckProperty(
                StringPropertyName, PropertyNameSource.Default, StringPropertyValue, properties);
            CheckProperty(
                DateTimePropertyName, PropertyNameSource.Default, DateTimePropertyValue, properties);
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
        public void Given_Class_When_MixpanelNameAndDataMemberAttributes_Then_MixpanelNameUsed()
        {
            var obj = new Class5
            {
                DecimalProperty = DecimalPropertyValue2,
            };

            var properties = PropertiesDigger.Get(obj, PropertyOrigin.RawProperty).ToArray();
            Assert.That(properties.Count, Is.EqualTo(1));
            CheckProperty(
                DecimalPropertyName2, PropertyNameSource.MixpanelName, DecimalPropertyValue2, properties);
        }

        private void CheckProperties(ObjectProperty[] properties)
        {
            Assert.That(properties.Count, Is.EqualTo(3));

            CheckProperty(DecimalPropertyName, PropertyNameSource.Default, DecimalPropertyValue, properties);
            CheckProperty(StringPropertyName, PropertyNameSource.Default, StringPropertyValue, properties);
            CheckProperty(DateTimePropertyName, PropertyNameSource.Default, DateTimePropertyValue, properties);
        }

        private void CheckProperty(
            string name,
            PropertyNameSource source,
            object value,
            ObjectProperty[] properties)
        {
            var objectProperty = GetProperty(name, properties);
            Assert.That(objectProperty.PropertyNameSource, Is.EqualTo(source));
            Assert.That(objectProperty.Value, Is.EqualTo(value));
        }

        private ObjectProperty GetProperty(
            string name,
            ObjectProperty[] properties)
        {
            return properties.Single(x => x.PropertyName == name);
        }
    }
}
