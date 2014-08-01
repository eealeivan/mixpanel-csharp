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

## Supported data types
Type | Description
---- | -----------
All value types except structs | Check the [list of C# value types] (http://msdn.microsoft.com/en-us/library/bfft1t3c.aspx)
String | Passed as is
DateTime | First converted to UTC (if needed) and then to Mixpanel date format 'YYYY-MM-DDThh:mm:ss'
Guid | Converted to string using default *ToString()* method
IEnumerable | Converted to Mixpanel list, if items are of supported types listed above

## Extensibility
mixpanel-csharp has a lot of extensibility points.

### Json Serializer
Mixpanel-csharp uses Microsoft's [JavaScriptSerializer](http://msdn.microsoft.com/en-us/library/system.web.script.serialization.javascriptserializer(v=vs.110).aspx) by default and it was choosen just to keep the project dependency free. It's highly recommended that you use some other(faster) JSON serializer. 

### HTTP Client
If by some reason deafult HTTP client that does HTTP POST requests is not good for you you can provide your own implementation:
```csharp
public bool CustomHttpPost(string url, string formData)
{
  // Your code here
}

MixpanelConfig.Global.HttpPostFn = CustomHttpPost;
```

## Tracking
mixpanel-csharp supports both Mixpanel data types: events and profile updates. There are separate methods for each data type. 
### Tracking events
```csharp
public bool Track(string @event, object properties)
public bool Track(string @event, object distinctId, object properties)
```
Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.Track("Level Complete", new {
    DistinctId = "12345",
    Ip = "111.111.111.111",
    Time = new DateTime(2013, 11, 30, 0, 0, 0, DateTimeKind.Utc),
    LevelNumber = 5,
    LevelName = "Super hard level"
});
```
JSON that will be sent to ```http://api.mixpanel.com/track/```:
```json
{
  "event": "Level Complete",
  "properties": {
    "token": "e3bc4100330c35722740fb8c6f5abddc",
    "distinct_id": "12345",
    "ip": "111.111.111.111",
    "time": 1385769600,
    "LevelNumber": 5,
    "LevelName": "Super hard level"
  }
}
```
###People Set
```csharp
public bool PeopleSet(object properties)
public bool PeopleSet(object distinctId, object properties)
```
Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.PeopleSet(new {
    DistinctId = "12345",
    Ip = "111.111.111.111",
    Time = new DateTime(2013, 11, 30, 0, 0, 0, DateTimeKind.Utc),
    IgnoreTime = true,
    FirstName = "Darth",
    LastName = "Vader",
    Name = "Darth Vader",
    Created = new DateTime(2014, 10, 22, 0, 0, 0, DateTimeKind.Utc),
    Email = "darth.vader@gmail.com",
    Phone = "123456",
    Sex = "M",
    Kills = 215
});
```
JSON that will be sent to ```http://api.mixpanel.com/engage/```:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",
    "$ip": "111.111.111.111",
    "$time": 1385769600,
    "$ignore_time": true,
    "$set": {
        "$first_name": "Darth",
        "$last_name": "Vader",
        "$name": "Darth Vader",
        "$created": "2014-10-22T00:00:00",
        "$email": "darth.vader@gmail.com",
        "$phone": "123456",
        "Sex": "M",
        "Kills": 215
    }
}
```
