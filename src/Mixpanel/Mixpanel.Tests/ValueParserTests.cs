using System;
using System.Collections.Generic;
using Mixpanel.Core;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class ValueParserTests
    {
        private ValueParser _vp;

        [SetUp]
        public void SetUp()
        {
            _vp = new ValueParser();
        }

        #region Valid types

        [Test]
        public void Parse_String_Parsed()
        {
            var val = _vp.Parse("str");
            Assert.That(val.Value, Is.EqualTo("str"));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Int_Parsed()
        {
            var val = _vp.Parse(5);
            Assert.That(val.Value, Is.EqualTo(5));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Long_Parsed()
        {
            var val = _vp.Parse(5L);
            Assert.That(val.Value, Is.EqualTo(5L));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Double_Parsed()
        {
            var val = _vp.Parse(5D);
            Assert.That(val.Value, Is.EqualTo(5D));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Decimal_Parsed()
        {
            var val = _vp.Parse(5M);
            Assert.That(val.Value, Is.EqualTo(5M));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Bool_Parsed()
        {
            var val = _vp.Parse(false);
            Assert.That(val.Value, Is.False);
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_DateTime_Parsed()
        {
            var val = _vp.Parse(new DateTime(2013, 7, 9, 15, 23, 44, DateTimeKind.Utc));
            Assert.That(val.Value, Is.EqualTo("2013-07-09T15:23:44"));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Float_Parsed()
        {
            var val = _vp.Parse(5F);
            Assert.That(val.Value, Is.EqualTo(5F));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Short_Parsed()
        {
            var val = _vp.Parse((short)5);
            Assert.That(val.Value, Is.EqualTo(5));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_UnsignedShort_Parsed()
        {
            var val = _vp.Parse((short)5);
            Assert.That(val.Value, Is.EqualTo(5));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_UnsignedInt_Parsed()
        {
            var val = _vp.Parse(5U);
            Assert.That(val.Value, Is.EqualTo(5U));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_UnsignedLong_Parsed()
        {
            var val = _vp.Parse(5UL);
            Assert.That(val.Value, Is.EqualTo(5UL));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Byte_Parsed()
        {
            var val = _vp.Parse((byte)5);
            Assert.That(val.Value, Is.EqualTo(5));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_SignedByte_Parsed()
        {
            var val = _vp.Parse((sbyte)5);
            Assert.That(val.Value, Is.EqualTo(5));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Char_Parsed()
        {
            var val = _vp.Parse('c');
            Assert.That(val.Value, Is.EqualTo('c'));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_NullableWithValue_Parsed()
        {
            var val = _vp.Parse(new int?(5));
            Assert.That(val.Value, Is.EqualTo(5));
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_NullableWithoutValue_Parsed()
        {
            var val = _vp.Parse(new int?());
            Assert.That(val.Value, Is.Null);
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Null_Parsed()
        {
            var val = _vp.Parse(null);
            Assert.That(val.Value, Is.Null);
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_ListOfStrings_Parsed()
        {
            var list = new List<string> { "one", "two", "three" };
            var val = _vp.Parse(list);
            Assert.That(val.Value, Is.TypeOf<List<object>>());
            for (int i = 0; i < list.Count; i++)
            {
                Assert.That(list[i], Is.EqualTo(((List<object>)val.Value)[i]));
            }
            Assert.That(val.IsValid, Is.True);
        }

        [Test]
        public void Parse_Guid_Parsed()
        {
            var guid = Guid.NewGuid();
            var val = _vp.Parse(guid);
            Assert.That(val.Value, Is.EqualTo(guid.ToString()));
            Assert.That(val.IsValid, Is.True);
        }

        #endregion

        #region Invalid types

        // TODO

        #endregion
    }
}