using System;
using System.Collections.Generic;
using AutoFixture;

namespace Benchmarks.Json
{
    public static class MessageCreator
    {
        public static Dictionary<string, object> Dynamic()
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

        public static Dictionary<string, object> Static()
        {
            return new Dictionary<string, object>
            {
                {"event", "Level Complete"},
                {
                    "properties", new Dictionary<string, object>
                    {
                        {"token", "e3bc4100330c35722740fb8c6f5abddc"},
                        {"distinct_id", "123456"},
                        {"IntProperty", 545},
                        {"DecimalProperty", 555.5M},
                        {
                            "ArrayProperty",
                            new[]
                            {
                                1,
                                2,
                                3
                            }
                        }
                    }
                }
            };
        }
    }
}