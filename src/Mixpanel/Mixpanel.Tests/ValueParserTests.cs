using System;
using Mixpanel.Builders;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class ValueParserTests
    {
        [Test]
        public void ValueParser_parses_values()
        {
            var vp = new ValueParser();

            var strVal = vp.Parse("str");
            Assert.AreEqual("str", strVal.Item1);
            Assert.AreEqual(true, strVal.Item2);

            var intVal = vp.Parse(5);
            Assert.AreEqual(5, intVal.Item1);
            Assert.AreEqual(true, intVal.Item2);

            var longVal = vp.Parse(5L);
            Assert.AreEqual(5L, longVal.Item1);
            Assert.AreEqual(true, longVal.Item2);

            var doubleVal = vp.Parse(5D);
            Assert.AreEqual(5D, doubleVal.Item1);
            Assert.AreEqual(true, doubleVal.Item2);

            var decimalVal = vp.Parse(5M);
            Assert.AreEqual(5M, decimalVal.Item1);
            Assert.AreEqual(true, decimalVal.Item2);
            
            var boolVal = vp.Parse(true);
            Assert.AreEqual(true, boolVal.Item1);
            Assert.AreEqual(true, boolVal.Item2);

            var dateVal = vp.Parse(new DateTime(2013, 7, 9, 15, 23, 44, DateTimeKind.Utc));
            Assert.AreEqual("2013-07-09T15:23:44", dateVal.Item1);
            Assert.AreEqual(true, dateVal.Item2);

            var floatVal = vp.Parse(5F);
            Assert.AreEqual(5F, floatVal.Item1);
            Assert.AreEqual(true, floatVal.Item2);

            var shortVal = vp.Parse((short)5);
            Assert.AreEqual((short)5, shortVal.Item1);
            Assert.AreEqual(true, shortVal.Item2);
            
            var ushortVal = vp.Parse((ushort)5);
            Assert.AreEqual((ushort)5, ushortVal.Item1);
            Assert.AreEqual(true, ushortVal.Item2);

            var uintVal = vp.Parse(5U);
            Assert.AreEqual(5U, uintVal.Item1);
            Assert.AreEqual(true, uintVal.Item2);

            var ulongVal = vp.Parse(5UL);
            Assert.AreEqual(5UL, ulongVal.Item1);
            Assert.AreEqual(true, ulongVal.Item2);

            var byteVal = vp.Parse((byte)5);
            Assert.AreEqual((byte)5, byteVal.Item1);
            Assert.AreEqual(true, byteVal.Item2);

            var sbyteVal = vp.Parse((sbyte)5);
            Assert.AreEqual((sbyte)5, sbyteVal.Item1);
            Assert.AreEqual(true, sbyteVal.Item2);
            
            var charVal = vp.Parse('c');
            Assert.AreEqual('c', charVal.Item1);
            Assert.AreEqual(true, charVal.Item2);

            int? nullable1 = 5;
            var nullIntVal = vp.Parse(nullable1);
            Assert.AreEqual(5, nullIntVal.Item1);
            Assert.AreEqual(true, nullIntVal.Item2);

            double? nullable2 = null;
            var nullDoubleVal = vp.Parse(nullable2);
            Assert.AreEqual(null, nullDoubleVal.Item1);
            Assert.AreEqual(true, nullDoubleVal.Item2);

            var nullValue = vp.Parse(null);
            Assert.AreEqual(null, nullValue.Item1);
            Assert.AreEqual(true, nullValue.Item2);
        }
    }
}