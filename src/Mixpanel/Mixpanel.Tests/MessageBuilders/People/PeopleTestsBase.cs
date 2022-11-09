using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mixpanel.MessageBuilders;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;
using NUnit.Framework;

namespace Mixpanel.Tests.Unit.MessageBuilders.People
{
    public abstract class PeopleTestsBase : MixpanelTestsBase
    {
        protected abstract string OperationName { get; }

        internal List<ObjectProperty> CreateSuperProperties(params ObjectProperty[] objectProperties)
        {
            return objectProperties.ToList();
        }

        internal void AssertMessageSuccess(
            MessageBuildResult messageBuildResult,
            (string name, object value)[] messageProperties,
            (string name, object value)[] operationProperties)
        {
            AssertMessageSuccess(messageBuildResult);
            AssetMessageProperties(messageBuildResult, messageProperties);
            AssertOperationProperties(messageBuildResult, operationProperties);
        }

        internal void AssertMessageSuccess(
            MessageBuildResult messageBuildResult)
        {
            Assert.That(messageBuildResult.Success, Is.True);
            Assert.That(messageBuildResult.Error, Is.Null);
        }

        internal void AssetMessageProperties(
            MessageBuildResult messageBuildResult,
            (string name, object value)[] messageProperties)
        {

            IDictionary<string, object> message = messageBuildResult.Message;
            Assert.That(message.Count, Is.EqualTo(messageProperties.Length + 1 /*OPERATION_NAME*/));

            foreach ((string propertyName, object expectedValue) in messageProperties)
            {
                bool propertyExists = message.TryGetValue(propertyName, out var actualValue);

                Assert.That(propertyExists, "Missing property: " + propertyName);
                Assert.That(expectedValue, Is.EqualTo(actualValue));
            }
        }

        internal void AssertOperationProperties(
            MessageBuildResult messageBuildResult,
            (string name, object value)[] operationProperties)
        {
            IDictionary<string, object> message = messageBuildResult.Message;
            Assert.That(message.ContainsKey(OperationName));
            var operation = (Dictionary<string, object>)message[OperationName];

            if (operationProperties == null)
            {
                Assert.That(operation.Count, Is.EqualTo(0));
                return;
            }

            Assert.That(operation.Count, Is.EqualTo(operationProperties.Length));

            foreach ((string propertyName, object expectedValue) in operationProperties)
            {
                bool propertyExists = operation.TryGetValue(propertyName, out var actualValue);

                Assert.That(propertyExists, "Missing property: " + propertyName);
                if (CollectionParser.IsCollection(actualValue))
                {
                    Assert.That(expectedValue, Is.EquivalentTo((IEnumerable)actualValue));
                }
                else
                {
                    Assert.That(expectedValue, Is.EqualTo(actualValue));
                }
            }
        }

        internal void AssertOperation(
            MessageBuildResult messageBuildResult,
            Action<object> operationAssertFn)
        {
            IDictionary<string, object> message = messageBuildResult.Message;
            Assert.That(message.ContainsKey(OperationName));

            operationAssertFn(message[OperationName]);
        }

        internal void AssertMessageFail(MessageBuildResult messageBuildResult)
        {
            Assert.That(messageBuildResult.Success, Is.False);
            Assert.That(messageBuildResult.Error, Is.Not.Null);
            Assert.That(messageBuildResult.Message, Is.Null);
        }
    }
}