using System;
using System.Collections;
using System.Collections.Generic;

namespace Mixpanel.Parsers
{
    internal static class CollectionParser
    {
        public static bool IsCollection(object rawCollection)
        {
            return rawCollection is IEnumerable && !(rawCollection is string);
        }

        public static ValueParseResult Parse(
            object rawCollection,
            Func<object, ValueParseResult> itemParseFn)
        {
            switch (rawCollection)
            {
                case null:
                    return ValueParseResult.CreateFail("Can't be null.");

                case string _:
                    return ValueParseResult.CreateFail("Can't be string.");

                case IEnumerable collection:
                    var validItems = new List<object>();
                    foreach (object item in collection)
                    {
                        ValueParseResult itemParseResult = itemParseFn(item);
                        if (!itemParseResult.Success)
                        {
                            continue;
                        }

                        validItems.Add(itemParseResult.Value);
                    }
                    return ValueParseResult.CreateSuccess(validItems);

                default:
                    return ValueParseResult.CreateFail("Expected type is: IEnumerable.");
            }
        }
    }
}