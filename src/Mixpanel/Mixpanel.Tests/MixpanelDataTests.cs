using System;
using System.Collections.Generic;
using Mixpanel.Builders;
using Mixpanel.Exceptions;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MixpanelDataTests
    {
        private readonly Dictionary<string, string> _specialPropsBindings =
            new Dictionary<string, string>
            {
                {"event", MixpanelProperty.Event},

                {"token", MixpanelProperty.Token},

                {"distinct_id", MixpanelProperty.DistinctId},
                {"distinctid", MixpanelProperty.DistinctId},

                {"ip", MixpanelProperty.Ip},

                {"time", MixpanelProperty.Time},
            };

        [Test]
        public void MixpanelData_sets_props()
        {
            var md = new MixpanelData(_specialPropsBindings);

            md.ParseAndSetProperties(new
            {
                Event = "test-event",
                DistinctId = "12345",
                Prop1 = "val1"
            });
            md.SetProperty(MixpanelProperty.Token, "token1");
            md.SetProperty("Prop2", "val2");

            Assert.AreEqual(3, md.SpecialProps.Count);
            Assert.AreEqual("test-event", md.SpecialProps[MixpanelProperty.Event]);
            Assert.AreEqual("12345", md.SpecialProps[MixpanelProperty.DistinctId]);
            Assert.AreEqual("token1", md.SpecialProps[MixpanelProperty.Token]);

            Assert.AreEqual(2, md.Props.Count);
            Assert.AreEqual("val1", md.Props["Prop1"]);
            Assert.AreEqual("val2", md.Props["Prop2"]);
        }

        [Test]
        public void MixpanelData_override_props()
        {
            var md = new MixpanelData(_specialPropsBindings);
            md.ParseAndSetProperties(new
            {
                Event = "test-event1",
                Prop1 = "val1"
            });
            md.SetProperty(MixpanelProperty.Event, "test-event2");
            md.SetProperty("Prop1", "val2");

            Assert.AreEqual(1, md.SpecialProps.Count);
            Assert.AreEqual("test-event2", md.SpecialProps[MixpanelProperty.Event]);

            Assert.AreEqual(1, md.Props.Count);
            Assert.AreEqual("val2", md.Props["Prop1"]);
        }

        [Test]
        public void MixpanelData_gets_special_props()
        {
            var md = new MixpanelData(_specialPropsBindings);
            md.SetProperty(MixpanelProperty.Event, "test-event");
            md.SetProperty(MixpanelProperty.DistinctId, 12345);

            Assert.AreEqual("test-event", md.GetSpecialProp(MixpanelProperty.Event));
            Assert.AreEqual(12345, md.GetSpecialProp(MixpanelProperty.DistinctId));
            Assert.AreEqual(12345, md.GetSpecialRequiredProp(MixpanelProperty.DistinctId));
            Assert.AreEqual("12345", md.GetSpecialProp(MixpanelProperty.DistinctId, x => x.ToString()));
            Assert.AreEqual("12345", md.GetSpecialRequiredProp(MixpanelProperty.DistinctId, convertFn: x => x.ToString()));
        }

        [Test]
        [ExpectedException(typeof(MixpanelObjectStructureException),
            ExpectedMessage = "'token' property is not set.")]
        public void MixpanelData_throws_exception_when_get_required_no_set_prop()
        {
            var md = new MixpanelData(_specialPropsBindings);
            md.GetSpecialRequiredProp(MixpanelProperty.Token);
        }

        [Test]
        [ExpectedException(typeof(MixpanelRequiredPropertyNullOrEmptyException),
            ExpectedMessage = "'token' property can't be null.")]
        public void MixpanelData_throws_exception_when_get_required_is_null()
        {
            var md = new MixpanelData(_specialPropsBindings);
            md.SetProperty(MixpanelProperty.Token, null);
            md.GetSpecialRequiredProp(MixpanelProperty.Token);
        }

        [Test]
        [ExpectedException(typeof(Exception),
            ExpectedMessage = "'token' should be equal to '321'.")]
        public void MixpanelData_throws_exception_when_get_required_prop_validation_fails()
        {
            var md = new MixpanelData(_specialPropsBindings);
            md.SetProperty(MixpanelProperty.Token, "123");
            md.GetSpecialRequiredProp(MixpanelProperty.Token, validateFn:
                x =>
                {
                    if(!x.ToString().Equals("321"))
                        throw new Exception("'token' should be equal to '321'.");
                });
        }
    }
}