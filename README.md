# mixpanel-csharp
[Mixpanel](https://mixpanel.com/) is a great analitics platform, but unfortunetally there are no official integration library for .NET. So if you are writing code on .NET and want to use Mixpanel, then ```mixpanel-csharp``` can be an excellent choise. ```mixpanel-csharp``` main idea is to hide most api details (you don't need to remember what time formatting to use, or in which cases you should prefix properties with ```$```) and concentrate on data that you want to analize. It's also well documented, configurable and testable.

Sample usage:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.Track("Level Complete", new 
{ 
    DistinctId = "12345",
    Time = new DateTime(2013, 9, 26, 22, 33, 44, DateTimeKind.Utc),
    LevelNumber = 5
});
```
This will send the following JSON to Mixpanel:
```json
{
    "event": "Level Complete",
    "properties": {
      "token": "e3bc4100330c35722740fb8c6f5abddc",
      "distinct_id": "12345",
      "time": 1380234824,
      "LevelNumber": 5
    }
}
```
As you can see ```mixpanel-csharp``` made all dirty work for you. ```DistinctId``` is renamed to ```distinct_id```, ```Time``` value is converted to UNIX time.

Maybe you don't like that ```LevelNumber``` is still in pascal case. This can be changed easily:
```csharp
MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.TitleCase;
```
After that ```LevelNumber``` will be renamed to ```Level Number``` and the same rule will apply to all other properties.

## Providing analytics data
Most ```mixpanel-csharp``` methods that send data to Mixpanel API has ```properties``` parameter of type ```object```. This is a place where magic happens. You can choose between different types of objects.
###Dictionary
Just provide an object that inherits from ```IDictionary``` (or preferably ```IDictionary<string, object>```):
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.Track("Level Complete", new Dictionary<string, object>
    {
        {"DistinctId", "12345"},
        {"Level Number", 5},
        {"Level Name": "First Dungeon"}
    });
```
> If you use dictionaries, then rules for property name formatting will be not applied.
###Anonymous class
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.Track("Level Complete", new
    {
        DistinctId = "12345",
        LevelNumber = 5,
        LevelName = "First Dungeon"
    });
```
###Class with optional attributes
```csharp
class LevelCompletionInfo 
{
    public string DistinctId { get; set; }
    public int LevelNumber { get; set; }
    public string LevelName { get; set; }
}

var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.Track("Level Complete", new LevelCompletionInfo
    {
        DistinctId = "12345",
        LevelNumber = 5,
        LevelName = "First Dungeon"
    });
```
If you already have some existing classes, you can use them too. ```mixpanel-csharp``` respects ```DataContract```, ```DataMember``` and ```IgnoreDataMember``` attributes. There is also ```MixpanelName``` attribute. With use of these attributes you can do a lot of things:
```csharp
[DataContract]
class LevelCompletionInfo 
{
    [MixpanelName(MixpanelProperty.DistinctId)]
    [DataMember(Name = "user_id")]
    public string UserId { get; set; }
    
    [DataMember(Name = "level_number")]
    public int LevelNumber { get; set; }
    
    [MixpanelName("Level Name")]
    [DataMember(Name = "level_name")]
    public string LevelName { get; set; }
    
    [IgnoreDataMember]
    public string IgnoredProperty { get; set; }
    
    public decimal AnotherIgnoredProperty { get; set }
}
```

How such class will be parsed?

Property Name | Parse preocess
------------- | -------
UserId | Has two attributes but ```MixpanelName``` has greater priority than ```DataMember```, so the value will be ```MixpanelProperty.DistinctId```
LevelNumber | ```DataMember``` will be used, so the value will be *"level_number"*
LevelName | ```MixpanelName``` will be used, the value will be *"Level Name"*
IgnoredProperty | Ignored because of ```IgnoreDataMember``` attribute
AnotherIgnoredProperty | ignored because it's not a part of ```DataContract```

##Supported data types

Type | Description
---- | -----------
All value types except structs | Check the [list of C# value types] (http://msdn.microsoft.com/en-us/library/bfft1t3c.aspx)
String | Passed as is
DateTime | First converted to UTC (if needed) and then to Mixpanel date format 'YYYY-MM-DDThh:mm:ss'
Guid | Converted to string using default ```ToString()``` method
IEnumerable | Converted to Mixpanel list, if items are of supported types listed above

##Configuration
You can easily configure ```mixpanel-csharp```. Configuration is done using [```MixpanelConfig```](https://github.com/eealeivan/mixpanel-csharp/blob/master/src/Mixpanel/Mixpanel/MixpanelConfig.cs) class. You can use global configuration with ```MixpanelConfig.Global``` or create an instance of ```MixpanelConfig```and pass it to constructor of ```MixpanelClient```. In case when same property is set in global and local configuration, local one is used.

###JSON Serializer
```mixpanel-csharp``` uses Microsoft's [JavaScriptSerializer](http://msdn.microsoft.com/en-us/library/system.web.script.serialization.javascriptserializer(v=vs.110).aspx) by default and it was choosen just to keep the project dependency free. It's **highly recommended** that you use some other(faster) JSON serializer. 
TODO: JSON.NET example
###HTTP POST Method
If by some reason [deafult HTTP POST method](https://github.com/eealeivan/mixpanel-csharp/blob/master/src/Mixpanel/Mixpanel/DefaultHttpClient.cs) is not good for you you can provide your own implementation:
```csharp
public bool MixpanelHttpPost(string url, string formData)
{
  // Your code here...
}

MixpanelConfig.Global.HttpPostFn = MixpanelHttpPost;
```
###Error Logging
All methods in ```mixpanel-csharp``` catch all exceptions and if you do not configure error logging you never know if something is wrong.
```csharp
public void LogMixpanelErrors(string message, Exception exception)
{
  // Error logging code...
}

MixpanelConfig.Global.ErrorLogFn = LogMixpanelErrors;
```
###Custom Property Name Formatting
You can control how property names are formatted. 
```csharp
MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.TitleCase;
```
There are following formatting options:

Format | Description
------ | ------
None | No formatting is applied. This is default option.
SentenceCase | Property name will be parsed in sentence with only first word capitalized. <br>```"VeryLongProperty" -> "Very long property"```
TitleCase | Property name will be parsed in sentence with all words capitalized. <br>```"VeryLongProperty" -> "Very Long Property"```
LowerCase | Property name will be parsed in sentence with no words capitalized. <br>```"VeryLongProperty" -> "very long property"```

> If class property has ```DataMember``` attribute with ```Name``` property set or ```MixpanelName``` attribute, then property name formatting is ignored.

## Tracking
mixpanel-csharp supports both Mixpanel data types: events and profile updates. There are separate methods for each data type. 
### Track
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
###Alias
```csharp
public bool Alias(object distinctId, object alias)
```
Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.Alias("12345", "67890");
```
JSON that will be sent to ```http://api.mixpanel.com/track/```:
```json
{
    "event": "$create_alias",
    "properties": {
        "token": "e3bc4100330c35722740fb8c6f5abddc",
        "distinct_id": "12345",
        "alias": "67890"
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
###People Set Once
```csharp
public bool PeopleSetOnce(object properties)
public bool PeopleSetOnce(object distinctId, object properties)
```
Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.PeopleSetOnce(new {
    DistinctId = "12345",
    FirstLogin = new DateTime(2014, 10, 22, 0, 0, 0, DateTimeKind.Utc)
});
```
JSON that will be sent to ```http://api.mixpanel.com/engage/```:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",
    "$set_once": {
        "FirstLogin": "2014-10-22T00:00:00"
    }
}
```
###People Add
```csharp
public bool PeopleAdd(object properties)
public bool PeopleAdd(object distinctId, object properties)
```
When parsing ```properties``` object only numeric properties will be processed, all other types will be ignored. 

Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.PeopleAdd(new {
    DistinctId = "12345",
    TransactionsCount = 1,
    MoneySpent = 24.99M
});
```
JSON that will be sent to ```http://api.mixpanel.com/engage/```:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",
    "$add": {
        "TransactionsCount": 1,
        "MoneySpent": 24.99
    }
}
```
###People Append
```csharp
public bool PeopleAppend(object properties)
public bool PeopleAppend(object distinctId, object properties)
```

Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.PeopleAppend(new {
    DistinctId = "12345",
    PowerUps = "Bubble Lead",
    ItemIds = 1568
});
```
JSON that will be sent to ```http://api.mixpanel.com/engage/```:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",
    "$append": {
        "PowerUps": "Bubble Lead",
        "ItemIds": 1568
    }
}
```
###People Union
```csharp
public bool PeopleUnion(object properties)
public bool PeopleUnion(object distinctId, object properties)
```
When parsing ```properties``` object only collection type properties will be processed, all other types will be ignored. 

Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.PeopleUnion(new {
    DistinctId = "12345",
    PowerUps = new [] { "Bubble Lead", "Super Mushroom" },
    ItemIds = new []  { 1568, 7653 }
});
```
JSON that will be sent to ```http://api.mixpanel.com/engage/```:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",
    "$union": {
        "PowerUps": ["Bubble Lead", "Super Mushroom"],
        "ItemIds": [1568, 7653]
    }
}
```
###People Unset
```csharp
public bool PeopleUnset(IEnumerable<string> propertyNames)
public bool PeopleUnset(object distinctId, IEnumerable<string> propertyNames)
```
Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.PeopleUnset("12345", new [] { "Days Overdue" });
```
JSON that will be sent to ```http://api.mixpanel.com/engage/```:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",
    "$unset": ["Days Overdue"]
}
```
###People Delete
```csharp
public bool PeopleDelete(object distinctId)
```
Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.PeopleDelete("12345");
```
JSON that will be sent to ```http://api.mixpanel.com/engage/```:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",
    "$delete": ""
}
```
###People Track Charge
```csharp
public bool PeopleTrackCharge(object distinctId, decimal amount)
public bool PeopleTrackCharge(object distinctId, decimal amount, DateTime time)
```
Example:
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
mc.PeopleTrackCharge("12345", 24.99M, new DateTime(2014, 10, 22, 0, 0, 0, DateTimeKind.Utc));
```
JSON that will be sent to ```http://api.mixpanel.com/engage/```:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",
    "$append": {
        "$transactions": {
            "$amount": 24.99,
            "$time": "2014-10-22T00:00:00"
        }
    }
}
```
