using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class PropertiesDiggerTests
    {
        [Test]
        public void PropertiesDigger_parses_string_key_object_value_dictionary()
        {
            var now = DateTime.Now;

            var inDic = new Dictionary<string, object>
            {
                {"property1", 1},
                {"property2", "val"},
                {"property3", now}
            };

            var outDic = new PropertiesDigger().Get(inDic);
            Assert.AreEqual(3, outDic.Count);
            Assert.AreEqual(1, outDic["property1"]);
            Assert.AreEqual("val", outDic["property2"]);
            Assert.AreEqual(now, outDic["property3"]);
        }

        [Test]
        public void PropertiesDigger_parses_string_key_non_object_value_dictionary()
        {
            var inDic = new Dictionary<string, decimal>
            {
                {"property1", 1M},
                {"property2", 2M}
            };

            var outDic = new PropertiesDigger().Get(inDic);
            Assert.AreEqual(2, outDic.Count);
            Assert.AreEqual(1M, outDic["property1"]);
            Assert.AreEqual(2M, outDic["property2"]);
        }

        [Test]
        public void PropertiesDigger_parses_non_string_key_dictionary()
        {
            var now = DateTime.Now;
            var inDic = new Dictionary<object, object>
            {
                {"property1", 1M},
                {2, "val2"},
                {"property3", now}
            };

            var outDic = new PropertiesDigger().Get(inDic);
            Assert.AreEqual(2, outDic.Count);
            Assert.AreEqual(1M, outDic["property1"]);
            Assert.AreEqual(now, outDic["property3"]);
        }


        [Test]
        public void PropertiesDigger_parses_hash_table()
        {
            var now = DateTime.Now;

            var hashtable = new Hashtable
            {
                {"property1", 1M},
                {2, "val2"},
                {"property3", now}
            };

            var outDic = new PropertiesDigger().Get(hashtable);
            Assert.AreEqual(2, outDic.Count);
            Assert.AreEqual(1M, outDic["property1"]);
            Assert.AreEqual(now, outDic["property3"]);
        }

        [Test]
        public void PropertiesDigger_parses_expando_object()
        {
            var now = DateTime.Now;
            dynamic expando = new ExpandoObject();
            expando.property1 = 1M;
            expando.property2 = "val2";
            expando.property3 = now;

            var outDic = new PropertiesDigger().Get(expando);
            Assert.AreEqual(3, outDic.Count);
            Assert.AreEqual(1, outDic["property1"]);
            Assert.AreEqual("val2", outDic["property2"]);
            Assert.AreEqual(now, outDic["property3"]);
        }

        [Test]
        public void PropertiesDigger_parses_dynamic()
        {
            var now = DateTime.Now;
            dynamic dyn = new
            {
                Property1 = 1M,
                Property2 = "val2",
                Property3 = now
            };

            var outDic = new PropertiesDigger().Get(dyn);
            Assert.AreEqual(3, outDic.Count);
            Assert.AreEqual(1, outDic["Property1"]);
            Assert.AreEqual("val2", outDic["Property2"]);
            Assert.AreEqual(now, outDic["Property3"]);
        }

        internal class Test1
        {
            public decimal Property1 { get; set; }
            public string Property2 { get; set; }
            public DateTime Property3 { get; set; }
        }

        [Test]
        public void PropertiesDigger_parses_class()
        {
            var now = DateTime.Now;
            var test1 = new Test1
            {
                Property1 = 1M,
                Property2 = "val2",
                Property3 = now
            };

            var outDic = new PropertiesDigger().Get(test1);
            Assert.AreEqual(3, outDic.Count);
            Assert.AreEqual(1, outDic["Property1"]);
            Assert.AreEqual("val2", outDic["Property2"]);
            Assert.AreEqual(now, outDic["Property3"]);
        }


        internal class Test2
        {
            [MixpanelProperty("property1")]
            public decimal Property1 { get; set; }

            public string Property2 { get; set; }

            [MixpanelProperty("property3")]
            public DateTime Property3 { get; set; }
        }

        [Test]
        public void PropertiesDigger_parses_class_with_mixpanel_property_attribute()
        {
            var now = DateTime.Now;
            var test1 = new Test2
            {
                Property1 = 1M,
                Property2 = "val2",
                Property3 = now
            };

            var outDic = new PropertiesDigger().Get(test1);
            Assert.AreEqual(3, outDic.Count);
            Assert.AreEqual(1, outDic["property1"]);
            Assert.AreEqual("val2", outDic["Property2"]);
            Assert.AreEqual(now, outDic["property3"]);
        }

        internal class Test3
        {
            [MixpanelProperty("property1")]
            public decimal Property1 { get; set; }

            [IgnoreDataMember]
            public string Property2 { get; set; }

            [DataMember(Name = "property3")]
            public DateTime Property3 { get; set; }
        }

        [Test]
        public void PropertiesDigger_parses_class_with_ignore_data_member()
        {
            var now = DateTime.Now;
            var test1 = new Test3
            {
                Property1 = 1M,
                Property2 = "val2",
                Property3 = now
            };

            var outDic = new PropertiesDigger().Get(test1);
            Assert.AreEqual(2, outDic.Count);
            Assert.AreEqual(1, outDic["property1"]);
            Assert.AreEqual(now, outDic["property3"]);
        }

        [DataContract]
        internal class Test4
        {
            [MixpanelProperty("mp_property1")]
            [DataMember(Name = "property1")]
            public decimal Property1 { get; set; }

            [IgnoreDataMember]
            public string Property2 { get; set; }

            [DataMember(Name = "property3")]
            public DateTime Property3 { get; set; }

            public string Property4 { get; set; }

            [MixpanelProperty("mp_property5")]
            public string Property5 { get; set; }

            [DataMember]
            public string Property6 { get; set; }
        }

        [Test]
        public void PropertiesDigger_parses_class_with_all_possible_attributes()
        {
            var now = DateTime.Now;
            var test4 = new Test4
            {
                Property1 = 1M,
                Property2 = "val2",
                Property3 = now,
                Property4 = "p4",
                Property5 = "p5",
                Property6 = "p6"
            };

            var outDic = new PropertiesDigger().Get(test4);
            Assert.AreEqual(4, outDic.Count);
            Assert.AreEqual(1M, outDic["mp_property1"]);
            Assert.AreEqual(now, outDic["property3"]);
            Assert.AreEqual("p5", outDic["mp_property5"]);
            Assert.AreEqual("p6", outDic["Property6"]);
        }
    }
}
