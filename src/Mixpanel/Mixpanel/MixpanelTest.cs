using System;
using System.Collections.Generic;

namespace Mixpanel
{
    public class MixpanelTest
    {
        public IDictionary<string, object> Data { get; set; }
        public Exception DataException { get; set; }

        public string Json { get; set; }
        public Exception JsonException { get; set; }

        public string Base64 { get; set; }
        public Exception Base64Exception { get; set; }
    }
}