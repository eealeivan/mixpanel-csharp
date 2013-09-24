using System.Collections.Generic;

namespace Mixpanel.Builders
{
    /// <summary>
    /// Base class for building mixpanel objects (dictionaries) that will be serialized to json 
    /// and sent to Mixpanel.
    /// </summary>
    internal abstract class BuilderBase
    {
        public abstract IDictionary<string, object> Object { get; } 
    }
}