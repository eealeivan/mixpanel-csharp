using System.Web.Script.Serialization;

namespace Mixpanel
{
    internal class DefaultJsonSerializer
    {
        public string Serialize(object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
    }
}