using System;
using System.Runtime.Serialization;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that some property is of wrong type.
    /// For example <see cref="MixpanelProperty.Token"/> is of type <see cref="DateTime"/>.
    /// </summary>
    public class MixpanelPropertyWrongTypeException : Exception
    {
        public MixpanelPropertyWrongTypeException()
        {
        }

        public MixpanelPropertyWrongTypeException(string message) : base(message)
        {
        }

        public MixpanelPropertyWrongTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MixpanelPropertyWrongTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}