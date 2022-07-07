using System;
using System.Collections.Generic;
using Mixpanel.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Mixpanel.Tests.Json
{
    [TestFixture]
    public class MixpanelJsonSerializerTests : MixpanelTestsBase
    {
        [Test]
        public void When_SimpleObject_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {StringPropertyName, StringPropertyValue},
                {IntPropertyName, IntPropertyValue }
            };

            AssertSameAsJsonNet(dic);
        }

        [Test]
        public void When_CollectionOfObjects_Then_TheSameAsJsonNet()
        {
            var dics = new[]
            {
                new Dictionary<string, object>
                {
                    {StringPropertyName, StringPropertyValue},
                    {IntPropertyName, IntPropertyValue}
                },
                new Dictionary<string, object>
                {
                    {DoublePropertyName, DoublePropertyValue},
                    {StringPropertyName2, StringPropertyValue2}
                }
            };

            AssertSameAsJsonNet(dics);
        }

        [Test]
        public void When_Null_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {StringPropertyName, StringPropertyValue},
                {IntPropertyName, null }
            };

            AssertSameAsJsonNet(dic);
        }

        [Test]
        public void When_Bool_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {"Fast", true},
                {"Slow", false}
            };

            AssertSameAsJsonNet(dic);
        }

        [Test]
        public void When_Char_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {"char1", 'a'},
                {"char2", '-' }
            };

            AssertSameAsJsonNet(dic);
        }

        [Test]
        public void When_Escape_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {"quotation", "test\"test"},
                {"reverseSolidus", @"test\test"},
                {"solidus", "test/test"},
                {"backspace", "test\btest"},
                {"formFeed", "test\ftest"},
                {"newLine", "test\ntest"},
                {"carriageReturn", "test\rtest"},
                {"horizontalTab", "test\ttest"},
                {"face", "😂"}
            };

            AssertSameAsJsonNet(dic);
        }

        [Test]
        public void When_WholeNumbers_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {"byte", (byte)20},
                {"byteMin", byte.MinValue},
                {"byteMax", byte.MaxValue},
                {"sbyte", (sbyte)40},
                {"sbyteMin", sbyte.MinValue},
                {"sbyteMax", sbyte.MaxValue},
                {"short", (short)1234},
                {"shortMin", short.MinValue},
                {"shortMax", short.MaxValue},
                {"ushort", (ushort)12345},
                {"ushortMin", ushort.MinValue},
                {"ushortMax", ushort.MaxValue},
                {"int", 1000},
                {"intMin", int.MinValue},
                {"intMax", int.MaxValue},
                {"uint", (uint)2000},
                {"uintMin", uint.MinValue},
                {"uintMax", uint.MaxValue},
                {"long", 100000L},
                {"longMin", long.MinValue},
                {"longMax", long.MaxValue},
                {"ulong", (ulong)200000},
                {"ulongMin", ulong.MinValue},
                {"ulongMax", ulong.MaxValue}
            };

            AssertSameAsJsonNet(dic);
        }

        [Test]
        public void When_FloatingNumbers_Then_CorrectValues()
        {
            // Json.NET handles floating min/max with different precision
            // and also finishes all floating numbers with .0

            var dic = new Dictionary<string, object>
            {
                {"float", 2.5F},
                {"floatMin", float.MinValue},
                {"floatMax", float.MaxValue},
                {"double", 3.456D},
                {"doubleMin", double.MinValue},
                {"doubleMax", double.MaxValue},
                {"decimalMin", decimal.MinValue},
                {"decimalMax", decimal.MaxValue},
                {"decimal", 23.5M},
                {"decimal1", 1M},
                {"decimal2", 1.0M}
            };

            string mixpanelJsonSerializerResult = MixpanelJsonSerializer.Serialize(dic);
            string expectedJson = "";


#if NET45
            expectedJson =
                "{" +
                "\"float\":2.5," +
                "\"floatMin\":-3.402823E+38," +
                "\"floatMax\":3.402823E+38," +
                "\"double\":3.456," +
                "\"doubleMin\":-1.79769313486232E+308," +
                "\"doubleMax\":1.79769313486232E+308," +
                "\"decimalMin\":-79228162514264337593543950335," +
                "\"decimalMax\":79228162514264337593543950335," +
                "\"decimal\":23.5," +
                "\"decimal1\":1," +
                "\"decimal2\":1.0" +
                "}";
#endif

#if NET6
            expectedJson =
                "{" +
                "\"float\":2.5," +
                "\"floatMin\":-3.4028235E+38," +
                "\"floatMax\":3.4028235E+38," +
                "\"double\":3.456," +
                "\"doubleMin\":-1.7976931348623157E+308," +
                "\"doubleMax\":1.7976931348623157E+308," +
                "\"decimalMin\":-79228162514264337593543950335," +
                "\"decimalMax\":79228162514264337593543950335," +
                "\"decimal\":23.5," +
                "\"decimal1\":1," +
                "\"decimal2\":1.0" +
                "}";
#endif



            Assert.That(mixpanelJsonSerializerResult, Is.EqualTo(expectedJson));
        }

        [Test]
        public void When_Guid_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {"guid", Guid.NewGuid()}
            };

            AssertSameAsJsonNet(dic);
        }

        [Test]
        public void When_TimeSpan_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {"timeSpan", TimeSpan.FromSeconds(5)}
            };

            AssertSameAsJsonNet(dic);
        }

        [Test]
        public void When_InnerObject_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {StringPropertyName, StringPropertyValue},
                {
                    "Inner", new Dictionary<string, object>
                    {
                        {StringPropertyName2, StringPropertyValue2},
                        {DecimalPropertyName, DecimalPropertyValue}
                    }
                }
            };

            AssertSameAsJsonNet(dic);
        }

        [Test]
        public void When_Array_Then_TheSameAsJsonNet()
        {
            var dic = new Dictionary<string, object>
            {
                {DoublePropertyName, DoublePropertyValue},
                {StringPropertyName, StringPropertyArray}
            };

            AssertSameAsJsonNet(dic);
        }

        private void AssertSameAsJsonNet(object obj)
        {
            string mixpanelJsonSerializerResult = MixpanelJsonSerializer.Serialize(obj);
            string jsonNetSerializerResult = JsonConvert.SerializeObject(obj);

            Assert.That(mixpanelJsonSerializerResult, Is.EqualTo(jsonNetSerializerResult));
        }
    }
}