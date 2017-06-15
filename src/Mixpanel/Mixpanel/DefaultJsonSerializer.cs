#if JSON
using System.Web.Script.Serialization;
#endif
using System;

namespace Mixpanel
{
    internal class DefaultJsonSerializer
    {
        public string Serialize(object obj)
        {
#if JSON
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
#else
            throw new NotImplementedException(
                "There is no default JSON serializer in .NET Standard builds. Please use configuration to set it.");
#endif
        }
    }
}