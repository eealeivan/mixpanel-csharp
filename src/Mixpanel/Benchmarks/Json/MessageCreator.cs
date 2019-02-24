using System;
using System.Collections.Generic;
using AutoFixture;

namespace Benchmarks.Json
{
    public static class MessageCreator
    {
        public static Dictionary<string, object> Dictionary()
        {
            var fixture = new Fixture();

            return new Dictionary<string, object>
            {
                {"event", fixture.Create<string>()},
                {
                    "properties", new Dictionary<string, object>
                    {
                        {"token", fixture.Create<string>()},
                        {"distinct_id", fixture.Create<string>()},
                        {"IntProperty", fixture.Create<int>()},
                        {"DecimalProperty", fixture.Create<decimal>()},
                        {
                            "ArrayProperty",
                            new[]
                            {
                                fixture.Create<int>(),
                                fixture.Create<int>(),
                                fixture.Create<int>()
                            }
                        }
                    }
                }
            };
        }
    }
}