# mixpanel-csharp

Mixpanel C# integration library.

```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.Track("Level Complete", new { DistinctId = 123, LevelNumber = 5 });
```
This will send the following json to Mixpanel:
```json
{
  "event": "Level Complete",
  "properties": {
    "token": "e3bc4100330c35722740fb8c6f5abddc",
    "distinct_id": "123",
    "LevelNumber": 5
  }
}
```

## Supported property types
Type | Description
---- | -----------
All value types except structs | You can find list of C# value types [here](http://msdn.microsoft.com/en-us/library/bfft1t3c.aspx)
String | Passed as is
DateTime | First converted to UTC and then to Mixpanel format 'YYYY-MM-DDThh:mm:ss'
Guid | Converted to string using default *ToString()* method
IEnumerable | Converted to list, if items are of supported types listed above

## Extensibility
mixpanel-csharp has a lot of extensibility points.

### Json Serializer
Mixpanel-csharp uses Microsoft's [JavaScriptSerializer](http://msdn.microsoft.com/en-us/library/system.web.script.serialization.javascriptserializer(v=vs.110).aspx) by default and it was choosen just to keep it dependency free. It's highly recommended that you use some other(fast) JSON serializer. 

### HTTP Client
If by some reason deafult HTTP client that does HTTP POST requests is not good for you you can provide your own:
```csharp
public bool CustomHttpPost(string url, string formData)
{
  // Your code here
}

MixpanelConfig.Global.HttpPostFn = CustomHttpPost;
```
