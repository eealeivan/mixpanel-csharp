# mixpanel-csharp [![NuGet Version](http://img.shields.io/nuget/v/mixpanel-csharp.svg?style=flat)](https://www.nuget.org/packages/mixpanel-csharp/)
[Mixpanel](https://mixpanel.com/) is a great analitics platform, but unfortunetally there are no official integration library for .NET. So if you are writing code on .NET and want to use Mixpanel, then ```mixpanel-csharp``` can be an excellent choise. ```mixpanel-csharp``` main idea is to hide most api details (you don't need to remember what time formatting to use, or in which cases you should prefix properties with ```$```) and concentrate on data that you want to analize.

## Features
- Supports full [Mixpanel HTTP Tracking API](https://mixpanel.com/help/reference/http)
- [Send tracking messages](https://github.com/eealeivan/mixpanel-csharp/wiki/Sending-messages) synchronously or asynchronously, pack them into batches, save messages to send them later
- Pass [message data](https://github.com/eealeivan/mixpanel-csharp/wiki/Message-data) in form that you prefer: predifined contract,  `IDictionary<string, object>`, anonymous type or dynamic
- Add properties globally to all messages with super properties. Usable for properties such as `distinct_id`
- Great [configurability](https://github.com/eealeivan/mixpanel-csharp/wiki/Configuration). For example you can provide your own JSON serializer or function that will make HTTP requests
- No dependencies. Keeps your project clean
- Runs on many platforms: .NET 3.5, .NET 4.0, .NET 4.5 and [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- Good [documentation](https://github.com/eealeivan/mixpanel-csharp/wiki)

## Sample usage for track message
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
await mc.TrackAsync("Level Complete", new {
    DistinctId = "12345",
    Time = new DateTime(2013, 11, 30, 0, 0, 0, DateTimeKind.Utc),
    LevelNumber = 5
});
```
This will send the following JSON to `http://api.mixpanel.com/track/`:
```json
{
  "event": "Level Complete",
  "properties": {
    "token": "e3bc4100330c35722740fb8c6f5abddc",
    "distinct_id": "12345",
    "time": 1385769600,
    "LevelNumber": 5
  }
}
```

## Sample usage for profile message
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
await mc.PeopleSetAsync(new {
    DistinctId = "12345",   
    Name = "Darth Vader",    
    Kills = 215
});
```
This will send the following JSON to `http://api.mixpanel.com/engage/`:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",    
    "$set": {       
        "$name": "Darth Vader",      
        "Kills": 215
    }
}
```

## Copyright
Copyright © 2015 Aleksandr Ivanov

## Licence
```mixpanel-csharp``` is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php). Refer to [LICENSE](https://github.com/eealeivan/mixpanel-csharp/blob/master/LICENSE) for more information.
