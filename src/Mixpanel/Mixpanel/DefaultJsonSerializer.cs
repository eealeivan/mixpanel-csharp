#if !(PORTABLE || PORTABLE40)
using System.Web.Script.Serialization;
#endif
using System;

namespace Mixpanel
{
    internal class DefaultJsonSerializer
    {
        public string Serialize(object obj)
        {
#if !(PORTABLE || PORTABLE40)
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
#else
            throw new NotImplementedException(
                "There is no default JSON serializer in portable builds. Please use configuration to set it.");
#endif
        }
    }
}